using System;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents all game controls.
    /// </summary>
    [Flags]
    internal enum GameControlsEnum
    {
        // Written, 20.01.2019

        Null = 0,
        Left = 1,
        Right = 4,
        Throttle = 9,
        Brake = 16,
        Clutch = 25,
        ShiftUp = 36,
        ShiftDown = 49,
        IndicatorLeft = 64,
        IndicatorRight = 81,
        Range = 100,
        HighBeam = 121,
        Wipers = 144,
        Boost = 169,
        Handbrake = 1764,
        DrivingMode = 196,
        PlayerLeft = 225,
        PlayerRight = 256,
        PlayerUp = 289,
        PlayerDown = 324,
        Jump = 361,
        Run = 400,
        Zoom = 441,
        Use = 484,
        Crouch = 529,
        WristWatch = 576,
        ReachLeft = 625,
        ReachRight = 676,
        Hitchhike = 729,
        Swear = 784,
        Hit = 841,
        Push = 900,
        Finger = 961,
        Urinate = 1024,
        Drunk = 1089,
        Smoke = 1156,
        Reverse = 1225,
        First = 1296,
        Second = 1369,
        Third = 1444,
        Fourth = 1521,
        Fifth = 1600,
        Sixth = 1681,
    }
}
