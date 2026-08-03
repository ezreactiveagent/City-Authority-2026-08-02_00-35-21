using CityAuthority.Data;
using CityAuthority.Emergency;

namespace CityAuthority.Report
{
    // 08 §10: "a scenario-scoped pass/fail condition tied to the one emergency
    // (e.g., whether the life-safety threat was contained)." Contained means
    // the Critical notification was Acted on and the responding department
    // reached the target district at better than Uncovered coverage; never
    // dispatching, or dispatching too late for real coverage, is a failure.
    public static class ScenarioOutcomeResolver
    {
        public static ScenarioResult Resolve(EmergencyIncidentDefinition incident, DispatchResult dispatchResult)
        {
            if (!incident.LifeSafetyRisk)
            {
                return ScenarioResult.Success;
            }

            if (dispatchResult == null)
            {
                return ScenarioResult.Failure;
            }

            return dispatchResult.TargetDistrictCoverage == CoverageState.Uncovered
                ? ScenarioResult.Failure
                : ScenarioResult.Success;
        }
    }
}
