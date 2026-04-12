using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Enemy : GameObject
    {
        public Enemy()
        {
            Symbol = GameSymbols.Enemy;
            IsPassable = true;
            Color = GameColors.Enemy;
        }
        public override void Interact(Player player, GameField field, int x, int y)
        {
            player.TakeDamage(Config.EnemyDamage);
            player.SpendGold(Config.GoldStolen);
            field[x, y] = new Floor();
        }
    }
}
