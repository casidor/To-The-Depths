using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    //█
    public class GameSymbols
    {
        public const char Wall = '█';
        public const char Floor = '·';
        public const char Altar = '+';
        public const char Gold = '♦';
        public const char Trap = '^';
        public const char Exit = '▥';
        public const char Player = '☺';
        public const char Fog = '░';
        public const char Empty = ' ';
        public const char Key = '⚷';
        public const char Enemy = '☻';
        public const char Health = '♥';
    }
    public class GameColors
    {
        public const string Reset = "\x1b[0m";
        public const string Wall = "\x1b[38;2;107;114;128m";                          
        public const string Floor = "\x1b[48;2;18;18;30m\x1b[38;2;30;32;48m";         
        public const string Gold = "\x1b[48;2;18;18;30m\x1b[38;2;240;192;64m";        
        public const string Key = "\x1b[48;2;18;18;30m\x1b[38;2;64;170;255m";         
        public const string Player = "\x1b[48;2;18;18;30m\x1b[1m\x1b[38;2;90;255;140m";
        public const string Exit = "\x1b[48;2;18;18;30m\x1b[38;2;255;107;53m";        
        public const string Enemy = "\x1b[48;2;18;18;30m\x1b[38;2;226;75;74m";
        public const string Altar = "\x1b[48;2;40;35;45m\x1b[38;2;220;50;50m";
    }
    public class GameTexts
    {
        public static readonly string[] Title =
        {
          " _____       _   _            ____             _   _         ",
          "|_   _|__   | |_| |__   ___  |  _ \\  ___ _ __ | |_| |__  ___",
          "  | |/ _ \\  | __| '_ \\ / _ \\ | | | |/ _ \\ '_ \\| __| '_ \\/ __|",
          "  | | (_) | | |_| | | |  __/ | |_| |  __/ |_) | |_| | | \\__ \\",
          "  |_|\\___/   \\__|_| |_|\\___| |____/ \\___| .__/ \\__|_| |_|___/",
          "                                         |_|                  "
        };
        public static readonly string[] Options = { "Start Game", "Help", "Exit" };
    }
}
