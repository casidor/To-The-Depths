using GameCore.World;

namespace GameCore.Models.Entities.Enemies
{
    public class RangedEnemy : Enemy
    {
        public int Range { get; } = Config.RangedEnemyRange;

        public RangedEnemy(int x, int y, int floor, Random random) : base(x, y, floor, random)
        {
            MaxHP = (int)(Config.RangedMaxHP * FloorConfig.Get(floor).EnemyHPMultiplier);
            HP = MaxHP;
            Damage = Config.RangedDamage;
        }
        public void RangedAttack(Player player, GameField field)
        {
            int dist = Math.Abs(X - player.X) + Math.Abs(Y - player.Y);
            if (dist > Range) return;
            if (field.Fov[X, Y] != ExplorationState.Visible) return;

            player.TakeDamage(Damage);
            field.Log.Add(GameEventType.DamageTaken, $"-{Damage}", '♥',
                player.X, player.Y, amount: Damage);
        }
    }
}
