using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public class Floor : GameObject
    {
        public Floor()
        {
            Symbol = GameSymbols.Floor;
            IsPassable = true;
            Color = GameColors.Floor;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            return InteractionResult.None;  
        }
    }
}
