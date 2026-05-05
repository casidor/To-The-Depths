using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Wall : GameObject
    {
        public Wall()
        {
            IsPassable = false;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            return InteractionResult.None;
        }
    }
}
