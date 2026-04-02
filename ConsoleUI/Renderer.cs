using System;
using System.Collections.Generic;
using System.Text;
using GameCore;

namespace ConsoleUI
{
    public class Renderer
    {
        public void Render(GameField field, Player player)
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    if (x == player.X && y == player.Y)
                    {
                        Console.Write(GameSymbols.Player);
                    }
                    else
                    {
                        Console.Write(field.GetCell(x, y));
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
