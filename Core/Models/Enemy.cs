using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models
{
    public class Enemy : Entity
    {
        public Enemy(string name, int hp, int attack)
        {
            Name = name;
            HP = hp;
            MaxHP = hp;
            Attack = attack;
        }
        public static Enemy GetEnemy(Random random)
        {
            var data = EnemyData.Types[random.Next(EnemyData.Types.Count)];
            return new Enemy(data.Name, data.HP, data.Attack);
        }
    }
    public class EnemyTile : GameObject
    {
        public Enemy Enemy { get; private set; }
        public EnemyTile(Enemy enemy)
        {
            Symbol = GameSymbols.Enemy;
            IsPassable = true;
            Color = GameColors.Enemy;
            Enemy = enemy;
        }
        public override void Interact(Player player, GameField field, int x, int y)
        {
            player.StartBattle(Enemy);
        }
    }
}
