using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Exit : GameObject
    {
        public Exit()
        {
            IsPassable = true;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            if (player.KeysCollected >= player.KeysRequired) player.Exit();
            return InteractionResult.ExitReached;
        }
    }
}
