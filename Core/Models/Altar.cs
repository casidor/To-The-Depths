using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Altar : GameObject
    {
        int Charges { get; set; } = Config.AltarCharges;
        public Altar()
        {
            Symbol = GameSymbols.Altar;
            IsPassable = true;
            Color = GameColors.Altar;
        }
        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            if (Charges > 0 && player.SpendGold(Config.HealCost))
            {
                player.Heal(Config.AltarHeal);
                Charges--;
            }
            return InteractionResult.Altar;
        }
    }
}
