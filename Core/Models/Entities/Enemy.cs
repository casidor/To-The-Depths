using GameCore.Models.Objects;
using GameCore.World;

namespace GameCore.Models.Entities
{
    public class Enemy : Entity
    {
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; }
        public int Damage { get; protected set; } = Config.EnemyDamage;
        private int _goldDrop = 0;

        public Enemy(int x, int y)
        {
            X = x; Y = y;
            MaxHP = Config.EnemyMaxHP;
            HP = MaxHP;
        }

        public Enemy(int x, int y, int floor, Random random) : this(x, y)
        {
            var data = FloorConfig.Get(floor);
            MaxHP = (int)(Config.EnemyMaxHP * data.EnemyHPMultiplier);
            HP = MaxHP;
            _goldDrop = random.Next(data.GoldDropMin, data.GoldDropMax + 1);
        }
        public InteractionResult Interact(Player player, GameField field, int x, int y)
            => TakeDamage(player.Damage, player, field, x, y);

        public InteractionResult TakeDamage(int damage, Player player, GameField field, int x, int y)
        {
            HP -= damage;
            if (HP <= 0)
            {
                field.SetEntity(x, y, null);
                if (_goldDrop > 0 && field[x, y] is Floor)
                    field[x, y] = new Gold(_goldDrop);
                field.Log.Add(GameEventType.EnemyKilled, "Enemy slain!", '☠', x, y);
                return InteractionResult.EnemyKilled;
            }
            field.Log.Add(GameEventType.DamageDealt, $"-{damage}", '⚔', x, y);
            return InteractionResult.EnemyAttacked;
        }

        public InteractionResult Attack(Player player, GameField field)
        {
            player.TakeDamage(Damage);
            field.Log.Add(GameEventType.DamageTaken, $"-{Damage}", '♥', player.X, player.Y, amount: Damage);
            return InteractionResult.PlayerAttacked;
        }
    }
}
