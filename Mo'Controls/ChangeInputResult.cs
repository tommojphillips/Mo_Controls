using MSCLoader;

namespace Mo_Controls
{
    /// <summary>
    /// Represents data about changing an input.
    /// </summary>
    public class ChangeInputResult
    {
        // Written, 11.07.2018

        #region Properties
        
        /// <summary>
        /// Returns whether the input to change is a mod keybind.
        /// </summary>
        public bool isModKeybind
        {
            get
            {
                return mod != null ? true : false;
            }
        }
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
        /// If ressign key is a mod keybind, (<see cref="mod"/> != <see langword="null"/>) then this, <see cref="inputName"/> should equal 
        /// <see cref="Keybind.Name"/> property.
        /// </summary>
        public string inputName
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
        /// Represents the mod to modify the inputs. Only vaild if the input to change is a mod keybind! if not <see langword="null"/>.
        /// </summary>
        public Mod mod
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ChangeInputResult"/> and assigns the classes properties to its "off"/default state. 
        /// </summary>
        public ChangeInputResult()
        {
            // Written, 16.07.2018

            this.reassignKey = false;
            this.inputName = null;
            this.index = 0;
            this.mod = null;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="ChangeInputResult"/> and assigns the classes properties to the parameters.
        /// </summary>
        /// <param name="reassignKey">Indicates if the program should begin ressign key function.</param>
        /// <param name="inputName">The name of the input to change. If <paramref name="reassignKey"/> is equal to false, this should be null.</param>
        /// <param name="index">The input to change, either 1 or 2; 1 for primary input, 2 for secondary input. if <paramref name="reassignKey"/> is equal to <see langword="false"/>, this should be equal to 0</param>
        /// <param name="mod">The mod to change keybind.</param>
        public ChangeInputResult(bool reassignKey, string inputName, int index, Mod mod = null)
        {
            // Written, 11.07.2018

            this.reassignKey = reassignKey;
            this.inputName = inputName;
            this.index = index;
            this.mod = mod;
        }

        #endregion
    }
}
