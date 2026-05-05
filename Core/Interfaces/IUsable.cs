using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Interfaces
{
    public interface IUsable
    {
        UseResult Use(Player player, GameField field);
    }
}
