using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public class Exit : GameObject
    {
        public Exit()
        {
            Symbol = GameSymbols.Exit;
            IsPassable = true;
            Color = GameColors.Exit;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            if (player.KeysCollected >= player.KeysRequired) player.Exit();
            return InteractionResult.ExitReached;
        }
    }
}
