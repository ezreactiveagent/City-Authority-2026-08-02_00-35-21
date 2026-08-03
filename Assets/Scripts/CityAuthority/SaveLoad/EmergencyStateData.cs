using System;

namespace CityAuthority.SaveLoad
{
    [Serializable]
    public sealed class EmergencyStateData
    {
        public bool warningRaised;
        public bool criticalRaised;
        public bool warningResponded;
        public bool criticalResponded;
        public int committedUnitCount;
        public bool hasDispatchResult;
        public DispatchResultData dispatchResult;
    }
}
