using System;

namespace AiBots.Threads;

[Flags]
public enum MouseState
{
    None = 0,
    Left = 1,
    Right = 2,
    LeftRight = Right | Left, // 0x00000003
}