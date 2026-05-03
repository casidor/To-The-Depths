using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items.Weapons
{
    public class Crossbow : RangedWeapon
    {
        public Crossbow(int ammo) 
        { 
            Name = "Crossbow";
            Description = "Fires bolts at nearby enemies.";
            Damage = 35;
            Range = 8;
            Ammo = ammo;
            MaxAmmo = 5;
        }
    }
}
