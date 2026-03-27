namespace ConsoleUI
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;
            GameCore.GameField field = new GameCore.GameField(20, 10);
            field.GenerateField();
            GameCore.Player player = new GameCore.Player(1, 1);
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
