namespace GameCore
{
    public record FloorData(
        int EnemyCount,
        float EnemyHPMultiplier,
        int GoldDropMin,
        int GoldDropMax,
        bool SpawnFast,
        bool SpawnTank,
        bool SpawnRanged
    );

    public static class FloorConfig
    {
        public static FloorData Get(int floor) => floor switch
        {
            1 => new(5, 1.0f, 3, 8, false, false, false),
            2 => new(6, 1.3f, 5, 12, true, false, false),
            3 => new(7, 1.6f, 5, 12, true, true, false),
            4 => new(9, 2.0f, 8, 18, true, true, true),
            5 => new(12, 2.5f, 8, 18, true, true, true),
            _ => new(5, 1.0f, 3, 8, false, false, false)
        };
    }
}