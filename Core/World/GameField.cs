using GameCore.Models.Entities;
using GameCore.Models.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameCore.World
{
    public class GameField
    {
        private GameObject[,] _world;
        private Entity[,] _entities;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public FieldOfView Fov { get; private set; }

        public GameField(int width, int height)
        {
            Width = width;
            Height = height;
            _world = new GameObject[height, width];
            _entities = new Entity[height, width];
            Fov = new FieldOfView(width, height);
        }

        public GameObject this[int x, int y]
        {
            get => _world[y, x];
            set => _world[y, x] = value;
        }

        public Entity? GetEntity(int x, int y) => _entities[y, x];
        public void SetEntity(int x, int y, Entity? entity) => _entities[y, x] = entity;
        public bool HasEntity(int x, int y) => _entities[y, x] != null;
    }
}
