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
}
