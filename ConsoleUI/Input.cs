using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    internal class Input
    {
        public bool ProcessInput(GameCore.Player player, GameCore.GameField field)
        {
            var key = Console.ReadKey(true).Key;
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
    }
}
