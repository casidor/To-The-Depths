using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public enum GameState
    {
        MainMenu,
        Running,
        Help,
        Exit
    }
    public enum InteractionResult
    {
        None,
        PlayerAttacked,
        AltarMenu,
        ExitReached
    }
}
