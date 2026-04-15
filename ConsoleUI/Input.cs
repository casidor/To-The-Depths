using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ConsoleUI
{
    internal class Input
    {
        public (bool, InteractionResult) ProcessInput(Player player, GameField field)
        {
            var key = Console.ReadKey(true).Key;
            while (Console.KeyAvailable) Console.ReadKey(true);
            InteractionResult moveResult = InteractionResult.None;
            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    moveResult = player.Move(0, -1, field);
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    moveResult = player.Move(0, 1, field);
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    moveResult = player.Move(-1, 0, field);
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    moveResult = player.Move(1, 0, field);
                    break;
                case ConsoleKey.Escape:
                    return (false, InteractionResult.None);
            }
            return (true, moveResult);
        }
        public GameState ProcessMenuInput(Renderer renderer)
        {
            int selected = 0;
            int count = GameTexts.Options.Length;
            renderer.RenderMainMenu(selected);
            while (true)
            {
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
                        if (selected == 0) return GameState.Generating;
                        if (selected == 1) return GameState.Help;
                        if (selected == 2) return GameState.Exit;
                        break;
                }
            }
        }
        public GameState ProcessAltarInput(Renderer renderer, Player player, Altar altar)
        {
            int selected = 0;
            int count = 2;
            renderer.RenderAltarMenu(selected);
            while (true)
            {
                renderer.RenderAltarMenu(selected);

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + count) % count;
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % count;
                        break;
                    case ConsoleKey.Enter:
                        if (selected == 0)
                        {
                            var (success, message) = altar.TryHeal(player);
                            renderer.RenderPopup(message);
                            Thread.Sleep(1000);
                        }
                        return GameState.Running;
                    case ConsoleKey.Escape:
                        return GameState.Running;
                }
            }
        }
    }
}
