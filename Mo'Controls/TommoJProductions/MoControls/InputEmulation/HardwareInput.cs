using System.Runtime.InteropServices;

namespace TommoJProductions.MoControls.InputEmulation
{
    /// <summary> 
    /// Define HARDWAREINPUT struct 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }
}
