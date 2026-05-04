using GameCore.Models.Entities;
using GameCore.Models.Items;
using GameCore.Models.Items.Weapons;
using System.Text.Json;

namespace GameCore
{
    public record SavedItemData(
        string Type,
        int UpgradeLevel = 0,
        int Ammo = 0,
        int MaxAmmo = 0,
        int AmmoUpgradeLevel = 0
    );

    public record SavedInventoryData(
        SavedItemData? Melee,
        SavedItemData?[] Hotbar,
        int ActiveSlot
    );

    public record SaveData(
        int HP,
        int MaxHP,
        int Gold,
        int Keys,
        int Floor,
        int KeysRequired,
        int Seed,
        SavedInventoryData Inventory,
        int Checksum
    );

    public class SaveManager
    {
        public const string SaveFile = "checkpoint.json";

        private static int ComputeChecksum(int hp, int maxHp, int gold, int keys, int floor, int keysRequired, int seed)
        {
            unchecked 
            {
                int hash = 17;
                hash = hash * 31 + hp;
                hash = hash * 31 + maxHp;
                hash = hash * 31 + gold;
                hash = hash * 31 + keys;
                hash = hash * 31 + floor;
                hash = hash * 31 + keysRequired;
                hash = hash * 31 + seed;
                return hash;
            }
        }

        // Item -> SavedItemData
        private static SavedItemData? ItemToData(Item? item) => item switch
        {
            null => null,
            RangedWeapon rw => new SavedItemData(item.GetType().Name, rw.UpgradeLevel, rw.Ammo, rw.MaxAmmo, rw.AmmoUpgradeLevel),
            Weapon w => new SavedItemData(item.GetType().Name, w.UpgradeLevel),
            _ => null
        };

        // SavedItemData -> Item
        internal static Item? DataToItem(SavedItemData? data)
        {
            if (data == null) return null;

            Item? item = data.Type switch
            {
                nameof(Dagger) => new Dagger(),
                nameof(Sword) => new Sword(),
                nameof(Bow) => new Bow(data.Ammo),
                nameof(Crossbow) => new Crossbow(data.Ammo),
                _ => null
            };

            if (item is RangedWeapon rw)
            {
                rw.RestoreState(data.UpgradeLevel);
                rw.RestoreAmmoState(data.Ammo, data.MaxAmmo, data.AmmoUpgradeLevel);
            }
            else if (item is Weapon w)
            {
                w.RestoreState(data.UpgradeLevel);
            }

            return item;
        }

        public static void Save(Player player, int seed)
        {
            int checksum = ComputeChecksum(
                player.HP, player.MaxHP, player.GoldCollected,
                player.KeysCollected, player.CurrentFloor, player.KeysRequired, seed);

            var inv = player.Inventory;
            var inventoryData = new SavedInventoryData(
                Melee: ItemToData(inv.EquippedMelee),
                Hotbar: inv.Hotbar.Select(ItemToData).ToArray(),
                ActiveSlot: inv.ActiveSlot
            );

            var data = new SaveData(
                player.HP, player.MaxHP, player.GoldCollected,
                player.KeysCollected, player.CurrentFloor, player.KeysRequired,
                seed, inventoryData, checksum);

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFile, json);
        }

        public static (SaveData? data, SaveResult result) Load()
        {
            if (!File.Exists(SaveFile)) return (null, SaveResult.NotFound);
            try
            {
                var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SaveFile));
                if (data == null) return (null, SaveResult.Corrupted);

                int expected = ComputeChecksum(
                    data.HP, data.MaxHP, data.Gold,
                    data.Keys, data.Floor, data.KeysRequired, data.Seed);

                if (data.Checksum != expected) return (null, SaveResult.Unverified);

                return (data, SaveResult.Success);
            }
            catch
            {
                return (null, SaveResult.Corrupted);
            }
        }
        public static void DeleteSave()
        {
            if (File.Exists(SaveFile)) File.Delete(SaveFile);
        }
    }
}