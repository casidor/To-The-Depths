using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Enemy : GameObject
    {
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public int Damage { get; private set; } = Config.EnemyDamage;
        public Enemy()
        {
            Symbol = GameSymbols.Enemy;
            IsPassable = false;
            Color = GameColors.Enemy;
            MaxHP = Config.EnemyMaxHP;
            HP = MaxHP;
        }
        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            HP -= player.Damage;
            if (HP <= 0)
            {
                field[x, y] = new Floor();
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
