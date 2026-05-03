using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public class Gold : GameObject
    {
        public int Amount { get; }

        public Gold() : this(Config.GoldAmount) { }

        public Gold(int amount)
        {
            Amount = amount;
            Symbol = GameSymbols.Gold;
            IsPassable = true;
            Color = GameColors.Gold;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            player.AddGold(Amount);
            field.Log.Add(GameEventType.GoldCollected, $"Gold: +{Amount}", '♦', x, y, Amount, color: LogColor.Good);
            field[x, y] = new Floor();
            return InteractionResult.None;
        }
    }
}
