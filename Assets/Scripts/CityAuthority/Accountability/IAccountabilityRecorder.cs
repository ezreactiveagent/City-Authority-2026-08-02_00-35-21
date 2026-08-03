namespace CityAuthority.Accountability
{
    // Single "record an event" interface every decision node writes into (11 §2).
    // CityLog is the real implementation; call sites depend only on this interface.
    public interface IAccountabilityRecorder
    {
        void Record(AccountabilityEvent accountabilityEvent);
    }
}
