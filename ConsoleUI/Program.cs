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
            Random random = new Random();
            Renderer renderer = new Renderer();
            Input input = new Input();
            Console.Clear();
            GameField? field = null;
            Player? player = null;
            GameState state = GameState.MainMenu;
            do
            {
                switch (state)
                {
                    case GameState.MainMenu:
                        state = input.ProcessMenuInput(renderer);
                        break;
                    case GameState.Generating:
                        Console.Clear();
                        LevelGenerator levelGenerator = new LevelGenerator();
                        var generated = levelGenerator.Generate(Config.FieldWidth, Config.FieldHeight, random);
                        field = generated.field;
                        player = new Player(generated.x, generated.y);
                        state = GameState.Running;
                        break;
                    case GameState.Running:
                        {
                            renderer.Render(field, player);
                            var (isRunning, interaction) = input.ProcessInput(player, field);
                            if (!isRunning)
                            {
                                state = GameState.MainMenu;
                                break;
                            }
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
                            if (interaction == InteractionResult.PlayerAttacked)
                            {
                                renderer.Render(field, player);
                                renderer.RenderAttackPopup();
                                Console.ReadKey(true);
                            }
                            renderer.Render(field, player);
                            if (player.IsExited)
                            {
                                renderer.Render(field, player);
                                renderer.RenderEscapePopup(player);
                                Console.ReadKey(true);
                                state = GameState.MainMenu;
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
