namespace TommoJProductions.MoControls
{
    /// <summary>
    /// Represents different updating schemes invoked by the Unity runtime.
    /// </summary>
    public enum UnityRuntimeUpdateSchemesEnum
    {
        // Written, 17.10.2020

        /// <summary>
        /// Represents the method, Update() invoked by The Unity Runtime.
        /// </summary>
        update,
        /// <summary>
        /// Represents the method, LateUpdate() invoked by The Unity Runtime.
        /// </summary>
        lateUpdate,
        /// <summary>
        /// Represents the method, FixedUpdate() invoked by The Unity Runtime.
        /// </summary>
        fixedUpdate
    }
}
