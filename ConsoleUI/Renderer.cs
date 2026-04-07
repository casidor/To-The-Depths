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
            RenderSideBar(player);
        }
        public void RenderSideBar(Player player)
        {
            int x = Config.FieldWidth + 3;
            //Stats
            Console.SetCursorPosition(x, 0);
            Console.WriteLine("Player Stats:");
            Console.SetCursorPosition(x, 2);
            Console.WriteLine($"{GameSymbols.Gold} Score: {player.Score}");
            Console.SetCursorPosition(x, 4);
            Console.WriteLine($"{GameSymbols.Key} Keys: {player.KeysCollected}/{Config.KeysAmount}");
            //Controls
            Console.SetCursorPosition(x, 9);
            Console.Write("=== CONTROLS ===");
            Console.SetCursorPosition(x, 11);
            Console.Write("W/A/S/D or Arrows - Move");
            Console.SetCursorPosition(x, 12);
            Console.Write("ESC - Menu");
        }
        public void RenderMainMenu (int selected)
        {
            Console.Clear();
            int maxLen = 0;
            int startY = 2;
            foreach (var line in GameSymbols.Title)
            {
                if (line.Length > maxLen) maxLen = line.Length;
            }
            int startX = (Config.ConsoleWidth - maxLen) / 2;
            for (int i = 0; i < GameSymbols.Title.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.WriteLine(GameSymbols.Title[i]);
            }
            for (int i = 0; i < GameSymbols.Options.Length; i++)
            {
                string line = (i == selected ? "> " : "  ") + GameSymbols.Options[i];
                int x = (Config.ConsoleWidth - line.Length) / 2;
                Console.SetCursorPosition(x, startY + GameSymbols.Title.Length + 2 + i * 2);
                Console.Write(line);
            }
        }
    }
}
