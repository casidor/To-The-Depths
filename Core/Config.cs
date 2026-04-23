using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public static class Config
    {
        //Console settings
        public const int ConsoleWidth = 110;
        public const int ConsoleHeight = 30;
        //Field settings
        public const int FieldWidth = 80;
        public const int FieldHeight = 50;
        //Player settings
        public const int PlayerMaxHP = 100;
        //Enemy settings
        public const int EnemyDamage = 15;
        public const int GoldStolen = 10;
        public const int AggroRange = 7;
        //Generation settings
        public const int MaxRooms = 10;
        public const int GenAttempts = 300;
        public const int MinRoomSize = 3;
        public const int MaxRoomSize = 10;
        public const int GoldAmount = 5;
        public const int GoldChance = 15;
        public const int KeysAmount = 3;
        public const int ExitAmount = 1;
        public const int EnemiesAmount = 7;
        public const int AltarsAmount = 2;
        public const int MaxFloor = 5;
        //Tiles settings
        public const int AltarCharges = 2;
        public const int AltarHeal = 20;
        public const int HealCost = 20;
    }
}
