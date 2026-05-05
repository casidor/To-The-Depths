using GameCore.Models.Entities;
using GameCore.World;

namespace GameCore.Models.Objects
{
    public class Altar : GameObject
    {
        int Charges { get; set; } = Config.AltarCharges;
        public Altar()
        {
            IsPassable = true;
        }
        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            return InteractionResult.Altar;
        }
        public (bool Success, string Message) TryHeal(Player player)
        {
            if (Charges <= 0)
                return (false, "Altar magic has faded away.");
            if (player.HP >= player.MaxHP)
                return (false, "Your health is already full!");
            if (player.GoldCollected < Config.HealCost)
                return (false, $"Not enough gold! Need {Config.HealCost}.");

            player.SpendGold(Config.HealCost);
            player.Heal(Config.AltarHeal);
            Charges--;
            return (true, "You have been healed!");
        }
    }
}
