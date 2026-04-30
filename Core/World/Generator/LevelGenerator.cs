using GameCore.Models.Entities;
using GameCore.Models.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Void = GameCore.Models.Objects.Void;

namespace GameCore.World.Generator
{
    public abstract class LevelGenerator
    {
        public abstract (GameField field, int x, int y) Generate(int w, int h, Random random, int floor);

        protected void AddWalls(GameField field) // Add walls around floors
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
        public static void FillEmptySpaces(GameField field)// Fill the field with empty spaces
        {
            for (int y = 0; y < field.Height; y++)
                for (int x = 0; x < field.Width; x++)
                    field[x, y] = new Void();
        }

        protected void PlaceItemsByChance(GameField field, Random random, int chance, Func<GameObject> factory, List<Room> rooms)// Place items in rooms based on a chance percentage
        {
            foreach (var room in rooms)
                for (int y = room.Y; y < room.Y + room.H; y++)
                    for (int x = room.X; x < room.X + room.W; x++)
                        if (field[x, y] is Floor && random.Next(1, 101) <= chance)
                            field[x, y] = factory();
        }

        protected void PlaceInRandomRooms(GameField field, Random random, List<Room> rooms, int amount, Func<GameObject> factory, bool excludeFirst = false)// Place items in random rooms, optionally excluding the first room
        {
            int startRoom = excludeFirst ? 1 : 0;
            for (int i = 0; i < amount; i++)
            {
                int roomIndex = random.Next(startRoom, rooms.Count);
                PlaceItemInRoom(field, random, rooms[roomIndex], factory);
            }
        }

        protected void PlaceInRoomCenters(GameField field, Random random, List<Room> rooms, int amount, Func<GameObject> factory, bool excludeFirst = false)// Place items in the centers of random rooms, optionally excluding the first room
        {
            var used = new HashSet<int>();
            int startRoom = excludeFirst ? 1 : 0;
            for (int i = 0; i < amount && used.Count < rooms.Count - startRoom; i++)
            {
                int roomIndex;
                do { roomIndex = random.Next(startRoom, rooms.Count); }
                while (used.Contains(roomIndex));
                used.Add(roomIndex);
                var room = rooms[roomIndex];
                if (field[room.CenterX, room.CenterY] is Floor)
                    field[room.CenterX, room.CenterY] = factory();
                else
                    PlaceItemInRoom(field, random, room, factory);
            }
        }

        protected void PlaceInFarthestRoom(GameField field, Random random, List<Room> rooms, int amount, Func<GameObject> factory)// Place items in the farthest room from the first room
        {
            var start = rooms[0];
            var farthest = rooms.Skip(1)
                .OrderByDescending(r => Math.Sqrt(Math.Pow(r.CenterX - start.CenterX, 2) + Math.Pow(r.CenterY - start.CenterY, 2)))
                .First();
            for (int i = 0; i < amount; i++)
                PlaceItemInRoom(field, random, farthest, factory);
        }

        protected void PlaceItemInRoom(GameField field, Random random, Room room, Func<GameObject> factory)// Place an item in a specific room, trying random positions within the room
        {
            for (int attempt = 0; attempt < 100; attempt++)
            {
                int x = random.Next(room.X, room.X + room.W);
                int y = random.Next(room.Y, room.Y + room.H);
                if (field[x, y] is Floor) { field[x, y] = factory(); return; }
            }
        }

        protected void PlaceDoors(GameField field, List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                for (int x = room.X; x < room.X + room.W; x++)
                {
                    if (field[x, room.Y - 1] is Floor) field[x, room.Y - 1] = new Door();
                    if (field[x, room.Y + room.H] is Floor) field[x, room.Y + room.H] = new Door();
                }
                for (int y = room.Y; y < room.Y + room.H; y++)
                {
                    if (field[room.X - 1, y] is Floor) field[room.X - 1, y] = new Door();
                    if (field[room.X + room.W, y] is Floor) field[room.X + room.W, y] = new Door();
                }
            }
        }
        protected void PlaceEnemies(GameField field, Random random, List<Room> rooms, int amount, bool excludeFirst = false)
        {
            int startRoom = excludeFirst ? 1 : 0;
            for (int i = 0; i < amount; i++)
            {
                var room = rooms[random.Next(startRoom, rooms.Count)];
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    int x = random.Next(room.X, room.X + room.W);
                    int y = random.Next(room.Y, room.Y + room.H);
                    if (field[x, y] is Floor && !field.HasEntity(x, y))
                    {
                        var enemy = new Enemy(x, y);
                        field.SetEntity(x, y, enemy);
                        break;
                    }
                }
            }
        }
    }
}
