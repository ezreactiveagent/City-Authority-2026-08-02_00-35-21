using CityAuthority.Data;

namespace CityAuthority.Accountability
{
    // A single entry in the City Log (03 §5) / accountability record (05 §13):
    // warnings issued, player responses, and the mechanical outcomes they produced.
    public sealed class AccountabilityEvent
    {
        public string EventType { get; }
        public string Summary { get; }
        public District RelatedDistrict { get; }
        public NotificationLevel? RelatedLevel { get; }
        public AccountabilityCategory? Category { get; }

        public AccountabilityEvent(
            string eventType,
            string summary,
            District relatedDistrict = null,
            NotificationLevel? relatedLevel = null,
            AccountabilityCategory? category = null)
        {
            EventType = eventType;
            Summary = summary;
            RelatedDistrict = relatedDistrict;
            RelatedLevel = relatedLevel;
            Category = category;
        }
    }
}
