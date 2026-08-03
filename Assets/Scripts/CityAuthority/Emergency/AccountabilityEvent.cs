using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // A single entry destined for the City Log / accountability record (03 §5-6, 05 §13).
    // The full logging system is Step 3 (08 §14); this is the minimal shape every
    // slice node can already write into, per 11 §2's "record an event" interface.
    public sealed class AccountabilityEvent
    {
        public string EventType { get; }
        public string Summary { get; }
        public District RelatedDistrict { get; }

        public AccountabilityEvent(string eventType, string summary, District relatedDistrict = null)
        {
            EventType = eventType;
            Summary = summary;
            RelatedDistrict = relatedDistrict;
        }
    }
}
