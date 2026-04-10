using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Void : GameObject
    {
        public Void()
        {
            Symbol = GameSymbols.Empty;
            IsPassable = false;
        }
        public override void Interact(Player player, GameField field, int x, int y)
        {
        }
    }
}
