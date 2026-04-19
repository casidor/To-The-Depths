using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            int checksum = player.HP + player.MaxHP + player.GoldCollected + player.KeysCollected + player.CurrentFloor + seed;
            string data = $"{player.HP},{player.MaxHP},{player.GoldCollected},{player.KeysCollected},{player.CurrentFloor},{seed},{checksum}";
            File.WriteAllText(SaveFile, data);
        }
        public static (SaveData? data, SaveResult result) Load()
        {
            if (!File.Exists(SaveFile)) return (null, SaveResult.NotFound);
            try
            {
                string[] parts = File.ReadAllText(SaveFile).Split(',');
                if (parts.Length != 7) return (null, SaveResult.Corrupted);
                int hp = int.Parse(parts[0]);
                int maxHp = int.Parse(parts[1]);
                int gold = int.Parse(parts[2]);
                int keys = int.Parse(parts[3]);
                int floor = int.Parse(parts[4]);
                int seed = int.Parse(parts[5]);
                int checksum = int.Parse(parts[6]);
                if (checksum != hp + maxHp + gold + keys + floor + seed) return (null, SaveResult.Unverified);
                return (new SaveData(hp, maxHp, gold, keys, floor, seed), SaveResult.Success);
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
