namespace CityAuthority.Emergency
{
    // Single "record an event" interface every decision node writes into (11 §2).
    // Step 3 replaces the implementation with the real City Log; call sites don't change.
    public interface IAccountabilityRecorder
    {
        void Record(AccountabilityEvent accountabilityEvent);
    }
}
