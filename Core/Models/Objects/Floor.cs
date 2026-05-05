using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Floor : GameObject
    {
        public Floor()
        {
            IsPassable = true;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            return InteractionResult.None;
        }
    }
}
