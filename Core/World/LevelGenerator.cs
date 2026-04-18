using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Void = GameCore.Models.Void;

namespace GameCore.World
{
    public class LevelGenerator
    {
        private int[] _roomX = new int[Config.MaxRooms];
        private int[] _roomY = new int[Config.MaxRooms];
        private int[] _roomW = new int[Config.MaxRooms];
        private int[] _roomH = new int[Config.MaxRooms];
        private int RoomCount = 0;
        public (GameField field, int x,int y) Generate (int w, int h, Random random)
        {
            GameField field = new GameField(w, h);
            FillEmptySpaces(field);
            PlaceRooms(field, random);
            ConnectRooms(field, random);
            AddWalls(field);
            PlaceInRoomCenters(field, random, Config.AltarsAmount, () => new Altar(), excludeFirst: true);
            PlaceInFarthestRoom(field, random, Config.ExitAmount, () => new Exit());
            PlaceInRandomRooms(field, random, Config.KeysAmount, () => new Key(), excludeFirst: true);
            PlaceInRandomRooms(field, random, Config.EnemiesAmount, () => new Enemy(), excludeFirst: true);
            PlaceItemsByChance(field, random, Config.GoldChance, () => new Gold());
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
                        field[x, y] = new Void();
                    }
                }
            }
        }
        public void PlaceRooms(GameField field, Random random) // Generate random rooms
        {
            for (int i = 0; i < Config.GenAttempts; i++)
            {
                if (RoomCount >= Config.MaxRooms) break;
                int rw = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rh = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rx = random.Next(1, field.Width - rw - 1);
                int ry = random.Next(1, field.Height - rh - 1);
                if (!RoomsOverlap(rx, ry, rw, rh))
                {
                    for (int y = ry; y < ry + rh; y++)
                    {
                        for (int x = rx; x < rx + rw; x++)
                        {
                            field[x, y] = new Floor();
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
                        field[curX, curY] = new Floor();
                    }
                    while (curY != y2)
                    {
                        curY += Math.Sign(y2 - curY);
                        field[curX, curY] = new Floor();
                    }
                }
                else
                {
                    // Vertical first
                    while (curY != y2)
                    {
                        curY += Math.Sign(y2 - curY);
                        field[curX, curY] = new Floor();
                    }
                    while (curX != x2)
                    {
                        curX += Math.Sign(x2 - curX);
                        field[curX, curY] = new Floor();
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
                    if (field[x, y] is Floor)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (field[x + dx, y + dy] is Void)
                                {
                                    field[x + dx, y + dy] = new Wall();
                                }
                            }
                        }
                    }
                }
            }
        }
        private void PlaceInFarthestRoom(GameField field, Random random,int amount, Func<GameObject> factory) // Place an item in the farthest room from the starting room
        {
            int startX = _roomX[0] + _roomW[0] / 2;
            int startY = _roomY[0] + _roomH[0] / 2;
            double maxDistance = -1;
            int farthestRoom = 0;
            for (int i = 1; i < RoomCount; i++)
            {
                int centerX = _roomX[i] + _roomW[i] / 2;
                int centerY = _roomY[i] + _roomH[i] / 2;
                double dist = Math.Sqrt(Math.Pow(centerX - startX, 2) + Math.Pow(centerY - startY, 2));

                if (dist > maxDistance)
                {
                    maxDistance = dist;
                    farthestRoom = i;
                }
            }

            for (int i = 0; i < amount; i++)
            {
                PlaceItemInRoom(field, random, farthestRoom, factory);
            }
        }
        private void PlaceInRoomCenters(GameField field, Random random, int amount, Func<GameObject> factory, bool excludeFirst = false)
        {
            HashSet<int> usedRooms = new HashSet<int>();
            int startRoom = excludeFirst ? 1 : 0;

            for (int i = 0; i < amount && usedRooms.Count < RoomCount - startRoom; i++)
            {
                int roomIndex;
                do
                {
                    roomIndex = random.Next(startRoom, RoomCount);
                } while (usedRooms.Contains(roomIndex));

                usedRooms.Add(roomIndex);
                int centerX = _roomX[roomIndex] + _roomW[roomIndex] / 2;
                int centerY = _roomY[roomIndex] + _roomH[roomIndex] / 2;

                if (field[centerX, centerY] is Floor)
                {
                    field[centerX, centerY] = factory();
                }
                else
                {
                    PlaceItemInRoom(field, random, roomIndex, factory);
                }
            }
        }
        private void PlaceInRandomRooms(GameField field, Random random, int amount, Func<GameObject> factory, bool excludeFirst = false)
        {
            int startRoom = excludeFirst ? 1 : 0;

            for (int i = 0; i < amount; i++)
            {
                int roomIndex = random.Next(startRoom, RoomCount);
                PlaceItemInRoom(field, random, roomIndex, factory);
            }
        }
        private void PlaceItemInRoom(GameField field, Random random, int roomIndex, Func<GameObject> factory)
        {
            for (int attempt = 0; attempt < 100; attempt++)
            {
                int x = random.Next(_roomX[roomIndex], _roomX[roomIndex] + _roomW[roomIndex]);
                int y = random.Next(_roomY[roomIndex], _roomY[roomIndex] + _roomH[roomIndex]);

                if (field[x, y] is Floor)
                {
                    field[x, y] = factory();
                    return;
                }
            }
        }
        public void PlaceItemsByChance(GameField field, Random random, int chance, Func<GameObject> factory) // Place items based on a chance
        {
            for (int i = 0; i < RoomCount; i++)
            {
                for (int y = _roomY[i]; y < _roomY[i] + _roomH[i]; y++)
                {
                    for (int x = _roomX[i]; x < _roomX[i] + _roomW[i]; x++)
                    {
                        if (field[x, y] is Floor)
                        {
                            int roll = random.Next(1, 101);
                            if (roll <= chance)
                            {
                                field[x, y] = factory();
                            }
                        }
                    }
                }
            }
        }
    }
}
