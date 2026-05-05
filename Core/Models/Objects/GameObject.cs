using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public abstract class GameObject
    {
        public bool IsPassable { get; protected set; } = false;
        public virtual string SpriteName => GetType().Name.ToLower();
        public abstract InteractionResult Interact(Player player, GameField field, int x, int y);
    }
}
