using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    internal class Input
    {
        public bool ProcessInput(Player player, GameField field)
        {
            var key = Console.ReadKey(true).Key;
            while (Console.KeyAvailable) Console.ReadKey(true);
            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    player.Move(0, -1, field);
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    player.Move(0, 1, field);
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    player.Move(-1, 0, field);
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
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
            int count = GameTexts.Options.Length;
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
                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % count;
                        renderer.RenderMainMenu(selected);
                        break;
                    case ConsoleKey.Enter:
                        if (selected == 0) return GameState.Running;
                        if (selected == 1) return GameState.Help;
                        if(selected == 2) return GameState.Exit;
                        break;
                }
            }
        }
    }
}
