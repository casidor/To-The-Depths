using GameCore.Models.Items.Weapons;
using GameCore.Models.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Void = GameCore.Models.Objects.Void;

namespace GameCore.World.Generator
{
    public class RoomCorridorGenerator : LevelGenerator
    {
        private List<Room> _rooms = new();

        public override (GameField field, int x, int y) Generate(int w, int h, Random random, int floor)
        {
            _rooms = new();
            var field = new GameField(w, h);
            FillEmptySpaces(field);
            PlaceRooms(field, random);
            ConnectRooms(field, random);
            AddWalls(field);
            PlaceInRoomCenters(field, random, _rooms, Config.AltarsAmount, () => new Altar(), excludeFirst: true);
            PlaceInFarthestRoom(field, random, _rooms, Config.ExitAmount, () => new Exit());
            PlaceInRandomRooms(field, random, _rooms, Config.KeysAmount, () => new Key(), excludeFirst: true);
            PlaceEnemies(field, random, _rooms, Config.EnemiesAmount, excludeFirst: true);
            PlaceItemsByChance(field, random, Config.GoldChance, () => new Gold(), _rooms);
            PlaceDoors(field, _rooms);
            PlaceInRandomRooms(field, random, _rooms, 1, () => new Dagger(), excludeFirst: true);
            PlaceInRandomRooms(field, random, _rooms, 1, () => new Bow(10), excludeFirst: true);
            return (field, _rooms[0].CenterX, _rooms[0].CenterY);
        }

        private void PlaceRooms(GameField field, Random random)
        {
            for (int i = 0; i < Config.GenAttempts; i++)
            {
                if (_rooms.Count >= Config.MaxRooms) break;
                int rw = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rh = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rx = random.Next(1, field.Width - rw - 1);
                int ry = random.Next(1, field.Height - rh - 1);
                var newRoom = new Room(rx, ry, rw, rh);
                if (_rooms.Any(r => r.Overlaps(newRoom))) continue;
                for (int y = ry; y < ry + rh; y++)
                    for (int x = rx; x < rx + rw; x++)
                        field[x, y] = new Floor();
                _rooms.Add(newRoom);
            }
        }

        private void ConnectRooms(GameField field, Random random)
        {
            for (int i = 0; i < _rooms.Count - 1; i++)
            {
                int x1 = _rooms[i].CenterX, y1 = _rooms[i].CenterY;
                int x2 = _rooms[i + 1].CenterX, y2 = _rooms[i + 1].CenterY;
                int curX = x1, curY = y1;
                if (random.Next(2) == 0)
                {
                    while (curX != x2) { curX += Math.Sign(x2 - curX); field[curX, curY] = new Floor(); }
                    while (curY != y2) { curY += Math.Sign(y2 - curY); field[curX, curY] = new Floor(); }
                }
                else
                {
                    while (curY != y2) { curY += Math.Sign(y2 - curY); field[curX, curY] = new Floor(); }
                    while (curX != x2) { curX += Math.Sign(x2 - curX); field[curX, curY] = new Floor(); }
                }
            }
        }
    }
}
