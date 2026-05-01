using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items.Weapons
{
    public class Dagger : MeleeWeapon
    {
        public Dagger()
        {
            Name = "Dagger";
            Description = "A small but fast blade.";
            Damage = 15;
        }
    }
}
