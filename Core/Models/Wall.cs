using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Wall : GameObject
    {
        public Wall()
        {
            Symbol = GameSymbols.Wall;
            IsPassable = false;
            Color = GameColors.Wall;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            return InteractionResult.None;
        }
    }
}
