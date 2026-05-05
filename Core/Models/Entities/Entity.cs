namespace GameCore.Models.Entities
{
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public virtual string SpriteName => GetType().Name.ToLower();
    }
}
