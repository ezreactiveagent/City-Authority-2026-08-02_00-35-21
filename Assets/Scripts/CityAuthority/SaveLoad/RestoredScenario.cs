using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;

namespace CityAuthority.SaveLoad
{
    // The live runtime objects ScenarioSaveService.Restore reconstructs from a
    // ScenarioSaveData — not itself serializable, just a bundle for the caller
    // (EmergencyDebugPanel) to swap in for its own fields.
    public sealed class RestoredScenario
    {
        public CityLog CityLog { get; }
        public DepartmentCoverageState RespondingDepartmentState { get; }
        public EmergencyIncidentRuntime EmergencyRuntime { get; }
        public bool WarningResponded { get; }
        public bool CriticalResponded { get; }
        public bool StructureCondemned { get; }
        public CondemnationCaseRuntime CourtCase { get; }
        public DevelopmentProposalCycleRuntime DevelopmentCycle { get; }
        public NewspaperCoverageRuntime Newspaper { get; }
        public bool FinalReportGenerated { get; }

        public RestoredScenario(
            CityLog cityLog,
            DepartmentCoverageState respondingDepartmentState,
            EmergencyIncidentRuntime emergencyRuntime,
            bool warningResponded,
            bool criticalResponded,
            bool structureCondemned,
            CondemnationCaseRuntime courtCase,
            DevelopmentProposalCycleRuntime developmentCycle,
            NewspaperCoverageRuntime newspaper,
            bool finalReportGenerated)
        {
            CityLog = cityLog;
            RespondingDepartmentState = respondingDepartmentState;
            EmergencyRuntime = emergencyRuntime;
            WarningResponded = warningResponded;
            CriticalResponded = criticalResponded;
            StructureCondemned = structureCondemned;
            CourtCase = courtCase;
            DevelopmentCycle = developmentCycle;
            Newspaper = newspaper;
            FinalReportGenerated = finalReportGenerated;
        }
    }
}
