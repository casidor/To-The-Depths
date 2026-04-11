using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Exit : GameObject
    {
        public Exit()
        {
            Symbol = GameSymbols.Exit;
            IsPassable = true;
            Color = GameColors.Exit;
        }

        public override void Interact(Player player, GameField field, int x, int y)
        {
            if (player.KeysCollected == Config.KeysAmount)
            {
                player.Exit();
            }
        }
    }
}
