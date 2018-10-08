namespace TommoJProdutions.MoControls
{
    /// <summary>
    /// Reprsents monitor input data for <see cref="Input.monitorForInput"/>.
    /// </summary>
    public class MonitorInputData
    {
        /// <summary>
        /// Indicates whether the algorithm has found input.
        /// </summary>
        public bool foundInput
        {
            get;
            set;
        }
        /// <summary>
        /// The input that was found.
        /// </summary>
        public string input
        {
            get;
            set;
        }
    }
}
