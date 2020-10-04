namespace TommoJProductions.ModAPI
{
    /// <summary>
    /// Represents Mod Api data.
    /// </summary>
    internal class ModApiData
    {
        // Written, 06.01.2019

        #region Properties

        /// <summary>
        /// Represents whether the end-user is running a version of mod api.
        /// </summary>
        internal bool runningModApi
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the version of the mod api.
        /// </summary>
        internal string version
        {
            get;
            private set;
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initilizes a new instance of <see cref="ModApi"/> and assigns it's members.
        /// </summary>
        /// <param name="inRunningModApi">Param1</param>
        /// <param name="inVersion">Param2</param>
        internal ModApiData(bool inRunningModApi, string inVersion)
        {
            // Written, 06.01.2019

            this.runningModApi = inRunningModApi;
            this.version = inVersion;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="ModApiData"/> and assigns <see cref="ModApiData.runningModApi"/> to the parameter. and calls to <see cref="ModApi.ModClient.version"/>.
        /// </summary>
        /// <param name="inRunningModApi">Param1</param>
        internal ModApiData(bool inRunningModApi)
        {
            // Written, 07.01.2019

            this.runningModApi = inRunningModApi;
            if (inRunningModApi)
                this.version = ModApi.ModClient.version;
            else
                this.version = null;
        }

        #endregion
    }
}
