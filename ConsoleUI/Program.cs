using GameCore;

namespace ConsoleUI
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Random random = new Random();
            LevelGenerator levelGenerator = new LevelGenerator();
            var (field, x, y) = levelGenerator.Generate(70, 25, random);
            Player player = new Player(x, y);
            Renderer renderer = new Renderer();
            Input input = new Input();
            bool running = true;
            Console.Clear();
            while (running)
            {
                renderer.Render(field, player);
                running = input.ProcessInput(player, field);
            }
        }
    }
}
