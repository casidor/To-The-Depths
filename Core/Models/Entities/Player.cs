using GameCore.Models.Items;
using GameCore.World;

namespace GameCore.Models.Entities
{
    public class Player : Entity
    {
        public int MaxHP { get; private set; }
        public int HP { get; private set; }
        public int Damage => Inventory.EquippedMelee?.Damage ?? Config.PlayerDamage;
        public bool IsAlive => HP > 0;
        public int KeysCollected { get; private set; } = 0;
        public int CurrentFloor { get; private set; } = 1;
        private int _gold;
        public int GoldCollected
        {
            get => _gold;
            private set => _gold = Math.Max(value, 0);
        }
        public bool IsExited { get; private set; } = false;
        public Inventory Inventory { get; } = new();
        public int KeysRequired { get; private set; } = Config.KeysAmount;
        public Player(int x, int y)
        {
            X = x;
            Y = y;
            HP = Config.PlayerMaxHP;
            MaxHP = Config.PlayerMaxHP;
        }
        public Player(int x, int y, int hp, int maxHp, int gold, int keys, int floor, int keysRequired, SavedInventoryData inventory)
        {
            X = x;
            Y = y;
            HP = hp;
            MaxHP = maxHp;
            GoldCollected = gold;
            KeysCollected = keys;
            CurrentFloor = floor;
            KeysRequired = keysRequired;
            Inventory.RestoreFromSave(inventory);
        }
        public InteractionResult Move(int dX, int dY, GameField field)
        {
            int newX = X + dX;
            int newY = Y + dY;
            if (field.GetEntity(newX, newY) is Enemy enemy)
                return enemy.Interact(this, field, newX, newY);
            var cell = field[newX, newY];
            var result = cell.Interact(this, field, newX, newY);
            if (field[newX, newY].IsPassable)
            {
                X = newX;
                Y = newY;
            }
            return result;
        }
        public void Descend(int startX, int startY, int keysRequired)
        {
            X = startX;
            Y = startY;
            KeysCollected = 0;
            IsExited = false;
            CurrentFloor++;
            KeysRequired = keysRequired;
        }
        public void AddGold(int amount) => GoldCollected += amount;
        public void AddKey() => KeysCollected++;
        public void Exit() => IsExited = true;
        public void TakeDamage(int damage) => HP = Math.Max(HP - damage, 0);
        public bool SpendGold(int amount)
        {
            if (GoldCollected < amount) return false;
            GoldCollected -= amount;
            return true;
        }
        public bool Heal(int amount)
        {
            if (HP >= MaxHP) return false;
            HP = Math.Min(HP + amount, MaxHP);
            return true;
        }
    }
}
