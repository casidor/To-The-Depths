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
    }
    public class EnemyTile : GameObject
    {
        public Enemy Enemy { get; private set; }
        public EnemyTile(Enemy enemy)
        {
            Enemy = enemy;
        }
        public override void Interact(Player player, GameField field, int x, int y)
        {
            
        }
    }
}
