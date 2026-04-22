using CommunityToolkit.Mvvm.ComponentModel;
using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Numerics;

namespace AvaloniaUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly int currentSeed;

        public GameField Field { get; private set; }
        public Player Player { get; private set; }
        public EnemyAI EnemyAI { get; private set; }

        public MainViewModel()
        {
            currentSeed = new Random().Next();
            var random = new Random(currentSeed);
            var generator = new LevelGenerator();
            var generated = generator.Generate(Config.FieldWidth, Config.FieldHeight, random);
            Field = generated.field;
            Player = new Player(generated.x, generated.y);
            EnemyAI = new EnemyAI(random);
            SaveManager.Save(Player, currentSeed);
        }
        public void MovePlayer(int dx, int dy)
        {
            // TODO: If player is dead, trigger Game Over screen/popup here instead of just returning.
            if (!Player.IsAlive) return;
            var interaction = Player.Move(dx, dy, Field);
            EnemyAI.BuildDistanceMap(Field, Player);
            var worldInteraction = EnemyAI.UpdateEnemies(Field, Player);
            if (interaction == InteractionResult.Altar)
            {
                // TODO: Trigger Altar UI Popup (ask player if they want to spend gold to heal).
            }
            if (Player.IsExited)
            {
                if (Player.CurrentFloor == Config.MaxFloor)
                {
                    // TODO: Trigger Victory screen / Win Game popup.
                    return;
                }
                // TODO: Trigger "Descending..." popup or transition animation here.
                var random = new Random(currentSeed + Player.CurrentFloor + 1);
                var nextfloor = new LevelGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random);
                Field = nextfloor.field;
                Player.Descend(nextfloor.x, nextfloor.y);
                EnemyAI = new EnemyAI(random);
                SaveManager.Save(Player, currentSeed);
            }
        }
    }
}
