using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public class Key : GameObject
    {
        public Key()
        {
            Symbol = GameSymbols.Key;
            IsPassable = true;
            Color = GameColors.Key;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
                player.AddKey();
                field[x, y] = new Floor();
                return InteractionResult.None;
        }
    }
}
