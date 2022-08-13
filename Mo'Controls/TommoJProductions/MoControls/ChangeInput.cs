using System;

using MSCLoader;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents data about changing an input.
    /// </summary>
    public class ChangeInput
    {
        // Written, 11.07.2018

        public enum ChangeInputType 
        {
            GameControl,
            KeyBind,
            XboxButton
        }

        #region Properties

        /// <summary>
        /// Represents whether to reassign a key or not.
        /// </summary>
        public bool reassignKey
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the name of the input to change; if <see cref="reassignKey"/> is false, this is equal to null. 
        /// The input name should equal the one from <see cref="cInput"/>.
        /// If ressign key is a mod keybind, (<see cref="mod"/> != <see langword="null"/>) then this, <see cref="controlName"/> should equal 
        /// <see cref="Keybind.ID"/> property.
        /// </summary>
        public string controlName
        {
            get;
            set;
        }
        /// <summary>
        /// Represents which input to modify, either 1 for primary input, or 2 for secondary input. If <see cref="reassignKey"/> is false, this is equal to null. 
        /// The input name should equal the one from <see cref="cInput"/>. 
        /// If ressign key is a mod keybind, (<see cref="mod"/> != <see langword="null"/>) then this, <see cref="index"/> should either equal 1 
        /// for <see cref="Keybind.Key"/> or 2 for <see cref="Keybind.Modifier"/>.
        /// </summary>
        public int index
        {
            get;
            set;
        }
        /// <summary>
        /// Reprents the mode to modify.
        /// </summary>
        public PlayerModeEnum? mode
        {
            get;
            set;
        }
        public ChangeInputType type { get; set; }

        public Action action { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ChangeInput"/> and assigns the classes properties to its "off"/default state. 
        /// </summary>
        public ChangeInput()
        {
            // Written, 16.07.2018

            reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// resets this instance of <see cref="ChangeInput"/> and assigns the classes properties to the parameters.
        /// </summary>
        public void reset()
        {
            // Written, 30.07.2022

            reassignKey = false;
            controlName = null;
            index = 0;
            mode = null;
            type = 0;
            action = null;
        }

        /// <summary>
        /// Changes the <see cref="changeInputResult"/> to it's "polling" state, with the values provided, <paramref name="controlName"/>, + <paramref name="index"/>. Which inturn lets <see cref="Update"/> branch to <see cref="monitorForInput"/>.
        /// </summary>
        /// <param name="controlName">The game control to change.</param>
        /// <param name="index">The index to change, Primary = 1, Secondary = 2.</param>
        public void changeToPollingState(string controlName, int index, PlayerModeEnum? mode, ChangeInputType type, Action action)
        {
            // Written, 20.07.2018

            if (!reassignKey)
            {
                reassignKey = true;
                this.controlName = controlName;
                this.index = index;
                this.mode = mode;
                this.type = type;
                this.action = action;
            }
        }

        #endregion
    }
}
