using GameCore.Models.Entities;
using GameCore.Models.Entities.Enemies;
using GameCore.Models.Objects;
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
                        if ((field[nx, ny].IsPassable || field[nx, ny] is Door) && map[nx, ny] == -1)
                        {
                            map[nx, ny] = currentDistance + 1;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }
        }
        private void MoveEnemy(GameField field, Enemy enemy, int toX, int toY)
        {
            if (field[toX, toY] is Door door)
                door.Open();
            if (field[toX, toY].IsPassable)
            {
                field.SetEntity(enemy.X, enemy.Y, null);
                enemy.X = toX;
                enemy.Y = toY;
                field.SetEntity(toX, toY, enemy);
            }
        }
        private bool Chase(Enemy enemy, GameField field, Player player, bool flee = false)
        {
            int bestDistance = map![enemy.X, enemy.Y];
            (int dx, int dy) bestMove = (0, 0);

            foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                int nx = enemy.X + dx;
                int ny = enemy.Y + dy;
                if (nx < 0 || nx >= field.Width || ny < 0 || ny >= field.Height) continue;
                if ((field[nx, ny].IsPassable || field[nx, ny] is Door) &&
                    !field.HasEntity(nx, ny) && map[nx, ny] != -1)
                {
                    bool better = flee ? map[nx, ny] > bestDistance : map[nx, ny] < bestDistance;
                    if (better)
                    {
                        bestDistance = map[nx, ny];
                        bestMove = (dx, dy);
                    }
                }
            }
            if (bestMove == (0, 0)) return false;

            int tx = enemy.X + bestMove.dx;
            int ty = enemy.Y + bestMove.dy;

            if (tx == player.X && ty == player.Y)
            {
                enemy.Attack(player, field);
                return true;
            }
            MoveEnemy(field, enemy, tx, ty);
            return false;
        }
        private void Wander(Enemy enemy, GameField field)
        {
            var directions = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            var (dx, dy) = directions[random.Next(directions.Length)];
            int nx = enemy.X + dx;
            int ny = enemy.Y + dy;
            if (nx >= 0 && nx < field.Width && ny >= 0 && ny < field.Height)
                if ((field[nx, ny].IsPassable || field[nx, ny] is Door) && !field.HasEntity(nx, ny))
                    MoveEnemy(field, enemy, nx, ny);
        }
        private void UpdateRanged(RangedEnemy enemy, GameField field, Player player)
        {
            bool canSee = field.Fov[enemy.X, enemy.Y] == ExplorationState.Visible;
            int dist = Math.Abs(enemy.X - player.X) + Math.Abs(enemy.Y - player.Y);

            if (canSee && dist <= enemy.Range)
            {
                enemy.RangedAttack(player, field);
                Chase(enemy, field, player, flee: true);
            }
            else if (canSee)
                Chase(enemy, field, player);
            else
                Wander(enemy, field);
        }
        public InteractionResult UpdateEnemies(GameField field, Player player)
        {
            var enemies = new List<Enemy>();
            for (int y = 0; y < field.Height; y++)
                for (int x = 0; x < field.Width; x++)
                    if (field.GetEntity(x, y) is Enemy enemy)
                        enemies.Add(enemy);
            bool attacked = false;
            foreach (var enemy in enemies)
            {
                if (enemy is RangedEnemy ranged)
                {
                    UpdateRanged(ranged, field, player);
                    continue;
                }
                bool canSee = field.Fov[enemy.X, enemy.Y] == ExplorationState.Visible;
                bool canHear = map![enemy.X, enemy.Y] != -1 && map[enemy.X, enemy.Y] <= Config.HearRange;
                bool canChase = canSee && map[enemy.X, enemy.Y] <= Config.AggroRange;

                if (canChase || canHear)
                    attacked |= Chase(enemy, field, player);
                else
                    Wander(enemy, field);
            }
            return attacked ? InteractionResult.PlayerAttacked : InteractionResult.None;
        }
    }
}
