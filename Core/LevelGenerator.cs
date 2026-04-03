using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public class LevelGenerator
    {
        private const int MaxRooms = 10;
        private const int GenAttempts = 300;
        private const int MinRoomSize = 3;
        private const int MaxRoomSize = 8;
        private const int GoldChance = 10;
        private const int KeysAmount = 3;
        private const int ExitAmount = 1;
        private int[] _roomX = new int[10];
        private int[] _roomY = new int[10];
        private int[] _roomW = new int[10];
        private int[] _roomH = new int[10];
        private int RoomCount = 0;
        public (GameField, int x,int y) Generate (int w, int h, Random random)
        {
            GameField field = new GameField(w, h);
            FillEmptySpaces(field);
            PlaceRooms(field, random);
            ConnectRooms(field, random);
            AddWalls(field);
            PlaceItemsByChance(field, random,GoldChance, GameSymbols.Gold);
            PlaceFixedItems(field, random, KeysAmount, GameSymbols.Key);
            PlaceFixedItems(field, random, ExitAmount, GameSymbols.Exit);
            return (field, _roomX[0] + _roomW[0] / 2, _roomY[0] + _roomH[0] / 2);
        }
        private bool RoomsOverlap(int x, int y, int w, int h)// Check if the new room overlaps with existing rooms
        {
            for (int i = 0; i < RoomCount; i++)
            {
                if (x < _roomX[i] + _roomW[i] && x + w > _roomX[i] &&
                    y < _roomY[i] + _roomH[i] && y + h > _roomY[i])
                {
                    return true;
                }
            }
            return false;
        }
        public static void FillEmptySpaces(GameField field)  // Fill the field with empty spaces
        {
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    {
                        field.SetCell(x, y, GameSymbols.Empty);
                    }
                }
            }
        }
        public void PlaceRooms(GameField field, Random random) // Generate random rooms
        {
            for (int i = 0; i < GenAttempts; i++)
            {
                if (RoomCount >= MaxRooms) break;
                int rw = random.Next(MinRoomSize, MaxRoomSize);
                int rh = random.Next(MinRoomSize, MaxRoomSize);
                int rx = random.Next(1, field.Width - rw - 1);
                int ry = random.Next(1, field.Height - rh - 1);
                if (!RoomsOverlap(rx, ry, rw, rh))
                {
                    for (int y = ry; y < ry + rh; y++)
                    {
                        for (int x = rx; x < rx + rw; x++)
                        {
                            field.SetCell(x, y, GameSymbols.Floor);
                        }
                    }
                    _roomX[RoomCount] = rx;
                    _roomY[RoomCount] = ry;
                    _roomW[RoomCount] = rw;
                    _roomH[RoomCount] = rh;
                    RoomCount++;
                }
            }
        }
        public void ConnectRooms(GameField field, Random random) // Connect rooms with corridors
        {
            for (int i = 0; i < RoomCount - 1; i++)
            {
                int x1 = _roomX[i] + _roomW[i] / 2;
                int y1 = _roomY[i] + _roomH[i] / 2;
                int x2 = _roomX[i + 1] + _roomW[i + 1] / 2;
                int y2 = _roomY[i + 1] + _roomH[i + 1] / 2;
                int curX = x1;
                int curY = y1;
                if (random.Next(2) == 0)
                {
                    // Horizontal first
                    while (curX != x2)
                    {
                        curX += Math.Sign(x2 - curX);
                        field.SetCell(curX, curY, GameSymbols.Floor);
                    }
                    while (curY != y2)
                    {
                        curY += Math.Sign(y2 - curY);
                        field.SetCell(curX, curY, GameSymbols.Floor);
                    }
                }
                else
                {
                    // Vertical first
                    while (curY != y2)
                    {
                        curY += Math.Sign(y2 - curY);
                        field.SetCell(curX, curY, GameSymbols.Floor);
                    }
                    while (curX != x2)
                    {
                        curX += Math.Sign(x2 - curX);
                        field.SetCell(curX, curY, GameSymbols.Floor);
                    }
                }
            }
        }
        public void AddWalls(GameField field) // Add walls around floors
        {
            for (int y = 1; y < field.Height - 1; y++)
            {
                for (int x = 1; x < field.Width - 1; x++)
                {
                    if (field.GetCell(x, y) == GameSymbols.Floor)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (field.GetCell(x + dx, y + dy) == GameSymbols.Empty)
                                {
                                    field.SetCell(x + dx, y + dy, GameSymbols.Wall);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void PlaceItemsByChance (GameField field, Random random,int chance, char symbol)// Place items based on a chance
        {
            for (int i = 0; i < RoomCount; i++)
            {
                for (int y = _roomY[i]; y < _roomY[i] + _roomH[i]; y++)
                {
                    for(int x = _roomX[i]; x < _roomX[i] + _roomW[i]; x++)
                    {
                        if(field.GetCell(x, y) == GameSymbols.Floor)
                        {
                            int roll = random.Next(1,101);
                            if(roll <= chance)
                            {
                                field.SetCell(x, y, symbol);
                            }
                        }
                    }
                }
            }
        }
        public void PlaceFixedItems(GameField field, Random random,int amount, char symbol)// Place a fixed number of items in random locations within rooms
        {
            int placed = 0;
            while (placed < amount)
            {
                int roomIndex = random.Next(RoomCount);
                int x = random.Next(_roomX[roomIndex], _roomX[roomIndex] + _roomW[roomIndex]);
                int y = random.Next(_roomY[roomIndex], _roomY[roomIndex] + _roomH[roomIndex]);

                if (field.GetCell(x, y) == GameSymbols.Floor)
                {
                    field.SetCell(x, y, symbol);
                    placed++;
                }
            }
        }
    }
}
