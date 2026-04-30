using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Entities
{
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; protected set; }
        public string Color { get; protected set; } = GameColors.Reset;
    }
}
