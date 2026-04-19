using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
                        sb.Append(player.Color);
                        sb.Append(player.Symbol);
                        sb.Append(GameColors.Reset);
                    }
                    else
                    {
                        var obj = field[x, y];
                        sb.Append(obj.Color);
                        sb.Append(obj.Symbol);
                        sb.Append(GameColors.Reset);
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
            Console.SetCursorPosition(x, 0);
            Console.WriteLine($"=== FLOOR {player.CurrentFloor} ===");
            //Stats
            Console.SetCursorPosition(x, 2);
            Console.WriteLine($"=== Player Stats ===");
            Console.SetCursorPosition(x, 4);
            Console.WriteLine($"{GameColors.Health}{GameSymbols.Health} Health: {player.HP}/{player.MaxHP}{GameColors.Reset}     ");
            Console.SetCursorPosition(x, 6);
            Console.WriteLine($"{GameColors.GoldText}{GameSymbols.Gold} Gold: {player.GoldCollected}{GameColors.Reset}     ");
            Console.SetCursorPosition(x, 8);
            Console.WriteLine($"{GameColors.KeyText}{GameSymbols.Key} Keys: {player.KeysCollected}/{Config.KeysAmount}{GameColors.Reset}     ");
            //Missions
            Console.SetCursorPosition(x, 10);
            Console.WriteLine($"=== MISSION ===");
            Console.SetCursorPosition(x, 12);
            if (player.KeysCollected < Config.KeysAmount)
            {
                Console.WriteLine($"Collect {Config.KeysAmount} keys!");
                Console.SetCursorPosition(x, 13);
                Console.WriteLine($"                             ");
            }
            else
            {
                Console.Write("Exit is OPEN!    ");
                Console.SetCursorPosition(x, 13);
                Console.Write("Find the exit!   ");
            }
            //Controls
            Console.SetCursorPosition(x, 15);
            Console.WriteLine($"=== CONTROLS ===");
            Console.SetCursorPosition(x, 17);
            Console.Write("W/A/S/D or Arrows - Move");
            Console.SetCursorPosition(x, 18);
            Console.Write("ESC - Menu");
        }
        public void RenderMainMenu (int selected)
        {
            int maxLen = 0;
            int startY = 2;
            foreach (var line in GameTexts.Title)
            {
                if (line.Length > maxLen) maxLen = line.Length;
            }
            int startX = (Config.ConsoleWidth - maxLen) / 2;
            for (int i = 0; i < GameTexts.Title.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.WriteLine(GameTexts.Title[i]);
            }
            for (int i = 0; i < GameTexts.Options.Length; i++)
            {
                string line = (i == selected ? "> " : "  ") + GameTexts.Options[i];
                int x = (Config.ConsoleWidth - line.Length) / 2;
                Console.SetCursorPosition(x, startY + GameTexts.Title.Length + 2 + i * 2);
                Console.Write(line);
            }
        }
        public void RenderPopup(params string[] lines)
        {
            int maxLen = 0;
            foreach (var line in lines)
            {
                if (line.Length > maxLen) maxLen = line.Length;
            }
            int popupWidth = Math.Max(maxLen + 4, 40);
            int popupHeight = Math.Max(lines.Length + 4, 9);
            int startX = (Config.ConsoleWidth - popupWidth) / 2;
            int startY = (Config.ConsoleHeight - popupHeight) / 2;
            int textStartY = startY + (popupHeight - lines.Length) / 2;
            for (int y = 0; y < popupHeight; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                StringBuilder sb = new StringBuilder(popupWidth);
                for (int x = 0; x < popupWidth; x++)
                {
                    if (y == 0 && x == 0) sb.Append("╔");
                    else if (y == 0 && x == popupWidth - 1) sb.Append("╗");
                    else if (y == popupHeight - 1 && x == 0) sb.Append("╚");
                    else if (y == popupHeight - 1 && x == popupWidth - 1) sb.Append("╝");
                    else if (y == 0 || y == popupHeight - 1) sb.Append("═");
                    else if (x == 0 || x == popupWidth - 1) sb.Append("║");
                    else sb.Append(" ");
                }
                int currentLineIndex = startY + y - textStartY;
                if (currentLineIndex >= 0 && currentLineIndex < lines.Length)
                {
                    string text = lines[currentLineIndex];
                    int textStartX = (popupWidth - text.Length) / 2;
                    sb.Remove(textStartX, text.Length);
                    sb.Insert(textStartX, text);
                }
                Console.Write(sb.ToString());
            }
        }
        public void RenderExitConfirm(int selected)
        {
            string[] lines = [
                "--- UNSAVED PROGRESS ---",
                "",
                "You have unsaved progress.",
                "Are you sure you want to exit?",
                "Current floor progress will be lost.",
                "",
                (selected == 0 ? "> EXIT TO MENU" : "  EXIT TO MENU"),
                "",
                (selected == 1 ? "> CANCEL" : "  CANCEL")];
            RenderPopup(lines);
        }
        public void RenderDescentPopup(Player player)
        {
            string[] lines = [
                "--- DEEPER INTO THE DARK ---",
                "",
                "The heavy stone door closes behind you.",
                "The air grows colder, and the shadows lengthen.",
                "",
                $"Entering Floor {player.CurrentFloor + 1}...",
                "",
                "Game Autosaved.",
                "",
                "Press any key to descend..."];
            RenderPopup(lines);
        }
        public void RenderAttackPopup()
        {
            string[] lines = [
                "--- AMBUSH! ---",
                "",
                "A thief jumps out of the shadows!",
                "He strikes you and grabs your purse.",
                "",
                $"Health lost: {Config.EnemyDamage}",
                $"Gold stolen: {Config.GoldStolen}",
                "",
                "Press any key to continue..."];
            RenderPopup(lines);
        }
        public void RenderAltarMenu(int selected)
        {
            string[] lines = [
                "=== ANCIENT ALTAR ===",
                "",
                $"Heal {Config.AltarHeal} HP for {Config.HealCost} Gold?",
                "",
                (selected == 0 ? "> YES " : "  YES"),
                "",
                (selected == 1 ? ">  NO" : "   NO ")];
            RenderPopup(lines);
        }
        public void RenderEscapePopup(Player player)
        {
            RenderPopup(["YOU ESCAPED!",
                "",
                $"Gold Collected: {player.GoldCollected}",
                "",
                "Press any key to exit..."]);
        }
        public void RenderDeathPopup(Player player)
        {
            RenderPopup(["YOU DIED!",
                "",
                $"Gold Collected: {player.GoldCollected}",
                "",
                "Press any key to exit..."]);
        }
        public void RenderHelp()
        {
            Console.Clear();
            string[] helpText = [
            "                      === HOW TO PLAY ===                          ",
            "",
            $"  GOAL: Descend deeper, survive each level, and find a way out of the dungeon.",
            "",
            "                        === LEGEND ===                             ",
            "",
            $"  {GameColors.Player}{GameSymbols.Player}{GameColors.Reset} - You (Hero)          {GameColors.Enemy}{GameSymbols.Enemy}{GameColors.Reset} - Enemy (Thief)",
            $"  {GameColors.Key}{GameSymbols.Key}{GameColors.Reset} - Key                 {GameColors.Gold}{GameSymbols.Gold}{GameColors.Reset} - Gold (+{Config.GoldAmount})",
            $"  {GameColors.Exit}{GameSymbols.Exit}{GameColors.Reset} - Exit                {GameColors.Altar}{GameSymbols.Altar}{GameColors.Reset} - Altar (healing)",
            $"  {GameColors.Wall}{GameSymbols.Wall}{GameColors.Reset} - Wall                {GameColors.Floor}{GameSymbols.Floor}{GameColors.Reset} - Floor",
            "",
            "                       === CONTROLS ===                            ",
            "",
            "  W/A/S/D or Arrow Keys  -  Move",
            "  ESC                    -  Menu / Cancel",
            "  ENTER                  -  Confirm (in menus)",
            "",
            "                      === GAME MECHANICS ===                        ",
            "",
            $"  {GameColors.Health}{GameSymbols.Health}{GameColors.Reset} Health can be restored at altars using gold.",
            $"  {GameColors.Enemy}{GameSymbols.Enemy}{GameColors.Reset} Enemies will chase and attack you if you get too close.",
            $"  {GameColors.Key}{GameSymbols.Key}{GameColors.Reset} Collect keys to unlock the exit and progress deeper.",
            $"  {GameColors.Gold}{GameSymbols.Gold}{GameColors.Reset} Gold is used for healing and survival.",
            $"  ⚑ Progress is saved when descending to the next level.",
            "",
            "  TIP: Descend as deep as possible. Something below might free you.",
            "  For full game guide, see README.md in the project repository.",
            "",
            "                  Press any key to return to menu..."
            ];
            int startX = (Config.ConsoleWidth - 83) / 2;
            int startY = Math.Max(0, (Config.ConsoleHeight - helpText.Length) / 2);
            for (int i = 0; i < helpText.Length && (startY + i) < Config.ConsoleHeight; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.WriteLine(helpText[i]);
            }
        }
    }
}
