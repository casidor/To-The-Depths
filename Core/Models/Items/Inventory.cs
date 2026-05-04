using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public class Inventory
    {
        public const int HotbarSize = 4;

        public MeleeWeapon? EquippedMelee { get; set; }
        public Item?[] Hotbar { get; } = new Item?[HotbarSize];
        public int ActiveSlot { get; private set; } = 0;

        public Item? ActiveItem => Hotbar[ActiveSlot];

        public string MeleeStatLine => EquippedMelee?.StatLine ?? $"Deals {Config.PlayerDamage} dmg";
        public string ActiveItemStatLine => (ActiveItem as Weapon)?.StatLine ?? "---";

        public bool IsHotbarFull => Array.TrueForAll(Hotbar, s => s != null);

        public void SwitchSlot(int index)
        {
            if (index >= 0 && index < HotbarSize)
                ActiveSlot = index;
        }

        public bool TryAddToHotbar(Item item)
        {
            for (int i = 0; i < Hotbar.Length; i++)
            {
                if (Hotbar[i] == null)
                {
                    Hotbar[i] = item;
                    return true;
                }
            }
            return false;
        }

        public void RemoveFromHotbar(Item item)
        {
            for (int i = 0; i < Hotbar.Length; i++)
                if (Hotbar[i] == item)
                    Hotbar[i] = null;
        }
        public void RestoreFromSave(SavedInventoryData data)
        {
            EquippedMelee = SaveManager.DataToItem(data.Melee) as MeleeWeapon;

            for (int i = 0; i < HotbarSize && i < data.Hotbar.Length; i++)
                Hotbar[i] = SaveManager.DataToItem(data.Hotbar[i]);

            SwitchSlot(data.ActiveSlot);
        }
    }
}
