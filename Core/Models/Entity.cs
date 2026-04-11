using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public abstract class Entity
    {
        public string Name { get; set; }
        public int MaxHP { get; set; } = 0;
        public int HP { get; set; } = 0;
        public int Attack { get; set; } = 0;
        public bool IsAlive => HP > 0;
        public void TakeDamage(int damage)
        {
            HP = Math.Max(HP - damage, 0);
        }
    }
}
