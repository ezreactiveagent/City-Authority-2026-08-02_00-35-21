using System;
using CityAuthority.Data;

namespace CityAuthority.SaveLoad
{
    // JsonUtility-serializable form of AccountabilityEvent. hasLevel/hasCategory
    // stand in for the nullable fields JsonUtility can't serialize directly.
    [Serializable]
    public sealed class AccountabilityEventData
    {
        public string eventType;
        public string summary;
        public string districtId = "";
        public bool hasLevel;
        public NotificationLevel level;
        public bool hasCategory;
        public AccountabilityCategory category;
    }
}
