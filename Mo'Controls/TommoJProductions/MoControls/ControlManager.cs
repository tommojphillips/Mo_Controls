using System;
using System.Linq;
using HutongGames.PlayMaker;
using MSCLoader;
using TommoJProductions.Debugging;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    public class ControlManager : MonoBehaviour
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
            "Watch",
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
            cInput.OnKeyChanged -= this.CInput_OnKeyChanged;
            cInput.OnKeyChanged += this.CInput_OnKeyChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs after game starts.
        /// </summary>
        private void Start()
        {
            // Written, 08.10.2018

            MoControlsMod.print(nameof(ControlManager) + ": Started", DebugTypeEnum.full);
        }
        /// <summary>
        /// Occurs on update.
        /// </summary>
        private void Update()
        {
            // Written, 31.08.2018

            if (this.currentPlayerMode != playerMode)
            {
                this.currentPlayerMode = playerMode;
                this.changeControlMode((PlayerModeEnum)currentPlayerMode);
                MoControlsMod.print("Control Mode changed: " + currentPlayerMode, DebugTypeEnum.full);
            }
        }
        /// <summary>
        /// Loads provided control list to cInput.
        /// </summary>
        /// <param name="inControlMode">The control mode.</param>
        internal void loadControlModeToCInput(PlayerModeEnum inPlayerMode, string[,] inControlMode)
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
                    MoControlsMod.print(String.Format("<b><color=green>Successfully</color></b> loaded {0} inputs to cInput.", controlListName), DebugTypeEnum.full);
            }
            catch (NullReferenceException)
            {
                MoControlsMod.print(String.Format("control inputs was null; setting {0} inputs to current control settings.", controlListName), DebugTypeEnum.full);
                if (inPlayerMode == PlayerModeEnum.OnFoot)
                    this.footControls = loadControlInputsFromCInput();
                else
                    this.drivingControls = loadControlInputsFromCInput();
            }
            catch
            {
                MoControlsMod.print(String.Format("<b><color=red>Unsuccessfully</color></b> loaded {0} inputs to cInput.", controlListName), DebugTypeEnum.full);
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
        internal static string[,] loadControlInputsFromCInput()
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
                    MoControlsMod.print("<b><color=green>Successfully</color></b> loaded game control inputs from cInput.", DebugTypeEnum.full);
                return controls;
            }
            catch (Exception ex)
            {
                MoControlsMod.print("<b><color=red>Unsuccessfully</color></b> loaded game control inputs from cInput.", DebugTypeEnum.full);
                ModConsole.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Assigns the control groups.
        /// </summary>
        /// <param name="inFootControls">The group of foot controls to assign.</param>
        /// <param name="inDrivingControls">The group of driving controls to assign.</param>
        internal void setControls(string[,] inFootControls, string[,] inDrivingControls)
        {
            // Written, 22.08.2018

            this.footControls = inFootControls;
            this.drivingControls = inDrivingControls;
        }
        /// <summary>
        /// Sets the provided game control in provided mode.
        /// </summary>
        /// <param name="inMode"></param>
        internal void setGameControl(PlayerModeEnum inMode, string inControlName, int inIndex, string inInput)
        {
            // Written, 02.09.2018

            if (inIndex != 1 && inIndex != 2)
            {
                MoControlsMod.print("<b>C285 PControlManager</b>\r\nIndex out of range for game control editing...", DebugTypeEnum.full);
                throw new IndexOutOfRangeException();
            }
            switch (inMode)
            {
                case PlayerModeEnum.Driving:
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
                    break;
                case PlayerModeEnum.OnFoot:
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
                    break;
            }
        }
        /// <summary>
        /// Changes the input for a control defined in <see cref="changeInputResult"/> to the provided input string, <paramref name="inInput"/>.
        /// </summary>
        /// <param name="inInput">The input to assign.</param>
        internal void changeInput(string inInput)
        {
            // Written, 09.07.2018

            if (!this.changeInputResult.isModKeybind)
            {
                // Treat as a game control.

                PlayerModeEnum? playerMode = this.changeInputResult?.mode;

                if (playerMode == null)
                {
                    bool mistake = true;
                    ModUI.ShowYesNoMessage("Player Mode was null, is that right?", "Mistake?", delegate ()
                    {
                        mistake = false;
                    });
                    if (!mistake)
                    {

                        if (this.changeInputResult.index == 1)
                            cInput.ChangeKey(this.changeInputResult.controlName, inInput, cInput.GetText(this.changeInputResult.controlName, 2));
                        else
                            cInput.ChangeKey(this.changeInputResult.controlName, cInput.GetText(this.changeInputResult.controlName, 1), inInput);
                        this.currentControls = loadControlInputsFromCInput();
                    }
                    MoControlsMod.print("Player mode wasa null while attempting to change input..", DebugTypeEnum.full);
                }
                else
                {
                        this.setGameControl((PlayerModeEnum)playerMode, this.changeInputResult.controlName, this.changeInputResult.index, inInput);
                        MoControlsMod.print("Player mode was equal to <b>" + this.changeInputResult.mode + "</b> whiling setting '" + this.changeInputResult.controlName + "' to '" + inInput + "'.", DebugTypeEnum.full);
                    MoControlsSaveData.saveSettings(MoControlsMod.moControlsGO);
                }
            }
            else
            {
                // Treat as a mod keybind.

                Keybind modKeybind = Keybind.Get(this.changeInputResult.mod).Where(kb => kb.ID == this.changeInputResult.controlName).First();
                if (this.changeInputResult.index == 1)
                {
                    modKeybind.Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), inInput);
                }
                else
                {
                    modKeybind.Key = (KeyCode)Enum.Parse(typeof(KeyCode), inInput);
                }
                ModSettings_menu.SaveModBinds(this.changeInputResult.mod);
                    MoControlsMod.print("saved <i>" + modKeybind.Mod.Name + "</i> mod keybinds.", DebugTypeEnum.full);

            }
            this.changeInputResult = new ChangeInput();
        }

        #endregion
    }
}
