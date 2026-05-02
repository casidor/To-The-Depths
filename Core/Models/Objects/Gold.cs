using GameCore.Models.Entities;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Objects
{
    public class Gold : GameObject
    {
        public Gold()
        {
            Symbol = GameSymbols.Gold;
            IsPassable = true;
            Color = GameColors.Gold;
        }

        public override InteractionResult Interact(Player player, GameField field, int x, int y)
        {
            player.AddGold(Config.GoldAmount);
            field.Log.Add(GameEventType.GoldCollected, $"Gold: +{Config.GoldAmount}", '♦', x, y, Config.GoldAmount, color: LogColor.Good);
            field[x, y] = new Floor();
            return InteractionResult.None;
        }
    }
}
