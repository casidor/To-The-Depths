using System;
using System.Collections.Generic;
using System.Text;
using GameCore;
using GameCore.Models;
using GameCore.World;

namespace ConsoleUI
{
    public class Renderer
    {
        public void Render(GameField field, Player player)
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder sb = new StringBuilder(field.Width * field.Height + field.Height * 2);
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    if (x == player.X && y == player.Y)
                    {
                        sb.Append(GameSymbols.Player);
                    }
                    else
                    {
                        sb.Append(field.GetCell(x, y));
                    }
                }
                sb.AppendLine();
            }
            Console.Write(sb.ToString());
            RenderSideBar(player);
        }
        public void RenderSideBar(Player player)
        {
            int x = Config.FieldWidth + 3;
            //Stats
            Console.SetCursorPosition(x, 0);
            Console.WriteLine("Player Stats:");
            Console.SetCursorPosition(x, 2);
            Console.WriteLine($"{GameSymbols.Gold} Gold: {player.Gold}");
            Console.SetCursorPosition(x, 4);
            Console.WriteLine($"{GameSymbols.Key} Keys: {player.KeysCollected}/{Config.KeysAmount}");
            //Missions
            Console.SetCursorPosition(x, 6);
            Console.Write("=== MISSION ===");
            Console.SetCursorPosition(x, 8);
            if (player.KeysCollected < Config.KeysAmount)
            {
                Console.Write($"Collect {Config.KeysAmount} keys!");
            }
            else
            {
                Console.Write("Exit is OPEN!    ");
                Console.SetCursorPosition(x, 9);
                Console.Write("Find the exit!   ");
            }
            //Controls
            Console.SetCursorPosition(x, 11);
            Console.Write("=== CONTROLS ===");
            Console.SetCursorPosition(x, 13);
            Console.Write("W/A/S/D or Arrows - Move");
            Console.SetCursorPosition(x, 14);
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
        public void RenderEscapePopup(Player player)
        {
            int popupWidth = 40;
            int popupHeight = 9;
            int startX = (Config.ConsoleWidth - popupWidth) / 2;
            int startY = (Config.ConsoleHeight - popupHeight) / 2;
            for (int y = 0; y < popupHeight; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                for (int x = 0; x < popupWidth; x++)
                {
                    if (y == 0 && x == 0) Console.Write("╔");
                    else if (y == 0 && x == popupWidth - 1) Console.Write("╗");
                    else if (y == popupHeight - 1 && x == 0) Console.Write("╚");
                    else if (y == popupHeight - 1 && x == popupWidth - 1) Console.Write("╝");
                    else if (y == 0 || y == popupHeight - 1) Console.Write("═");
                    else if (x == 0 || x == popupWidth - 1) Console.Write("║");
                    else Console.Write(" ");
                }
            }
            string title = "YOU ESCAPED!";
            Console.SetCursorPosition(startX + (popupWidth - title.Length) / 2, startY + 2);
            Console.Write(title);
            string goldStr = $"Gold Collected: {player.Gold}";
            Console.SetCursorPosition(startX + (popupWidth - goldStr.Length) / 2, startY + 4);
            Console.Write(goldStr);
            string prompt = "Press any key to exit...";
            Console.SetCursorPosition(startX + (popupWidth - prompt.Length) / 2, startY + 6);
            Console.Write(prompt);
        }
        public void RenderHelp()
        {
            Console.Clear();
            string[] helpText = {
                "=== HOW TO PLAY ===",
                "",
                $"Your goal is to collect all {Config.KeysAmount} keys to unlock the exit.",
                $"Watch your steps and gather gold on your way!",
                "",
                "=== LEGEND ===",
                $"{GameSymbols.Player} - You",
                $"{GameSymbols.Key} - Key",
                $"{GameSymbols.Gold} - Gold",
                $"{GameSymbols.Exit} - Exit",
                "",
                "=== CONTROLS ===",
                "W, A, S, D / Arrows - Move",
                "ESC - Return to Menu",
                "",
                "Press any key to go back..."
            };
            int maxLen = 0;
            foreach (var line in helpText)
            {
                if (line.Length > maxLen) maxLen = line.Length;
            }
            int startX = (Config.ConsoleWidth - maxLen) / 2;
            int startY = (Config.ConsoleHeight - helpText.Length) / 2;
            for (int i = 0; i < helpText.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.WriteLine(helpText[i]);
            }
        }
    }
}
