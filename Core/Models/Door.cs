using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Door : GameObject
    {
        public bool IsOpen { get; private set; } = false;
        public Door()
        {
            Symbol = '+';
            IsPassable = false;
            Color = GameColors.Wall;
        }
        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            Open();
            return InteractionResult.None;
        }
        public void Open()
        {
            IsOpen = true;
            IsPassable = true;
        }
        public void Close()
        {
            IsOpen = false;
            IsPassable = false;
        }
    }
}
