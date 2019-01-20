using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TommoJProductions.MoControls
{
    internal static class Extentions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGameControlName"></param>
        /// <returns></returns>
        internal static GameControlsEnum getGameControl(this string inGameControlName)
        {
            // Written, 20.01.2019

            GameControlsEnum? gameControl = null;
            switch (inGameControlName)
            {
                case "Left":
                    gameControl = GameControlsEnum.Left;
                    break;
                case "Right":
                    gameControl = GameControlsEnum.Right;
                    break;
                case "Throttle":
                    gameControl = GameControlsEnum.Throttle;
                    break;
                case "Brake":
                    gameControl = GameControlsEnum.Brake;
                    break;
                case "Clutch":
                    gameControl = GameControlsEnum.Clutch;
                    break;
                case "ShiftUp":
                    gameControl = GameControlsEnum.ShiftUp;
                    break;
                case "ShiftDown":
                    gameControl = GameControlsEnum.ShiftDown;
                    break;
                case "IndicatorLeft":
                    gameControl = GameControlsEnum.IndicatorLeft;
                    break;
                case "IndicatorRight":
                    gameControl = GameControlsEnum.IndicatorRight;
                    break;
                case "Range":
                    gameControl = GameControlsEnum.Range;
                    break;
                case "HighBeam":
                    gameControl = GameControlsEnum.HighBeam;
                    break;
                case "Wipers":
                    gameControl = GameControlsEnum.Wipers;
                    break;
                case "Boost":
                    gameControl = GameControlsEnum.Boost;
                    break;
                case "Handbrake":
                    gameControl = GameControlsEnum.Handbrake;
                    break;
                case "DrivingMode":
                    gameControl = GameControlsEnum.DrivingMode;
                    break;
                case "PlayerLeft":
                    gameControl = GameControlsEnum.PlayerLeft;
                    break;
                case "PlayerRight":
                    gameControl = GameControlsEnum.PlayerRight;
                    break;
                case "PlayerUp":
                    gameControl = GameControlsEnum.PlayerUp;
                    break;
                case "PlayerDown":
                    gameControl = GameControlsEnum.PlayerDown;
                    break;
                case "Jump":
                    gameControl = GameControlsEnum.Jump;
                    break;
                case "Run":
                    gameControl = GameControlsEnum.Run;
                    break;
                case "Zoom":
                    gameControl = GameControlsEnum.Zoom;
                    break;
                case "Use":
                    gameControl = GameControlsEnum.Use;
                    break;
                case "Crouch":
                    gameControl = GameControlsEnum.Crouch;
                    break;
                case "Watch":
                    gameControl = GameControlsEnum.WristWatch;
                    break;
                case "ReachLeft":
                    gameControl = GameControlsEnum.ReachLeft;
                    break;
                case "ReachRight":
                    gameControl = GameControlsEnum.ReachRight;
                    break;
                case "Hitchhike":
                    gameControl = GameControlsEnum.Hitchhike;
                    break;
                case "Swear":
                    gameControl = GameControlsEnum.Swear;
                    break;
                case "Hit":
                    gameControl = GameControlsEnum.Hit;
                    break;
                case "Push":
                    gameControl = GameControlsEnum.Push;
                    break;
                case "Finger":
                    gameControl = GameControlsEnum.Finger;
                    break;
                case "Urinate":
                    gameControl = GameControlsEnum.Urinate;
                    break;
                case "Drunk":
                    gameControl = GameControlsEnum.Drunk;
                    break;
                case "Smoking":
                    gameControl = GameControlsEnum.Smoke;
                    break;
                case "reverse":
                    gameControl = GameControlsEnum.Reverse;
                    break;
                case "first":
                    gameControl = GameControlsEnum.First;
                    break;
                case "second":
                    gameControl = GameControlsEnum.Second;
                    break;
                case "third":
                    gameControl = GameControlsEnum.Third;
                    break;
                case "fourth":
                    gameControl = GameControlsEnum.Fourth;
                    break;
                case "fifth":
                    gameControl = GameControlsEnum.Fifth;
                    break;
                case "sixth":
                    gameControl = GameControlsEnum.Sixth;
                    break;
                default:
                    gameControl = GameControlsEnum.Null;
                    break;
            }
            return (GameControlsEnum)gameControl;
        }
        internal static string getName(this GameControlsEnum inGameControl)
        {
            // Written, 20.01.2019

            string gameControlName = "";
            switch (inGameControl)
            {
                case GameControlsEnum.Left:
                    gameControlName = "Left";
                    break;
                case GameControlsEnum.Right:
                    gameControlName = "Right";
                    break;
                case GameControlsEnum.Throttle:
                    gameControlName = "Throttle";
                    break;
                case GameControlsEnum.Brake:
                    gameControlName = "Brake";
                    break;
                case GameControlsEnum.Clutch:
                    gameControlName = "Clutch";
                    break;
                case GameControlsEnum.ShiftUp:
                    gameControlName = "ShiftUp";
                    break;
                case GameControlsEnum.ShiftDown:
                    gameControlName = "ShiftDown";
                    break;
                case GameControlsEnum.IndicatorLeft:
                    gameControlName = "IndicatorLeft";
                    break;
                case GameControlsEnum.IndicatorRight:
                    gameControlName = "IndicatorRight";
                    break;
                case GameControlsEnum.Range:
                    gameControlName = "Range";
                    break;
                case GameControlsEnum.HighBeam:
                    gameControlName = "HighBeam";
                    break;
                case GameControlsEnum.Wipers:
                    gameControlName = "Wipers";
                    break;
                case GameControlsEnum.Boost:
                    gameControlName = "Boost";
                    break;
                case GameControlsEnum.Handbrake:
                    gameControlName = "Handbrake";
                    break;
                case GameControlsEnum.DrivingMode:
                    gameControlName = "DrivingMode";
                    break;
                case GameControlsEnum.PlayerLeft:
                    gameControlName = "PlayerLeft";
                    break;
                case GameControlsEnum.PlayerRight:
                    gameControlName = "PlayerRight";
                    break;
                case GameControlsEnum.PlayerUp:
                    gameControlName = "PlayerUp";
                    break;
                case GameControlsEnum.PlayerDown:
                    gameControlName = "PlayerDown";
                    break;
                case GameControlsEnum.Jump:
                    gameControlName = "Jump";
                    break;
                case GameControlsEnum.Run:
                    gameControlName = "Run";
                    break;
                case GameControlsEnum.Zoom:
                    gameControlName = "Zoom";
                    break;
                case GameControlsEnum.Use:
                    gameControlName = "Use";
                    break;
                case GameControlsEnum.Crouch:
                    gameControlName = "Crouch";
                    break;
                case GameControlsEnum.WristWatch:
                    gameControlName = "Watch";
                    break;
                case GameControlsEnum.ReachLeft:
                    gameControlName = "ReachLeft";
                    break;
                case GameControlsEnum.ReachRight:
                    gameControlName = "ReachRight";
                    break;
                case GameControlsEnum.Hitchhike:
                    gameControlName = "Hitchhike";
                    break;
                case GameControlsEnum.Swear:
                    gameControlName = "Swear";
                    break;
                case GameControlsEnum.Hit:
                    gameControlName = "Hit";
                    break;
                case GameControlsEnum.Push:
                    gameControlName = "Push";
                    break;
                case GameControlsEnum.Finger:
                    gameControlName = "Finger";
                    break;
                case GameControlsEnum.Urinate:
                    gameControlName = "Urinate";
                    break;
                case GameControlsEnum.Drunk:
                    gameControlName = "Drunk";
                    break;
                case GameControlsEnum.Smoke:
                    gameControlName = "Smoking";
                    break;
                case GameControlsEnum.Reverse:
                    gameControlName = "reverse";
                    break;
                case GameControlsEnum.First:
                    gameControlName = "first";
                    break;
                case GameControlsEnum.Second:
                    gameControlName = "second";
                    break;
                case GameControlsEnum.Third:
                    gameControlName = "third";
                    break;
                case GameControlsEnum.Fourth:
                    gameControlName = "fourth";
                    break;
                case GameControlsEnum.Fifth:
                    gameControlName = "fifth";
                    break;
                case GameControlsEnum.Sixth:
                    gameControlName = "sixth";
                    break;
                case GameControlsEnum.Null:
                    gameControlName = null;
                    break;
            }
            return gameControlName;
        }
    }
}
