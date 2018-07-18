namespace Mo_Controls.XboxController.Monitoring
{
    /// <summary>
    /// Represents current and previous connection statuses.
    /// </summary>
    public class ControllerConnection
    {
        /// <summary>
        /// Represents the current connection status.
        /// </summary>
        public bool? currentConnectionStatus
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the previous connection status.
        /// </summary>
        public bool? previousConnectionStatus
        {
            get;
            set;
        }
    }
}
