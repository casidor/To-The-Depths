using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Gold : GameObject
    {
        public Gold()
        {
            Symbol = GameSymbols.Gold;
            IsPassable = true;
        }

        public override void Interact(Player player, GameField field, int x, int y)
        {
            player.AddGold(Config.GoldAmount);
            field[x, y] = new Floor();
        }
    }
}
