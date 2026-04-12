using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class EnemyTile : GameObject
    {
        public EnemyTile()
        {
            Symbol = GameSymbols.Enemy;
            IsPassable = true;
            Color = GameColors.Enemy;
        }
        public override void Interact(Player player, GameField field, int x, int y)
        {
        }
    }
}
