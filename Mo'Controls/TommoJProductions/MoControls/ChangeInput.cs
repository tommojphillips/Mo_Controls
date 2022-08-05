using MSCLoader;

namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents data about changing an input.
    /// </summary>
    public class ChangeInput
    {
        // Written, 11.07.2018

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
        /// <summary>
        /// Initializes a new instance of <see cref="ChangeInput"/> and assigns the classes properties to the parameters.
        /// </summary>
        /// <param name="inReassignKey">Indicates if the program should begin ressign key function.</param>
        /// <param name="inInputName">The name of the input to change. If <paramref name="inReassignKey"/> is equal to false, this should be null.</param>
        /// <param name="inIndex">The input to change, either 1 or 2; 1 for primary input, 2 for secondary input. if <paramref name="inReassignKey"/> is equal to <see langword="false"/>, this should be equal to 0</param>
        public ChangeInput(bool inReassignKey, string inInputName, int inIndex, PlayerModeEnum? inMode = null)
        {
            // Written, 11.07.2018

            reassignKey = inReassignKey;
            controlName = inInputName;
            index = inIndex;
            mode = inMode;
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
        }

        /// <summary>
        /// Changes the <see cref="changeInputResult"/> to it's "polling" state, with the values provided, <paramref name="inControlName"/>, + <paramref name="inIndex"/>. Which inturn lets <see cref="Update"/> branch to <see cref="monitorForInput"/>.
        /// </summary>
        /// <param name="inControlName">The game control to change.</param>
        /// <param name="inIndex">The index to change, Primary = 1, Secondary = 2.</param>
        public void changeToPollingState(string inControlName, int inIndex, PlayerModeEnum? inMode = null)
        {
            // Written, 20.07.2018

            if (!reassignKey)
            {
                reassignKey = true;
                controlName = inControlName;
                index = inIndex;
                mode = inMode;
            }
        }

        #endregion
    }
}
