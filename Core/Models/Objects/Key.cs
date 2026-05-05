using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Key : GameObject
    {
        public Key()
        {
            IsPassable = true;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            player.AddKey();
            field.Log.Add(GameEventType.KeyCollected, $"Key collected: {player.KeysCollected}/{player.KeysRequired}", '⚷', x, y, color: LogColor.Good);
            field[x, y] = new Floor();
            return InteractionResult.None;
        }
    }
}
