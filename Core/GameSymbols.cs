using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public class GameSymbols
    {
        //█☺
        public const char Wall = '█';
        public const char Floor = '·';
        public const char Door = '+';
        public const char Gold = '$';
        public const char Trap = '^';
        public const char Exit = '▥';
        public const char Player = '☺';
        public const char Fog = '░';
        public const char Empty = ' ';
        public const char Key = '⚷';
        public static readonly string[] Title =
        {
          " _____       _   _            ____             _   _         ",
          "|_   _|__   | |_| |__   ___  |  _ \\  ___ _ __ | |_| |__  ___",
          "  | |/ _ \\  | __| '_ \\ / _ \\ | | | |/ _ \\ '_ \\| __| '_ \\/ __|",
          "  | | (_) | | |_| | | |  __/ | |_| |  __/ |_) | |_| | | \\__ \\",
          "  |_|\\___/   \\__|_| |_|\\___| |____/ \\___| .__/ \\__|_| |_|___/",
          "                                         |_|                  "
        };
        public static readonly string[] Options = { "Start Game", "Exit" };
    }
}
