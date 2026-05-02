using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Entities
{
    public class Enemy : Entity
    {
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public int Damage { get; private set; } = Config.EnemyDamage;
        public Enemy(int x, int y)
        {
            X = x; Y = y;
            Symbol = GameSymbols.Enemy;
            Color = GameColors.Enemy;
            MaxHP = Config.EnemyMaxHP;
            HP = MaxHP;
        }
        public InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            HP -= player.Damage;
            if (HP <= 0)
            {
                field.SetEntity(x, y, null);
                field.Log.Add(GameEventType.EnemyKilled, "Enemy slain!", '☠', x, y, amount: player.Damage);
                return InteractionResult.EnemyKilled;
            }
            field.Log.Add(GameEventType.DamageDealt, $"-{player.Damage}", '⚔', x, y, amount: player.Damage);
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
