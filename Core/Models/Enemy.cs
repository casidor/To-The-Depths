using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Enemy : Entity
    {
        public Enemy(string name, int hp, int attack)
        {
            Name = name;
            HP = hp;
            MaxHP = hp;
            Attack = attack;
        }
    }
}
