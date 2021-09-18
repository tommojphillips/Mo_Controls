using System;

namespace TommoJProductions.MoControls
{
    public class OldSaveDataException : Exception
    {
        public MoControlsSaveData oldSaveData;

        public OldSaveDataException(MoControlsSaveData inOldSaveData) 
        {
            oldSaveData = inOldSaveData;
        }
    }
}
