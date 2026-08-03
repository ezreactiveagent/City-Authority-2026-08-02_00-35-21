using System;
using System.Collections.Generic;

namespace CityAuthority.SaveLoad
{
    // 08 §13 item 8: the complete on-disk record of a mid-scenario save.
    // JsonUtility-compatible (plain public fields, no properties, no nullable
    // value types) so it can round-trip through JsonUtility.ToJson/FromJson.
    [Serializable]
    public sealed class ScenarioSaveData
    {
        public List<AccountabilityEventData> logEvents = new();
        public EmergencyStateData emergency = new();
        public bool structureCondemned;
        public bool hasCourtRuling;
        public CourtRulingData courtRuling;
        public DevelopmentStateData development = new();
        public List<NewsArticleData> articles = new();
        public bool finalReportGenerated;
    }
}
