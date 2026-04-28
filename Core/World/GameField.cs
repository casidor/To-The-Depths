using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.World
{
    public class GameField
    {
        private GameObject[,] Field;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public FieldOfView Fov { get; private set; }
        public GameField(int width, int height )
        {
            Width = width;
            Height = height;
            Field = new GameObject[height, width];
            Fov = new FieldOfView(width, height);
        }
        public GameObject this[int x, int y]
        {
            get { return Field[y, x]; }
            set { Field[y, x] = value; }
        }
    }
}
