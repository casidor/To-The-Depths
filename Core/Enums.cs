using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public enum GameState
    {
        MainMenu,
        Generating,
        Running,
        AltarMenu,
        Win,
        LoadGame,
        Help,
        Exit
    }
    public enum InteractionResult
    {
        None,
        PlayerAttacked,
        EnemyAttacked,
        EnemyKilled,
        Altar,
        ExitReached
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
}
