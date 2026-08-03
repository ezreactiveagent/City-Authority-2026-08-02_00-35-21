using System;
using CityAuthority.Data;

namespace CityAuthority.SaveLoad
{
    // JsonUtility-serializable form of Notification (03 §5). districtId is ""
    // when there's no related district — JsonUtility can't serialize a null
    // UnityEngine.Object reference meaningfully, so districts are always
    // looked up by id against the SliceConfig at restore time.
    [Serializable]
    public sealed class NotificationData
    {
        public NotificationLevel level;
        public string message;
        public string districtId = "";
    }
}
