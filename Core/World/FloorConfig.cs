namespace GameCore
{
    public record FloorData
        (
        int EnemyCount,
        float EnemyHPMultiplier,
        int GoldDropMin,
        int GoldDropMax,
        bool SpawnTank,
        bool SpawnRanged,
        int KeysAmount,
        int FloorGoldMin,
        int FloorGoldMax
    );

    public static class FloorConfig
    {
        public static FloorData Get(int floor) => floor switch
        {
            1 => new(7, 1.0f, 3, 8, false, false, 3, 5, 13),
            2 => new(9, 1.3f, 5, 12, false, false, 3, 7, 15),
            3 => new(12, 1.6f, 5, 12, true, true, 4, 9, 17),
            4 => new(15, 2.0f, 8, 18, true, true, 4, 11, 20),
            5 => new(18, 2.5f, 8, 18, true, true, 5, 13, 23),
            _ => new(5, 1.0f, 3, 8, false, false, 3, 5, 13)
        };
    }
}