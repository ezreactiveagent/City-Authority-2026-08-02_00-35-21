using System;

namespace CityAuthority.SaveLoad
{
    [Serializable]
    public sealed class DevelopmentStateData
    {
        public float developerInterest;
        public bool rejected;
        public string approvedProposalId = "";
    }
}
