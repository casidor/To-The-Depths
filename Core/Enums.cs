namespace GameCore
{
    public enum InteractionResult
    {
        None,
        PlayerAttacked,
        EnemyAttacked,
        EnemyKilled,
        Altar,
        ExitReached,
        ItemPickedUp
    }
    public enum SaveResult
    {
        NotFound,
        Corrupted,
        Unverified,
        Success
    }
    public enum ExplorationState : byte
    {
        Unknown = 0,
        Explored = 1,
        Visible = 2
    }
    public enum UseResult
    {
        Failed,
        Missed,
        Hit
    }
    public enum GameEventType
    {
        DamageDealt,
        DamageTaken,
        EnemyKilled,
        ItemPickedUp,
        ItemEquipped,
        GoldCollected,
        KeyCollected,
        Healed,
        Missed,
        NoAmmo,
        NoTarget
    }
    public enum LogColor { Normal, Good, Bad }
}
