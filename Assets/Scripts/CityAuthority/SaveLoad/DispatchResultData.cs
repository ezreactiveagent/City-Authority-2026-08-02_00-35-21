using System;
using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.SaveLoad
{
    [Serializable]
    public sealed class DispatchResultData
    {
        public CoverageState targetDistrictCoverage;
        public float severityMultiplier;
        public List<NotificationData> secondaryNotifications = new();
    }
}
