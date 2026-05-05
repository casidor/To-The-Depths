using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Door : GameObject
    {
        public bool IsOpen { get; private set; } = false;
        public Door()
        {
            IsPassable = false;
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
