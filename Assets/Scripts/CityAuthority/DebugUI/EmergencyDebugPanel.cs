using System.Text;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;
using CityAuthority.Report;
using UnityEngine;

namespace CityAuthority.DebugUI
{
    // Bare-bones IMGUI panel for driving the Step 2 scripted emergency by hand in
    // Play mode. No styling, no prefabs — a debugging aid, not a game screen.
    public sealed class EmergencyDebugPanel : MonoBehaviour
    {
        [SerializeField] private SliceConfig sliceConfig;

        private EmergencyIncidentRuntime runtime;
        private DepartmentCoverageState fireState;
        private CityLog cityLog;
        private Notification currentWarning;
        private Notification currentCritical;
        private bool warningResponded;
        private bool criticalResponded;
        private DispatchResult dispatchResult;
        private CondemnationCaseRuntime courtCase;
        private bool structureCondemned;
        private DevelopmentProposalCycleRuntime developmentCycle;
        private NewspaperCoverageRuntime newspaper;
        private AccountabilityReport finalReport;
        private Vector2 logScroll;
        private Rect windowRect = new(20, 20, 440, 860);

        public DispatchResult LastDispatchResult => dispatchResult;
        public CourtRulingRecord LastRuling => courtCase?.Ruling;
        public DevelopmentProposalCycleRuntime DevelopmentCycle => developmentCycle;
        public NewspaperCoverageRuntime Newspaper => newspaper;
        public AccountabilityReport FinalReport => finalReport;
        public System.Collections.Generic.IReadOnlyList<AccountabilityEvent> Events => cityLog?.Events;

        private void Awake()
        {
            if (sliceConfig == null || sliceConfig.EmergencyScenario == null)
            {
                Debug.LogError("EmergencyDebugPanel: SliceConfig with an EmergencyScenario must be assigned.");
                enabled = false;
                return;
            }

            var incident = sliceConfig.EmergencyScenario.Incident;
            fireState = new DepartmentCoverageState(incident.RespondingDepartment);
            cityLog = new CityLog();
            runtime = new EmergencyIncidentRuntime(
                incident,
                fireState,
                sliceConfig.EmergencyScenario.Districts,
                sliceConfig.CitywideTravelTimeBands,
                cityLog);

            if (sliceConfig.CourtCase != null)
            {
                courtCase = new CondemnationCaseRuntime(sliceConfig.CourtCase, cityLog);
            }

            if (sliceConfig.DevelopmentListing != null)
            {
                developmentCycle = new DevelopmentProposalCycleRuntime(sliceConfig.DevelopmentListing, cityLog);
            }

            if (sliceConfig.Newspaper != null)
            {
                newspaper = new NewspaperCoverageRuntime(sliceConfig.Newspaper, cityLog);
            }
        }

        public NewsArticle PublishEmergencyResponseStory()
        {
            return newspaper.PublishEmergencyResponseStory(sliceConfig.EmergencyScenario.Incident, dispatchResult);
        }

        public NewsArticle PublishCourtRulingStory()
        {
            return newspaper.PublishCourtRulingStory(sliceConfig.CourtCase, courtCase.Ruling);
        }

        public NewsArticle PublishDevelopmentRejectionStory()
        {
            return newspaper.PublishDevelopmentRejectionStory(sliceConfig.DevelopmentListing, developmentCycle.DeveloperInterestScore);
        }

        public void ApproveDevelopmentProposal(DevelopmentProposal proposal)
        {
            developmentCycle.ApproveProposal(proposal);
        }

        public void RejectDevelopmentProposals()
        {
            developmentCycle.RejectBoth();
        }

        // 02 §13: the Emergency Commander condemns the structure once the
        // life-safety threat is confirmed, without prior player approval.
        public void CondemnStructure()
        {
            if (structureCondemned)
            {
                return;
            }

            structureCondemned = true;
            cityLog.Record(new AccountabilityEvent(
                "StructureCondemned",
                $"{sliceConfig.CourtCase.DisplayName} condemned by emergency order; payment responsibility disputed in Court.",
                sliceConfig.CourtCase.TargetDistrict));
        }

        public CourtRulingRecord IssueRuling()
        {
            return courtCase.IssueRuling();
        }

        // 08 §10, 08 §13 item 7: assembled once, on demand, from the City Log
        // plus whatever court ruling and newspaper coverage already exist —
        // neither is required, since a scenario can end without the emergency
        // ever escalating to condemnation or coverage.
        public AccountabilityReport GenerateFinalReport()
        {
            if (finalReport != null)
            {
                return finalReport;
            }

            var incident = sliceConfig.EmergencyScenario.Incident;
            var outcome = ScenarioOutcomeResolver.Resolve(incident, dispatchResult);
            var ruling = courtCase?.Ruling;
            var articles = newspaper?.PublishedArticles ?? System.Array.Empty<NewsArticle>();

            finalReport = FinalReportGenerator.Generate(cityLog, outcome, ruling, articles);
            cityLog.Record(new AccountabilityEvent("FinalReportGenerated", $"Scenario ended: {outcome}. {finalReport.Summary}"));
            return finalReport;
        }

        public void RaiseWarningNow()
        {
            currentWarning = runtime.RaiseWarning();
        }

        public void EscalateNow()
        {
            currentCritical = runtime.EscalateToCritical();
        }

        public void RespondToWarning(PlayerResponseType response)
        {
            warningResponded = true;
            Respond(NotificationLevel.Warning, response);
        }

        public void RespondToCritical(PlayerResponseType response)
        {
            criticalResponded = true;
            Respond(NotificationLevel.Critical, response);
        }

        private void Respond(NotificationLevel level, PlayerResponseType response)
        {
            switch (response)
            {
                case PlayerResponseType.Act:
                    dispatchResult = runtime.RecordActAndDispatch(level);
                    break;
                case PlayerResponseType.Acknowledge:
                    runtime.RecordAcknowledge(level);
                    break;
                case PlayerResponseType.Ignore:
                    runtime.RecordIgnore(level);
                    break;
            }
        }

        private void OnGUI()
        {
            if (runtime == null)
            {
                return;
            }

            windowRect = GUILayout.Window(GetHashCode(), windowRect, DrawWindow, "Emergency Scenario Debug Panel");
        }

        private void DrawWindow(int id)
        {
            var incident = sliceConfig.EmergencyScenario.Incident;

            if (developmentCycle != null)
            {
                DrawDevelopmentListingSection();
                GUILayout.Space(10);
            }

            GUILayout.Label(incident.DisplayName, EditorBoldLabel());
            GUILayout.Space(4);

            if (!runtime.WarningRaised)
            {
                if (GUILayout.Button("Raise Warning"))
                {
                    RaiseWarningNow();
                }
            }
            else
            {
                GUILayout.Label("[Warning] " + currentWarning.Message);
                if (!warningResponded)
                {
                    DrawResponseButtons(RespondToWarning);
                }
                else
                {
                    GUILayout.Label("Warning response recorded.");
                }
            }

            GUILayout.Space(6);

            if (runtime.WarningRaised && !runtime.CriticalRaised)
            {
                if (GUILayout.Button("Escalate to Critical"))
                {
                    EscalateNow();
                }
            }
            else if (runtime.CriticalRaised)
            {
                GUILayout.Label("[Critical] " + currentCritical.Message);
                if (runtime.HasDispatched)
                {
                    GUILayout.Label("Already dispatched — no further response needed.");
                }
                else if (!criticalResponded)
                {
                    DrawResponseButtons(RespondToCritical);
                }
                else
                {
                    GUILayout.Label("Critical response recorded.");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Fire uncommitted units: " + fireState.UncommittedUnitCount + " / " + fireState.TotalUnitCount);

            foreach (var district in sliceConfig.EmergencyScenario.Districts)
            {
                var coverage = CoverageResolver.ResolveDistrictCoverage(fireState, district, 0, sliceConfig.CitywideTravelTimeBands);
                GUILayout.Label(district.DisplayName + ": " + coverage);
            }

            if (dispatchResult != null)
            {
                GUILayout.Space(6);
                GUILayout.Label("Dispatch: " + dispatchResult.TargetDistrictCoverage + " coverage, " + dispatchResult.SeverityMultiplier + "x severity");
                foreach (var secondary in dispatchResult.SecondaryNotifications)
                {
                    GUILayout.Label("  [" + secondary.Level + "] " + secondary.Message);
                }
            }

            if (courtCase != null)
            {
                DrawCourtCaseSection();
            }

            if (newspaper != null)
            {
                DrawNewspaperSection();
            }

            if (criticalResponded)
            {
                DrawFinalReportSection();
            }

            GUILayout.Space(10);
            GUILayout.Label("By category: " +
                "Ignored=" + cityLog.CountByCategory(AccountabilityCategory.IgnoredWarning) +
                ", Acknowledged (unresolved)=" + cityLog.CountByCategory(AccountabilityCategory.AcknowledgedUnresolved) +
                ", Actions completed=" + cityLog.CountByCategory(AccountabilityCategory.ActionCompleted));

            GUILayout.Space(6);
            GUILayout.Label("City Log (" + cityLog.Events.Count + ")");
            logScroll = GUILayout.BeginScrollView(logScroll, GUILayout.Height(160));
            var sb = new StringBuilder();
            foreach (var evt in cityLog.Events)
            {
                sb.Append('[').Append(evt.EventType);
                if (evt.Category.HasValue)
                {
                    sb.Append('/').Append(evt.Category.Value);
                }
                sb.Append("] ").Append(evt.Summary).Append('\n');
            }
            GUILayout.Label(sb.ToString());
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void DrawDevelopmentListingSection()
        {
            var listing = sliceConfig.DevelopmentListing;

            GUILayout.Label("Development Listing", EditorBoldLabel());
            GUILayout.Label(listing.DisplayName + " (" + listing.Zoning + ")");

            if (!developmentCycle.IsResolved)
            {
                foreach (var proposal in listing.Proposals)
                {
                    GUILayout.Label(
                        proposal.DeveloperName + " — " + proposal.Density + " density, " +
                        proposal.TargetIncomeBand + " income, $" + proposal.EstimatedAnnualTaxRevenue.ToString("N0") +
                        "/yr tax, " + proposal.Risk + " risk");
                    GUILayout.Label(proposal.Description, GUI.skin.box);
                    if (GUILayout.Button("Approve " + proposal.DeveloperName))
                    {
                        ApproveDevelopmentProposal(proposal);
                    }
                    GUILayout.Space(4);
                }

                if (GUILayout.Button("Reject Both Proposals"))
                {
                    RejectDevelopmentProposals();
                }
            }
            else if (developmentCycle.ApprovedProposal != null)
            {
                GUILayout.Label("Approved: " + developmentCycle.ApprovedProposal.DeveloperName + " (binding).");
            }
            else
            {
                GUILayout.Label("Rejected. Developer interest: " + developmentCycle.DeveloperInterestScore.ToString("N0") + " / 100");
            }
        }

        private void DrawCourtCaseSection()
        {
            var courtCaseDefinition = sliceConfig.CourtCase;

            GUILayout.Space(10);
            GUILayout.Label("Court Case", EditorBoldLabel());

            if (!runtime.CriticalRaised || !sliceConfig.EmergencyScenario.Incident.LifeSafetyRisk)
            {
                GUILayout.Label("(No condemnation grounds yet.)");
                return;
            }

            if (!structureCondemned)
            {
                if (GUILayout.Button("Condemn Structure & Open Court Case"))
                {
                    CondemnStructure();
                }
                return;
            }

            GUILayout.Label(courtCaseDefinition.DisplayName + " — assessed value $" + courtCaseDefinition.AssessedValue.ToString("N0"));
            GUILayout.Label("Judge: " + courtCaseDefinition.AssignedJudge.JudgeName + " (" + courtCaseDefinition.AssignedJudge.PersonalityTag + ")");

            GUILayout.Label("Claimants:");
            foreach (var claimant in courtCaseDefinition.Claimants)
            {
                GUILayout.Label("  " + claimant.CitizenName + " (" + claimant.HousingStatus + ")");
            }

            if (!courtCase.HasRuling)
            {
                if (GUILayout.Button("Issue Ruling"))
                {
                    IssueRuling();
                }
                return;
            }

            var ruling = courtCase.Ruling;
            GUILayout.Space(4);
            GUILayout.Label("Ruling: " + ruling.SelectedOutcome);
            GUILayout.Label("City pays $" + ruling.CityAmount.ToString("N0") + ", Owner pays $" + ruling.OwnerAmount.ToString("N0"));
            GUILayout.Label(ruling.Explanation, GUI.skin.box);
        }

        private void DrawNewspaperSection()
        {
            GUILayout.Space(10);
            GUILayout.Label(sliceConfig.Newspaper.DisplayName, EditorBoldLabel());

            if (criticalResponded && FindArticle("EmergencyResponse") == null)
            {
                if (GUILayout.Button("Publish Emergency Response Story"))
                {
                    PublishEmergencyResponseStory();
                }
            }

            if (courtCase != null && courtCase.HasRuling && FindArticle("CourtRuling") == null)
            {
                if (GUILayout.Button("Publish Court Ruling Story"))
                {
                    PublishCourtRulingStory();
                }
            }

            if (developmentCycle != null && developmentCycle.WasRejected && FindArticle("DevelopmentRejection") == null)
            {
                if (GUILayout.Button("Publish Development Rejection Story"))
                {
                    PublishDevelopmentRejectionStory();
                }
            }

            foreach (var article in newspaper.PublishedArticles)
            {
                GUILayout.Space(4);
                GUILayout.Label(article.Headline, EditorBoldLabel());
                GUILayout.Label(article.Body, GUI.skin.box);
            }
        }

        private void DrawFinalReportSection()
        {
            GUILayout.Space(10);
            GUILayout.Label("Final Report", EditorBoldLabel());

            if (finalReport == null)
            {
                if (GUILayout.Button("Generate Final Report"))
                {
                    GenerateFinalReport();
                }
                return;
            }

            GUILayout.Label("Outcome: " + finalReport.Outcome, EditorBoldLabel());
            GUILayout.Label(finalReport.Summary, GUI.skin.box);
            GUILayout.Label("Warnings issued: " + finalReport.WarningsIssued.Count);
            GUILayout.Label("Acknowledged (unresolved): " + finalReport.AcknowledgedUnresolved.Count);
            GUILayout.Label("Ignored: " + finalReport.IgnoredWarnings.Count);
            GUILayout.Label("Actions taken: " + finalReport.ActionsTaken.Count);
            GUILayout.Label("Court ruling: " + (finalReport.CourtRuling != null ? finalReport.CourtRuling.SelectedOutcome.ToString() : "(none)"));
            GUILayout.Label("Media coverage: " + finalReport.MediaCoverage.Count + " article(s)");
        }

        private NewsArticle FindArticle(string sourceEventType)
        {
            foreach (var article in newspaper.PublishedArticles)
            {
                if (article.SourceEventType == sourceEventType)
                {
                    return article;
                }
            }
            return null;
        }

        private static void DrawResponseButtons(System.Action<PlayerResponseType> respond)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Act")) respond(PlayerResponseType.Act);
            if (GUILayout.Button("Acknowledge")) respond(PlayerResponseType.Acknowledge);
            if (GUILayout.Button("Ignore")) respond(PlayerResponseType.Ignore);
            GUILayout.EndHorizontal();
        }

        private static GUIStyle boldLabelStyle;
        private static GUIStyle EditorBoldLabel()
        {
            return boldLabelStyle ??= new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
        }
    }
}
