using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Entities.Enemies
{
    public class TankEnemy : Enemy
    {
        public TankEnemy(int x, int y, int floor, Random random) : base(x, y, floor, random)
        {
            MaxHP = (int)(Config.TankMaxHP * FloorConfig.Get(floor).EnemyHPMultiplier);
            HP = MaxHP;
            Damage = Config.TankDamage;
        }
    }
}
