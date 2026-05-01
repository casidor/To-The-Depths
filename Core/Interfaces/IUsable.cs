using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Interfaces
{
    public interface IUsable
    {
        UseResult Use(Player player, GameField field);
    }
}
