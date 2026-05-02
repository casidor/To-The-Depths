using GameCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.World
{
    public record GameEvent(
    GameEventType Type,
    string Text,
    char Icon,
    int X = 0,
    int Y = 0,
    int Amount = 0,
    Entity? Source = null,
    Entity? Target = null
    );

    public class GameLog
    {
        private readonly List<GameEvent> _events = new();
        public IReadOnlyList<GameEvent> Events => _events;

        public void Add(GameEventType type, string text, char icon, int x = 0, int y = 0, int Amount = 0, Entity? source = null, Entity? target = null)
            => _events.Add(new(type, text, icon, x, y, Amount, source, target));

        public IEnumerable<GameEvent> Get(params GameEventType[] types)
            => _events.Where(e => types.Contains(e.Type));

        public void Clear() => _events.Clear();
    }
}
