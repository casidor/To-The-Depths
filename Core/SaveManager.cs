using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public record SaveData(
        int HP,
        int MaxHP,
        int Gold,
        int Keys,
        int Floor,
        int Seed
    );
    public class SaveManager
    {
        public const string SaveFile = "checkpoint.txt";
        public static void Save(Player player, int seed)
        {
            string data = $"{player.HP},{player.MaxHP},{player.GoldCollected},{player.KeysCollected},{player.CurrentFloor},{seed}";
            File.WriteAllText(SaveFile, data);
        }
        public static SaveData? Load()
        {
            if (!File.Exists(SaveFile)) return null;
            try
            {
                string[] parts = File.ReadAllText(SaveFile).Split(',');
                if (parts.Length != 6) return null;
                int hp = int.Parse(parts[0]);
                int maxHp = int.Parse(parts[1]);
                int gold = int.Parse(parts[2]);
                int keys = int.Parse(parts[3]);
                int floor = int.Parse(parts[4]);
                int seed = int.Parse(parts[5]);
                return new SaveData(hp, maxHp, gold, keys, floor, seed);
            }
            catch
            {
                return null;
            }
        }
        public static void DeleteSave()
        {
            if (File.Exists(SaveFile)) File.Delete(SaveFile);
        }
    }
}
