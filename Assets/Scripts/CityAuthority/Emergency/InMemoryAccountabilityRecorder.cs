using System.Collections.Generic;

namespace CityAuthority.Emergency
{
    // Default IAccountabilityRecorder until the real City Log (Step 3) exists.
    public sealed class InMemoryAccountabilityRecorder : IAccountabilityRecorder
    {
        private readonly List<AccountabilityEvent> events = new();

        public IReadOnlyList<AccountabilityEvent> Events => events;

        public void Record(AccountabilityEvent accountabilityEvent)
        {
            events.Add(accountabilityEvent);
        }
    }
}
