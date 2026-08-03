using CityAuthority.Data;
using CityAuthority.Emergency;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class EmergencyScenarioTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void CoverageResolver_ResolvesBandsAtBoundaries()
        {
            var bands = AssetDatabase.LoadAssetAtPath<TravelTimeBandsConfig>("Assets/Data/Defaults/TravelTimeBands_Citywide.asset");

            Assert.AreEqual(CoverageState.Adequate, CoverageResolver.ResolveBand(0f, bands));
            Assert.AreEqual(CoverageState.Adequate, CoverageResolver.ResolveBand(6f, bands));
            Assert.AreEqual(CoverageState.Reduced, CoverageResolver.ResolveBand(6.01f, bands));
            Assert.AreEqual(CoverageState.Reduced, CoverageResolver.ResolveBand(12f, bands));
            Assert.AreEqual(CoverageState.Uncovered, CoverageResolver.ResolveBand(12.01f, bands));
        }

        [Test]
        public void ResponseTimePenalty_MatchesReportDefaults()
        {
            var bands = AssetDatabase.LoadAssetAtPath<TravelTimeBandsConfig>("Assets/Data/Defaults/TravelTimeBands_Citywide.asset");

            Assert.AreEqual(1f, ResponseTimePenalty.SeverityMultiplier(5f, bands));
            Assert.AreEqual(1.25f, ResponseTimePenalty.SeverityMultiplier(8f, bands));
            Assert.AreEqual(1.6f, ResponseTimePenalty.SeverityMultiplier(15f, bands));
        }

        [Test]
        public void EmergencyScenario_SliceInstance_IsWiredAndSized()
        {
            var slice = LoadSlice();
            Assert.IsNotNull(slice.EmergencyScenario, "SliceConfig must reference an EmergencyScenarioConfig");

            var scenario = slice.EmergencyScenario;
            Assert.AreEqual(2, scenario.Districts.Count, "Slice defines exactly the incident district plus one adjacent district");
            Assert.IsNotNull(scenario.Incident);

            var incident = scenario.Incident;
            Assert.IsNotNull(incident.TargetDistrict);
            Assert.IsNotNull(incident.RespondingDepartment);
            Assert.AreEqual(DepartmentType.Fire, incident.RespondingDepartment.DepartmentType, "Fire alone must be sufficient to trigger the coverage tradeoff (09 §7)");
            Assert.IsTrue(incident.LifeSafetyRisk);
            Assert.IsFalse(string.IsNullOrWhiteSpace(incident.WarningMessage));
            Assert.IsFalse(string.IsNullOrWhiteSpace(incident.CriticalMessage));

            var fire = incident.RespondingDepartment;
            var targetTravelTime = CoverageResolver.FindBaseTravelTime(fire, incident.TargetDistrict);
            Assert.IsNotNull(targetTravelTime, "Fire must define a travel time to the incident district");
            Assert.LessOrEqual(targetTravelTime.Value, slice.CitywideTravelTimeBands.AdequateMaxMinutes,
                "09 §7: direct response to the incident location must resolve to Adequate coverage");

            var adjacentDistricts = 0;
            foreach (var district in scenario.Districts)
            {
                if (district == incident.TargetDistrict) continue;
                var travelTime = CoverageResolver.FindBaseTravelTime(fire, district);
                if (travelTime.HasValue) adjacentDistricts++;
            }
            Assert.GreaterOrEqual(adjacentDistricts, 1, "Fire must also cover at least one adjacent district, per 10 §4");
        }

        [Test]
        public void Dispatch_ResolvesAdequateAtTarget_AndDropsAdjacentDistrictToUncovered()
        {
            var slice = LoadSlice();
            var scenario = slice.EmergencyScenario;
            var incident = scenario.Incident;
            var fireState = new DepartmentCoverageState(incident.RespondingDepartment);
            var recorder = new InMemoryAccountabilityRecorder();

            var runtime = new EmergencyIncidentRuntime(incident, fireState, scenario.Districts, slice.CitywideTravelTimeBands, recorder);

            var warning = runtime.RaiseWarning();
            Assert.AreEqual(NotificationLevel.Warning, warning.Level);

            var critical = runtime.EscalateToCritical();
            Assert.AreEqual(NotificationLevel.Critical, critical.Level);

            var result = runtime.RecordActAndDispatch(NotificationLevel.Critical);

            Assert.AreEqual(CoverageState.Adequate, result.TargetDistrictCoverage, "Direct response to the incident location must be Adequate");
            Assert.AreEqual(1f, result.SeverityMultiplier, "Direct response within the Adequate band uses the baseline multiplier");
            Assert.AreEqual(0, fireState.UncommittedUnitCount, "Fire's single unit is now committed");
            Assert.GreaterOrEqual(result.SecondaryNotifications.Count, 1, "Dispatch must produce at least one cross-station Uncovered notification (08 §6)");

            foreach (var secondary in result.SecondaryNotifications)
            {
                Assert.AreNotEqual(incident.TargetDistrict, secondary.RelatedDistrict);
            }

            // 08 §13 item 3: at least one Warning and one Critical notification.
            Assert.IsTrue(recorder.Events.Count >= 4, "Warning, Critical, Act, and dispatch-resolved events must all be recorded");
        }

        [Test]
        public void Respond_RecordsDistinctAccountabilityOutcomesPerResponseType()
        {
            var slice = LoadSlice();
            var scenario = slice.EmergencyScenario;
            var incident = scenario.Incident;

            var ignoreRecorder = new InMemoryAccountabilityRecorder();
            new EmergencyIncidentRuntime(incident, new DepartmentCoverageState(incident.RespondingDepartment), scenario.Districts, slice.CitywideTravelTimeBands, ignoreRecorder)
                .RecordIgnore(NotificationLevel.Warning);
            Assert.AreEqual("IgnoredWarning", ignoreRecorder.Events[0].EventType);

            var ackRecorder = new InMemoryAccountabilityRecorder();
            new EmergencyIncidentRuntime(incident, new DepartmentCoverageState(incident.RespondingDepartment), scenario.Districts, slice.CitywideTravelTimeBands, ackRecorder)
                .RecordAcknowledge(NotificationLevel.Warning);
            Assert.AreEqual("AcknowledgedUnresolved", ackRecorder.Events[0].EventType);

            var actRecorder = new InMemoryAccountabilityRecorder();
            new EmergencyIncidentRuntime(incident, new DepartmentCoverageState(incident.RespondingDepartment), scenario.Districts, slice.CitywideTravelTimeBands, actRecorder)
                .RecordActAndDispatch(NotificationLevel.Warning);
            Assert.AreEqual("ActionTaken", actRecorder.Events[0].EventType);
        }
    }
}
