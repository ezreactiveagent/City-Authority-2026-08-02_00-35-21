using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // Outcome of the Act response's dispatch (11 §2's "Act → unit dispatched →
    // coverage state resolved → response-time severity multiplier applied").
    public sealed class DispatchResult
    {
        public CoverageState TargetDistrictCoverage { get; }
        public float SeverityMultiplier { get; }
        public IReadOnlyList<Notification> SecondaryNotifications { get; }

        public DispatchResult(CoverageState targetDistrictCoverage, float severityMultiplier, IReadOnlyList<Notification> secondaryNotifications)
        {
            TargetDistrictCoverage = targetDistrictCoverage;
            SeverityMultiplier = severityMultiplier;
            SecondaryNotifications = secondaryNotifications;
        }
    }
}
