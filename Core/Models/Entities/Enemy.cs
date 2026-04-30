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
                return InteractionResult.EnemyKilled;
            }
            return InteractionResult.EnemyAttacked;
        }
        public InteractionResult Attack (Player player)
        {
            player.TakeDamage(Damage);
            return InteractionResult.PlayerAttacked;
        }
    }
}
