using System;
using HutongGames.PlayMaker;
using MSCLoader;

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

        public string[,] currentControls
        {
            get
            {
                return currentPlayerMode == PlayerModeEnum.Driving ? this.drivingControls : currentPlayerMode == PlayerModeEnum.OnFoot ? this.footControls : null;
            }
            set
            {
                if (this.currentPlayerMode == PlayerModeEnum.Driving)
                    this.drivingControls = value;
                else
                    //if (currentPlayerMode == PlayerModeEnum.OnDriving)
                        this.footControls = value;
            }
        }
        /// <summary>
        /// Reprsents the current player mode.
        /// </summary>
        private PlayerModeEnum? currentPlayerMode;
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
        public string[,] footControls
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the current driving controls.
        /// </summary>
        public string[,] drivingControls
        {
            get;
            private set;
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
            
            this.currentPlayerMode = null;
            this.changeInputResult = new ChangeInput();
            cInput.OnKeyChanged += null;
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads provided control list to cInput.
        /// </summary>
        /// <param name="inControlMode">The control mode.</param>
        public void loadControlModeToCInput(PlayerModeEnum inPlayerMode, string[,] inControlMode)
        {
            // Written, 31.08.2018

            string controlListName = "game control";
            try
            {
                controlListName = inPlayerMode.Equals(PlayerModeEnum.OnFoot) ? "foot control" : "driving control";
                for (int i = 0; i < inControlMode.GetLength(0); i++)
                {
                    cInput.ChangeKey(inControlMode[i, 0], inControlMode[i, 1], inControlMode[i, 2]);
                }
                ModConsole.Print(String.Format("<b>[loadControlInputsToCInput]</b> - <b><color=green>Successfully</color></b> loaded {0} inputs to cInput.", controlListName));
            }
            catch (NullReferenceException)
            {                
                ModConsole.Print(String.Format("<b>[loadControlInputsToCInput]</b> - control inputs was null; setting {0} inputs to current control settings.", controlListName));
                if (inPlayerMode == PlayerModeEnum.OnFoot)
                    this.footControls = loadControlInputsFromCInput();
                else
                    this.drivingControls = loadControlInputsFromCInput();
            }
            catch
            {
                ModConsole.Print(String.Format("<b>[loadControlInputsToCInput]</b> - <b><color=red>Unsuccessfully</color></b> loaded {0} inputs to cInput.", controlListName));
                throw;
            }            
        }
        /// <summary>
        /// Changes the control mode.
        /// </summary>
        /// <param name="inControlMode">The mode to change to.</param>
        private void changeControlMode(PlayerModeEnum inControlMode)
        {
            // Written, 31.08.2018

            switch (inControlMode)
            {
                case PlayerModeEnum.Driving:
                    this.loadControlModeToCInput(inControlMode, this.drivingControls);
                    break;
                case PlayerModeEnum.OnFoot:
                    this.loadControlModeToCInput(inControlMode, this.footControls);
                    break;
            }
        }
        /// <summary>
        /// Occurs when cinput keys are changed externally, (the game gui controls).
        /// </summary>
        private void CInput_OnKeyChanged()
        {
            // Written, 09.07.2018

            this.currentControls = loadControlInputsFromCInput();
        }
        /// <summary>
        /// Loads control inputs (defined in <see cref="inputNames"/>) from the class, <see cref="cInput"/> and adds each one to <see cref="controlInputs"/> with it's primary
        /// and secondary input.
        /// </summary>
        public static string[,] loadControlInputsFromCInput()
        {
            // Written, 31.08.2018

            try
            {
                string[,] controls = new string[inputNames.Length, 3];
                for (int i = 0; i < inputNames.Length; i++)
                {
                    controls[i, 0] = cInput.GetText(inputNames[i], 0);
                    controls[i, 1] = cInput.GetText(inputNames[i], 1);
                    controls[i, 2] = cInput.GetText(inputNames[i], 2);
                }
                ModConsole.Print("<b>[loadControlInputsFromCInput]</b> - <b><color=green>Successfully</color></b> loaded game control inputs from cInput.");
                return controls;
            }
            catch (Exception ex)
            {
                ModConsole.Print("<b>[loadControlInputsFromCInput]</b> - <b><color=red>Unsuccessfully</color></b> loaded game control inputs from cInput.");
                ModConsole.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Occurs on update.
        /// </summary>
        public void onUpdate()
        {
            // Written, 31.08.2018

            if (this.currentPlayerMode != playerMode)
            {
                this.currentPlayerMode = playerMode;
                this.changeControlMode((PlayerModeEnum)currentPlayerMode);
                ModConsole.Print("Control Mode changed: " + currentPlayerMode);
            }
        }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inMode"></param>
        public void setGameControl(PlayerModeEnum inMode, string inControlName, int inIndex, string inInput)
        {
            // Written, 02.09.2018

            if (inIndex != 1 && inIndex != 2)
            {
                ModConsole.Error("<b>C285 PControlManager</b>\r\nIndex out of range for game control editing...");
                throw new IndexOutOfRangeException();
            }
            if (inMode == PlayerModeEnum.Driving)
            {
                for (int i = 0; i < this.drivingControls.GetLength(0); i++)
                {
                    string _controlName = this.drivingControls[i, 0];
                    if (inControlName == _controlName)
                    {
                        this.drivingControls[i, inIndex] = inInput;
                        break;
                    }
                }
                if (this.currentPlayerMode == inMode)
                    this.loadControlModeToCInput(inMode, this.drivingControls);
            }
            else
            {
                for (int i = 0; i < this.footControls.GetLength(0); i++)
                {
                    string _controlName = this.footControls[i, 0];
                    if (inControlName == _controlName)
                    {
                        this.footControls[i, inIndex] = inInput;
                        break;
                    }
                }
                if (this.currentPlayerMode == inMode)
                    this.loadControlModeToCInput(inMode, this.footControls);
            }
        }

        #endregion
    }
}
