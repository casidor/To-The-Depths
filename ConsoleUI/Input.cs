using GameCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    internal class Input
    {
        public bool ProcessInput(GameCore.Player player, GameCore.GameField field)
        {
            if (!Console.KeyAvailable) return true;
            var key = Console.ReadKey(true).Key;
            while (Console.KeyAvailable) Console.ReadKey(true);
            switch (key)
            {
                case ConsoleKey.W:
                    player.Move(0, -1, field);
                    break;
                case ConsoleKey.S:
                    player.Move(0, 1, field);
                    break;
                case ConsoleKey.A:
                    player.Move(-1, 0, field);
                    break;
                case ConsoleKey.D:
                    player.Move(1, 0, field);
                    break;
                case ConsoleKey.Escape:
                    return false;
            }
            return true;
        }
        public GameState ProcessMenuInput(Renderer renderer)
        {
            int selected = 0;
            int count = GameSymbols.Options.Length;
            renderer.RenderMainMenu(selected);
            while (true) {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + count) % count;
                        renderer.RenderMainMenu(selected);
                        break;
                    case ConsoleKey.S:
                        selected = (selected + 1) % count;
                        renderer.RenderMainMenu(selected);
                        break;
                    case ConsoleKey.Enter:
                        if (selected == 0) return GameState.Running;
                        if (selected == 1) return GameState.Exit;
                        break;
                }
            }
        }
    }
}
