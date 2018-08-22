using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;

namespace Mo_Controls
{
    public class ControlManager
    {
        // Written, 22.08.2018

        #region Fields

        /// <summary>
        /// Represents all functional inputs for the game.
        /// </summary>
        public static string[] inputNames = new string[]
        {
            "Left",
            "Right",
            "Throttle",
            "Brake",
            "Clutch",
            "ShiftUp",
            "ShiftDown",
            "IndicatorLeft",
            "IndicatorRight",
            "Range",
            "HighBeam",
            "Wipers",
            "Boost",
            "Handbrake",
            "DrivingMode",
            "PlayerLeft",
            "PlayerRight",
            "PlayerUp",
            "PlayerDown",
            "Jump",
            "Run",
            "Zoom",
            "Use",
            "Crouch",
            "ReachLeft",
            "ReachRight",
            "Hitchhike",
            "Swear",
            "Hit",
            "Push",
            "Finger",
            "Urinate",
            "Drunk",
            "Smoking",
            "reverse",
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "sixth",
        };

        #endregion

        #region Properties

        /// <summary>
        /// Represents the current player mode. either on foot, or driving.
        /// </summary>
        public static PlayerModeEnum playerMode
        {
            get
            {
                PlayerModeEnum pme;
                string currentVehicle = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value;
                switch (currentVehicle)
                {
                    case "":
                        pme = PlayerModeEnum.OnFoot;
                        break;
                    default:
                        pme = PlayerModeEnum.Driving;
                        break;
                }
                return pme;
            }
        }
        /// <summary>
        /// Represents the change input result for the mod.
        /// </summary>
        public ChangeInput changeInputResult
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the current foot controls.
        /// </summary>
        private string[,] footControls
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the current driving controls.
        /// </summary>
        private string[,] drivingControls
        {
            get;
            set;
        }
        /// <summary>
        /// Represents whether this should display the overlay.
        /// </summary>
        public bool displayCurrentPlayerModeOverlay
        {
            get;
            set;
        }

        #endregion

        #region Constuctors

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ControlManager()
        {
            // Written, 22.08.2018

            this.changeInputResult = new ChangeInput();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assigns the control groups.
        /// </summary>
        /// <param name="inFootControls">The group of foot controls to assign.</param>
        /// <param name="inDrivingControls">The group of driving controls to assign.</param>
        public void setControls(string[,] inFootControls, string[,] inDrivingControls)
        {
            // Written, 22.08.2018

            this.footControls = inFootControls;
            this.drivingControls = inDrivingControls;
        }

        #endregion
    }
}
