using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameCore.World
{
    public class EnemyAI
    {
        private int[,]? map;
        private readonly Random random;
        public EnemyAI(Random random)
        {
            this.random = random;
        }
        public void BuildDistanceMap(GameField field, Player player)
        {
            map = new int[field.Width, field.Height];
            for (int y = 0; y < field.Height; y++)
            {
                for (int x = 0; x < field.Width; x++)
                {
                    map[x, y] = -1;
                }
            }
            map[player.X, player.Y] = 0;
            Queue<(int x, int y)> queue = new();
            queue.Enqueue((player.X, player.Y));
            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                int currentDistance = map[x, y];
                foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < field.Width && ny >= 0 && ny < field.Height)
                    {
                        if ((field[nx, ny] is Floor || field[nx, ny] is Enemy) && map[nx, ny] == -1)
                        {
                            map[nx, ny] = currentDistance + 1;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }
        }
        private void MoveEnemy(GameField field, int fromX, int fromY, int toX, int toY)
        {
            if (field[toX, toY] is Floor)
            {
                var enemy = field[fromX, fromY];
                field[toX, toY] = enemy;
                field[fromX, fromY] = new Floor();
            }
        }
        private bool Chase(int x, int y, GameField field, Player player)
        {
            if (map == null) return false;
            int bestDistance = map[x, y];
            (int dx, int dy) bestMove = (0, 0);
            foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < field.Width && ny >= 0 && ny < field.Height)
                {
                    if (field[nx, ny] is Floor && map[nx, ny] != -1 && map[nx, ny] < bestDistance)
                    {
                        bestDistance = map[nx, ny];
                        bestMove = (dx, dy);
                    }
                }
            }
            if (bestMove != (0, 0))
            {
                int tx = x + bestMove.dx;
                int ty = y + bestMove.dy;
                if (tx == player.X && ty == player.Y)
                {
                    if (field[x, y] is Enemy enemy)
                    {
                        enemy.Attack(player);
                    }
                    return true;
                }
                else
                {
                    MoveEnemy(field, x, y, tx, ty);
                    return false;
                }
            }
            return false;
        }
        private void Wander(int x, int y, GameField field)
        {
            var directions = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            for (int i = 0; i < directions.Length; i++)
            {
                var (dx, dy) = directions[random.Next(directions.Length)];
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < field.Width && ny >= 0 && ny < field.Height)
                {
                    if (field[nx, ny] is Floor)
                    {
                        MoveEnemy(field, x, y, nx, ny);
                        break;
                    }
                }
            }
        }
        public InteractionResult UpdateEnemies(GameField field, Player player)
        {
            var enemies = new List<(int x, int y)>();
            for (int y = 0; y < field.Height; y++)
                for (int x = 0; x < field.Width; x++)
                    if (field[x, y] is Enemy)
                        enemies.Add((x, y));
            bool attacked = false;
            foreach (var (x, y) in enemies)
            {
                if (field[x, y] is not Enemy) continue;
                if (map[x, y] != -1 && map[x, y] <= Config.AggroRange)
                {
                    attacked |= Chase(x, y, field, player);
                }
                else
                    Wander(x, y, field);
            }
            return attacked ? InteractionResult.PlayerAttacked : InteractionResult.None;
        }
    }
}
