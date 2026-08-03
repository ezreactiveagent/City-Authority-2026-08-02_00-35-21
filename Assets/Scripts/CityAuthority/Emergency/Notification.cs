using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // 03 §5. The slice's engine layer only needs the notification to exist and
    // carry a level/message/location — presentation (Go to Location, Open panel,
    // Pause and Inspect) is a UI concern layered on top later.
    public sealed class Notification
    {
        public NotificationLevel Level { get; }
        public string Message { get; }
        public District RelatedDistrict { get; }

        public Notification(NotificationLevel level, string message, District relatedDistrict)
        {
            Level = level;
            Message = message;
            RelatedDistrict = relatedDistrict;
        }
    }
}
