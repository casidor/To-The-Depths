using GameCore;
using GameCore.Models;
using GameCore.World;

namespace ConsoleUI
{
    internal class Program
    {
        static void InitConsole() 
        {
            if (OperatingSystem.IsWindows())
            {
                Console.SetWindowSize(Config.ConsoleWidth, Config.ConsoleHeight);
                Console.SetBufferSize(Config.ConsoleWidth, Config.ConsoleHeight);
                Console.CursorVisible = false;
                Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
        }
        static void Main()
        {
            InitConsole();
            int currentSeed = new Random().Next();
            Random random = new Random(currentSeed);
            Renderer renderer = new Renderer();
            Input input = new Input();
            Console.Clear();
            GameField? field = null;
            Player? player = null;
            EnemyAI? enemyAI = null;
            bool hasUnsavedProgress = false;
            GameState state = GameState.MainMenu;
            do
            {
                switch (state)
                {
                    case GameState.MainMenu:
                        state = input.ProcessMenuInput(renderer);
                        break;
                    case GameState.LoadGame:
                        var data = SaveManager.Load();
                        if (data != null)
                        {
                            Console.Clear();
                            currentSeed = data.Seed;
                            random = new Random(currentSeed + data.Floor);
                            var restored = new LevelGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random);
                            field = restored.field;
                            player = new Player(restored.x, restored.y, data.HP, data.MaxHP, data.Gold, data.Keys, data.Floor);
                            enemyAI = new EnemyAI(random);
                            state = GameState.Running;
                        }
                        else
                        {
                            renderer.RenderPopup("No save found or file corrupted!", "", "Press any key...");
                            Console.ReadKey(true);
                            state = GameState.MainMenu;
                        }
                        break;
                    case GameState.Generating:
                        Console.Clear();
                        currentSeed = new Random().Next();
                        random = new Random(currentSeed + 1);
                        LevelGenerator levelGenerator = new LevelGenerator();
                        var generated = levelGenerator.Generate(Config.FieldWidth, Config.FieldHeight, random);
                        field = generated.field;
                        player = new Player(generated.x, generated.y);
                        enemyAI = new EnemyAI(random);
                        SaveManager.Save(player, currentSeed);
                        hasUnsavedProgress = false;
                        state = GameState.Running;
                        break;
                    case GameState.Running:
                        {
                            renderer.Render(field, player);
                            var (isRunning, interaction) = input.ProcessInput(player, field);
                            var worldInteraction = InteractionResult.None;
                            if (isRunning)
                            {
                                enemyAI.BuildDistanceMap(field, player);
                                worldInteraction = enemyAI.UpdateEnemies(field, player);
                            }
                            if (!isRunning)
                            {
                                if (hasUnsavedProgress)
                                {
                                    bool confirmExit = input.ProcessExitConfirm(renderer, field, player);
                                    if (!confirmExit)
                                    {
                                        continue;
                                    }
                                }
                                state = GameState.MainMenu;
                                break;
                            } else hasUnsavedProgress = true;
                            if (interaction == InteractionResult.Altar)
                            {
                                renderer.Render(field, player);
                                if (field[player.X, player.Y] is Altar currentAltar)
                                {
                                    state = input.ProcessAltarInput(renderer, player, currentAltar);
                                }
                                else
                                {
                                    state = GameState.Running;
                                }
                            }
                            if (interaction == InteractionResult.PlayerAttacked || worldInteraction == InteractionResult.PlayerAttacked)
                            {
                                renderer.Render(field, player);
                                renderer.RenderAttackPopup();
                                Console.ReadKey(true);
                            }
                            renderer.Render(field, player);
                            if (player.IsExited)
                            {
                                renderer.Render(field, player);
                                renderer.RenderDescentPopup(player);
                                Console.ReadKey(true);
                                random = new Random(currentSeed + player.CurrentFloor + 1);
                                var nextfloor = new LevelGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random);
                                field = nextfloor.field;
                                player.Descend(nextfloor.x, nextfloor.y);
                                enemyAI = new EnemyAI(random);
                                SaveManager.Save(player, currentSeed);
                                hasUnsavedProgress = false;
                            }
                            else if (!player.IsAlive)
                            {
                                renderer.Render(field, player);
                                renderer.RenderDeathPopup(player);
                                Console.ReadKey(true);
                                state = GameState.MainMenu;
                            }
                        }
                        break;
                    case GameState.Help:
                        renderer.RenderHelp();
                        Console.ReadKey(true);
                        state = GameState.MainMenu;
                        break;
                }
            } while (state != GameState.Exit);
        }
    }
}
