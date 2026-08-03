using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // Orchestrates the slice's one scripted incident (08 §6): raises Warning/Critical
    // notifications, records the player's Act/Acknowledge/Ignore response (03 §6), and
    // — for Act — dispatches the responding department and resolves the resulting
    // coverage tradeoff (09 §6-7). No mutual aid, reassignment cancellation, or
    // Incident Commander authority, per 08 §6's explicit exclusions.
    public sealed class EmergencyIncidentRuntime
    {
        private readonly EmergencyIncidentDefinition definition;
        private readonly DepartmentCoverageState respondingDepartmentState;
        private readonly IReadOnlyList<District> allDistricts;
        private readonly TravelTimeBandsConfig bands;
        private readonly IAccountabilityRecorder recorder;

        private bool warningRaised;
        private bool criticalRaised;
        private DispatchResult dispatchResult;

        public EmergencyIncidentRuntime(
            EmergencyIncidentDefinition definition,
            DepartmentCoverageState respondingDepartmentState,
            IReadOnlyList<District> allDistricts,
            TravelTimeBandsConfig bands,
            IAccountabilityRecorder recorder)
        {
            this.definition = definition;
            this.respondingDepartmentState = respondingDepartmentState;
            this.allDistricts = allDistricts;
            this.bands = bands;
            this.recorder = recorder;
        }

        public bool HasDispatched => dispatchResult != null;
        public bool WarningRaised => warningRaised;
        public bool CriticalRaised => criticalRaised;

        public Notification RaiseWarning()
        {
            warningRaised = true;
            recorder.Record(new AccountabilityEvent(
                "EmergencyWarningRaised", definition.WarningMessage, definition.TargetDistrict, NotificationLevel.Warning));
            return new Notification(NotificationLevel.Warning, definition.WarningMessage, definition.TargetDistrict);
        }

        public Notification EscalateToCritical()
        {
            criticalRaised = true;
            recorder.Record(new AccountabilityEvent(
                "EmergencyCriticalRaised", definition.CriticalMessage, definition.TargetDistrict, NotificationLevel.Critical));
            return new Notification(NotificationLevel.Critical, definition.CriticalMessage, definition.TargetDistrict);
        }

        // 03 §6: acknowledging without resolving can create greater accountability
        // than never having seen the issue — recorded distinctly from Ignore.
        public void RecordAcknowledge(NotificationLevel level)
        {
            recorder.Record(new AccountabilityEvent(
                "AcknowledgedUnresolved",
                $"{level} notification for {definition.TargetDistrict.DisplayName} acknowledged but not resolved.",
                definition.TargetDistrict,
                level,
                AccountabilityCategory.AcknowledgedUnresolved));
        }

        public void RecordIgnore(NotificationLevel level)
        {
            recorder.Record(new AccountabilityEvent(
                "IgnoredWarning",
                $"{level} notification for {definition.TargetDistrict.DisplayName} was ignored.",
                definition.TargetDistrict,
                level,
                AccountabilityCategory.IgnoredWarning));
        }

        // Act → dispatch the responding department's unit, resolve the target
        // district's coverage/severity, and recheck cross-station exposure (09 §6)
        // on every other district the department also covers.
        public DispatchResult RecordActAndDispatch(NotificationLevel level)
        {
            if (dispatchResult != null)
            {
                return dispatchResult;
            }

            recorder.Record(new AccountabilityEvent(
                "ActionTaken",
                $"Responded to {level} notification for {definition.TargetDistrict.DisplayName} by dispatching {definition.RespondingDepartment.DisplayName}.",
                definition.TargetDistrict,
                level,
                AccountabilityCategory.ActionCompleted));

            var department = definition.RespondingDepartment;
            var travelToTarget = CoverageResolver.FindBaseTravelTime(department, definition.TargetDistrict);
            var targetCoverage = travelToTarget.HasValue
                ? CoverageResolver.ResolveBand(travelToTarget.Value, bands)
                : CoverageState.Uncovered;
            var severityMultiplier = travelToTarget.HasValue
                ? ResponseTimePenalty.SeverityMultiplier(travelToTarget.Value, bands)
                : ResponseTimePenalty.UncoveredBandMultiplier;

            respondingDepartmentState.CommitUnit();

            recorder.Record(new AccountabilityEvent(
                "DispatchResolved",
                $"{department.DisplayName} arrived at {definition.TargetDistrict.DisplayName}: {targetCoverage} coverage, {severityMultiplier}x severity multiplier.",
                definition.TargetDistrict,
                level));

            var secondaryNotifications = new List<Notification>();
            foreach (var district in allDistricts)
            {
                if (district == definition.TargetDistrict)
                {
                    continue;
                }

                if (CoverageResolver.FindBaseTravelTime(department, district) == null)
                {
                    continue;
                }

                var recalculated = CoverageResolver.ResolveDistrictCoverage(respondingDepartmentState, district, concurrentOpenCalls: 0, bands);
                if (recalculated == CoverageState.Uncovered)
                {
                    var message = $"{district.DisplayName} dropped to Uncovered coverage after {department.DisplayName}'s unit was reassigned.";
                    secondaryNotifications.Add(new Notification(NotificationLevel.Warning, message, district));
                    recorder.Record(new AccountabilityEvent("CrossStationExposureUncovered", message, district));
                }
            }

            dispatchResult = new DispatchResult(targetCoverage, severityMultiplier, secondaryNotifications);
            return dispatchResult;
        }
    }
}
