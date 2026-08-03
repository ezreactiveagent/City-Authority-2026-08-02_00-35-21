using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;
using CityAuthority.Report;
using CityAuthority.SaveLoad;
using UnityEngine;

namespace CityAuthority.Session
{
    // UI-agnostic owner of the slice's one live scenario. Constructs every
    // runtime instance from a SliceConfig — what EmergencyDebugPanel.Awake
    // used to do — and holds the session-level state that isn't stored on any
    // individual runtime class (dispatch result, warning/critical response
    // flags, condemnation flag, cached final report). Any UI — the OnGUI debug
    // panel or the UGUI menu — reads and acts through the same ScenarioSession
    // instance (via ScenarioSessionHost) rather than each bootstrapping its
    // own independent copy, so the two can never drift out of sync.
    public sealed class ScenarioSession
    {
        public SliceConfig Config { get; }
        public CityLog CityLog { get; private set; }
        public EmergencyIncidentRuntime EmergencyRuntime { get; private set; }
        public DepartmentCoverageState RespondingDepartmentState { get; private set; }
        public CondemnationCaseRuntime CourtCase { get; private set; }
        public DevelopmentProposalCycleRuntime DevelopmentCycle { get; private set; }
        public NewspaperCoverageRuntime Newspaper { get; private set; }

        public Notification CurrentWarning { get; private set; }
        public Notification CurrentCritical { get; private set; }
        public bool WarningResponded { get; private set; }
        public bool CriticalResponded { get; private set; }
        public DispatchResult LastDispatchResult { get; private set; }
        public bool StructureCondemned { get; private set; }
        public AccountabilityReport FinalReport { get; private set; }

        public bool HasSaveFile => SaveFileIO.Exists(SavePath);

        public ScenarioSession(SliceConfig sliceConfig)
        {
            Config = sliceConfig;

            var incident = sliceConfig.EmergencyScenario.Incident;
            RespondingDepartmentState = new DepartmentCoverageState(incident.RespondingDepartment);
            CityLog = new CityLog();
            EmergencyRuntime = new EmergencyIncidentRuntime(
                incident,
                RespondingDepartmentState,
                sliceConfig.EmergencyScenario.Districts,
                sliceConfig.CitywideTravelTimeBands,
                CityLog);

            if (sliceConfig.CourtCase != null)
            {
                CourtCase = new CondemnationCaseRuntime(sliceConfig.CourtCase, CityLog);
            }

            if (sliceConfig.DevelopmentListing != null)
            {
                DevelopmentCycle = new DevelopmentProposalCycleRuntime(sliceConfig.DevelopmentListing, CityLog);
            }

            if (sliceConfig.Newspaper != null)
            {
                Newspaper = new NewspaperCoverageRuntime(sliceConfig.Newspaper, CityLog);
            }
        }

        public Notification RaiseWarningNow()
        {
            CurrentWarning = EmergencyRuntime.RaiseWarning();
            return CurrentWarning;
        }

        public Notification EscalateNow()
        {
            CurrentCritical = EmergencyRuntime.EscalateToCritical();
            return CurrentCritical;
        }

        public void RespondToWarning(PlayerResponseType response)
        {
            WarningResponded = true;
            Respond(NotificationLevel.Warning, response);
        }

        public void RespondToCritical(PlayerResponseType response)
        {
            CriticalResponded = true;
            Respond(NotificationLevel.Critical, response);
        }

        private void Respond(NotificationLevel level, PlayerResponseType response)
        {
            switch (response)
            {
                case PlayerResponseType.Act:
                    LastDispatchResult = EmergencyRuntime.RecordActAndDispatch(level);
                    break;
                case PlayerResponseType.Acknowledge:
                    EmergencyRuntime.RecordAcknowledge(level);
                    break;
                case PlayerResponseType.Ignore:
                    EmergencyRuntime.RecordIgnore(level);
                    break;
            }
        }

        // 02 §13: the Emergency Commander condemns the structure once the
        // life-safety threat is confirmed, without prior player approval.
        public void CondemnStructure()
        {
            if (StructureCondemned)
            {
                return;
            }

            StructureCondemned = true;
            CityLog.Record(new AccountabilityEvent(
                "StructureCondemned",
                $"{Config.CourtCase.DisplayName} condemned by emergency order; payment responsibility disputed in Court.",
                Config.CourtCase.TargetDistrict));
        }

        public CourtRulingRecord IssueRuling()
        {
            return CourtCase.IssueRuling();
        }

        // 08 §10, 08 §13 item 7: assembled once, on demand, from the City Log
        // plus whatever court ruling and newspaper coverage already exist —
        // neither is required, since a scenario can end without the emergency
        // ever escalating to condemnation or coverage.
        public AccountabilityReport GenerateFinalReport()
        {
            if (FinalReport != null)
            {
                return FinalReport;
            }

            FinalReport = ComputeFinalReport();
            CityLog.Record(new AccountabilityEvent("FinalReportGenerated", $"Scenario ended: {FinalReport.Outcome}. {FinalReport.Summary}"));
            return FinalReport;
        }

        // Split out so LoadScenario can recompute the report after a reload
        // without appending a second "FinalReportGenerated" log entry — the
        // restored log already carries the original one. Pure function of
        // already-restored data, so recomputing is safe (06 §9 only forbids
        // re-generating the court ruling itself, not this read of it).
        private AccountabilityReport ComputeFinalReport()
        {
            var incident = Config.EmergencyScenario.Incident;
            var outcome = ScenarioOutcomeResolver.Resolve(incident, LastDispatchResult);
            var ruling = CourtCase?.Ruling;
            var articles = Newspaper?.PublishedArticles ?? System.Array.Empty<NewsArticle>();
            return FinalReportGenerator.Generate(CityLog, outcome, ruling, articles);
        }

        public void ApproveDevelopmentProposal(DevelopmentProposal proposal)
        {
            DevelopmentCycle.ApproveProposal(proposal);
        }

        public void RejectDevelopmentProposals()
        {
            DevelopmentCycle.RejectBoth();
        }

        public NewsArticle PublishEmergencyResponseStory()
        {
            return Newspaper.PublishEmergencyResponseStory(Config.EmergencyScenario.Incident, LastDispatchResult);
        }

        public NewsArticle PublishCourtRulingStory()
        {
            return Newspaper.PublishCourtRulingStory(Config.CourtCase, CourtCase.Ruling);
        }

        public NewsArticle PublishDevelopmentRejectionStory()
        {
            return Newspaper.PublishDevelopmentRejectionStory(Config.DevelopmentListing, DevelopmentCycle.DeveloperInterestScore);
        }

        // 08 §13 item 8: writes the full scenario state to disk as JSON.
        private static string SavePath => System.IO.Path.Combine(Application.persistentDataPath, "scenario_save.json");

        public void SaveScenario()
        {
            var data = ScenarioSaveService.Capture(
                CityLog, EmergencyRuntime, RespondingDepartmentState, WarningResponded, CriticalResponded, StructureCondemned,
                CourtCase, DevelopmentCycle, Newspaper, FinalReport != null);
            SaveFileIO.Save(SavePath, data);
        }

        // Restores every runtime instance from the save file rather than
        // mutating the existing ones in place — the court ruling, published
        // articles, and log entries all come back as stored data, never
        // re-simulated (06 §9).
        public void LoadScenario()
        {
            if (!SaveFileIO.Exists(SavePath))
            {
                return;
            }

            var data = SaveFileIO.Load(SavePath);
            var restored = ScenarioSaveService.Restore(data, Config);

            CityLog = restored.CityLog;
            RespondingDepartmentState = restored.RespondingDepartmentState;
            EmergencyRuntime = restored.EmergencyRuntime;
            WarningResponded = restored.WarningResponded;
            CriticalResponded = restored.CriticalResponded;
            StructureCondemned = restored.StructureCondemned;
            CourtCase = restored.CourtCase;
            DevelopmentCycle = restored.DevelopmentCycle;
            Newspaper = restored.Newspaper;
            LastDispatchResult = EmergencyRuntime.DispatchResultIfAny;

            var incident = Config.EmergencyScenario.Incident;
            CurrentWarning = EmergencyRuntime.WarningRaised
                ? new Notification(NotificationLevel.Warning, incident.WarningMessage, incident.TargetDistrict)
                : null;
            CurrentCritical = EmergencyRuntime.CriticalRaised
                ? new Notification(NotificationLevel.Critical, incident.CriticalMessage, incident.TargetDistrict)
                : null;

            FinalReport = restored.FinalReportGenerated ? ComputeFinalReport() : null;
        }
    }
}
