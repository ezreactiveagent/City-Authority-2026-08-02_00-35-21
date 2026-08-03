using System;

namespace CityAuthority.SaveLoad
{
    [Serializable]
    public sealed class NewsArticleData
    {
        public string sourceEventType;
        public string headline;
        public string body;
        public string districtId = "";
    }
}
