using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Interfaces
{
    namespace GameCore.Interfaces
    {
        public interface IEquippable
        {
            bool Equip(Player player, GameField field, int x, int y);
        }
    }
}
