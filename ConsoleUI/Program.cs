using GameCore;
using GameCore.Models;
using GameCore.World;

namespace ConsoleUI
{
    public enum GameState
    {
        MainMenu,
        Running,
        Help,
        Exit
    }
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
            GameState state = GameState.MainMenu;
            do
            {
                switch (state)
                {
                    case GameState.MainMenu:
                        state = input.ProcessMenuInput(renderer);
                        break;
                    case GameState.Running:
                        {
                            Console.Clear();
                            LevelGenerator levelGenerator = new LevelGenerator();
                            var (field, x, y) = levelGenerator.Generate(Config.FieldWidth, Config.FieldHeight, random);
                            Player player = new Player(x, y);
                            renderer.Render(field, player);
                            while (!player.isExited)
                            {
                                if (!input.ProcessInput(player, field))
                                {
                                    state = GameState.MainMenu;
                                    break;
                                }
                                renderer.Render(field, player);
                                if (player.isExited)
                                {
                                    renderer.Render(field, player);
                                    renderer.RenderEscapePopup(player);
                                    Console.ReadKey(true);
                                    state = GameState.MainMenu;
                                }
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
