using GameCore.Models.Items.Weapons;
using GameCore.Models.Objects;

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
            CalculateEntrances(field);
            FixRoomWalls(field, _rooms);
            PlaceDoors(field, _rooms);
            PlaceInRoomCenters(field, random, _rooms, Config.AltarsAmount, () => new Altar(), excludeFirst: true);
            PlaceInFarthestRoom(field, random, _rooms, Config.ExitAmount, () => new Exit());
            var data = FloorConfig.Get(floor);
            PlaceItemsByChance(field, random, Config.GoldChance,
                () => new Gold(random.Next(data.FloorGoldMin, data.FloorGoldMax)), _rooms);
            PlaceInRandomRooms(field, random, _rooms, data.KeysAmount,
                () => new Key(), excludeFirst: true);
            PlaceEnemies(field, random, _rooms, data.EnemyCount, (x, y) => CreateEnemy(x, y, floor, random), excludeFirst: true);
            var weaponFactory = GetWeaponForFloor(floor);
            if (weaponFactory != null)
                PlaceInRandomRooms(field, random, _rooms, 1, weaponFactory, excludeFirst: true);
            return (field, _rooms[0].CenterX, _rooms[0].CenterY);
        }
        private Func<GameObject>? GetWeaponForFloor(int floor) => floor switch
        {
            1 => () => new Bow(8),
            2 => () => new Dagger(),
            3 => () => new Sword(),
            4 => () => new Crossbow(5),
            _ => null
        };

        private void PlaceRooms(GameField field, Random random)
        {
            for (int i = 0; i < Config.GenAttempts; i++)
            {
                if (_rooms.Count >= Config.MaxRooms) break;
                int rw = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rh = random.Next(Config.MinRoomSize, Config.MaxRoomSize);
                int rx = random.Next(2, field.Width - rw - 2);
                int ry = random.Next(2, field.Height - rh - 2);
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
            var edges = BuildMST(_rooms);
            edges = AddExtraEdges(_rooms, edges, random, 0.2f);
            foreach (var (a, b) in edges)
                CarveCorridorSafe(field, a, b);
        }

        private void CarveCorridorSafe(GameField field, Room a, Room b)
        {
            if (TryFindPath(field, a, b, strict: true)) return;

            TryFindPath(field, a, b, strict: false);
        }

        private bool TryFindPath(GameField field, Room a, Room b, bool strict)
        {
            var queue = new Queue<(int x, int y)>();
            var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

            var start = (x: a.CenterX, y: a.CenterY);
            var target = (x: b.CenterX, y: b.CenterY);

            queue.Enqueue(start);
            cameFrom[start] = start;

            bool found = false;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.x == target.x && current.y == target.y)
                {
                    found = true;
                    break;
                }

                var neighbors = new (int x, int y)[]
                {
                    (current.x + 1, current.y), (current.x - 1, current.y),
                    (current.x, current.y + 1), (current.x, current.y - 1)
                };

                foreach (var n in neighbors)
                {
                    if (n.x <= 0 || n.x >= field.Width - 1 || n.y <= 0 || n.y >= field.Height - 1)
                        continue;

                    if (cameFrom.ContainsKey(n))
                        continue;

                    bool isObstacle = false;

                    if (strict)
                    {
                        foreach (var room in _rooms)
                        {
                            if (room == a || room == b) continue;
                            if (n.x >= room.X - 1 && n.x <= room.X + room.W &&
                                n.y >= room.Y - 1 && n.y <= room.Y + room.H)
                            {
                                isObstacle = true;
                                break;
                            }
                        }
                    }

                    if (!isObstacle)
                    {
                        queue.Enqueue(n);
                        cameFrom[n] = current;
                    }
                }
            }
            if (found)
            {
                var current = target;
                while (current.x != start.x || current.y != start.y)
                {
                    SetFloor(field, current.x, current.y);
                    current = cameFrom[current];
                }
                return true;
            }

            return false;
        }
        private void CalculateEntrances(GameField field)
        {
            foreach (var room in _rooms)
            {
                for (int x = room.X; x < room.X + room.W; x++)
                    if (field[x, room.Y - 1] is Floor &&
                        room.Y - 2 >= 0 && field[x, room.Y - 2] is Floor)
                    { room.Entrances.Add((x, room.Y - 1)); break; }

                for (int x = room.X; x < room.X + room.W; x++)
                    if (field[x, room.Y + room.H] is Floor &&
                        room.Y + room.H + 1 < field.Height && field[x, room.Y + room.H + 1] is Floor)
                    { room.Entrances.Add((x, room.Y + room.H)); break; }

                for (int y = room.Y; y < room.Y + room.H; y++)
                    if (field[room.X - 1, y] is Floor &&
                        room.X - 2 >= 0 && field[room.X - 2, y] is Floor)
                    { room.Entrances.Add((room.X - 1, y)); break; }

                for (int y = room.Y; y < room.Y + room.H; y++)
                    if (field[room.X + room.W, y] is Floor &&
                        room.X + room.W + 1 < field.Width && field[room.X + room.W + 1, y] is Floor)
                    { room.Entrances.Add((room.X + room.W, y)); break; }
            }
        }

        private void SetFloor(GameField field, int x, int y)
        {
            if (x > 0 && x < field.Width - 1 && y > 0 && y < field.Height - 1)
            {
                if (!(field[x, y] is Floor))
                    field[x, y] = new Floor();
            }
        }
        protected void FixRoomWalls(GameField field, List<Room> rooms)
        {
            foreach (var room in rooms)
                for (int y = room.Y - 1; y <= room.Y + room.H; y++)
                    for (int x = room.X - 1; x <= room.X + room.W; x++)
                    {
                        if (x >= room.X && x < room.X + room.W &&
                            y >= room.Y && y < room.Y + room.H) continue;

                        if (room.Entrances.Contains((x, y))) continue;

                        field[x, y] = new Wall();
                    }
        }
    }
}
