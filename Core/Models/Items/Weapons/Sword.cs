using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items.Weapons
{
    public class Sword : MeleeWeapon
    {
        public Sword()
        {
            Name = "Sword";
            Description = "A standard melee weapon.";
            Damage = 25;
        }
    }
}
