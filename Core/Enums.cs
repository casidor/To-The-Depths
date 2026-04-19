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
        LoadGame,
        Help,
        Exit
    }
    public enum InteractionResult
    {
        None,
        PlayerAttacked,
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
}
