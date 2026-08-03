using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.Accountability
{
    // The City Log (03 §5): every notification and player response, in the order
    // they occurred, queryable the way the eventual accountability/failure report
    // (05 §13, 01 §6, 08 §10) needs to break them down. Every later slice system
    // (court, development, newspaper) writes into the same shared instance.
    public sealed class CityLog : IAccountabilityRecorder
    {
        private readonly List<AccountabilityEvent> events = new();

        public IReadOnlyList<AccountabilityEvent> Events => events;

        public void Record(AccountabilityEvent accountabilityEvent)
        {
            events.Add(accountabilityEvent);
        }

        public IEnumerable<AccountabilityEvent> ByCategory(AccountabilityCategory category)
        {
            foreach (var evt in events)
            {
                if (evt.Category == category)
                {
                    yield return evt;
                }
            }
        }

        public IEnumerable<AccountabilityEvent> ByDistrict(District district)
        {
            foreach (var evt in events)
            {
                if (evt.RelatedDistrict == district)
                {
                    yield return evt;
                }
            }
        }

        public IEnumerable<AccountabilityEvent> ByLevel(NotificationLevel level)
        {
            foreach (var evt in events)
            {
                if (evt.RelatedLevel == level)
                {
                    yield return evt;
                }
            }
        }

        public int CountByCategory(AccountabilityCategory category)
        {
            var count = 0;
            foreach (var evt in events)
            {
                if (evt.Category == category)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
