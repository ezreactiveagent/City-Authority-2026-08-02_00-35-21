using System.Text;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;
using CityAuthority.Report;
using CityAuthority.Session;
using UnityEngine;

namespace CityAuthority.DebugUI
{
    // Bare-bones IMGUI panel for driving the scripted emergency by hand in
    // Play mode. No styling, no prefabs — a debugging aid, not a game screen.
    // Reads/acts through a shared ScenarioSessionHost rather than owning its
    // own scenario state, so it can run alongside the real UGUI menu without
    // the two drifting out of sync.
    public sealed class EmergencyDebugPanel : MonoBehaviour
    {
        [SerializeField] private ScenarioSessionHost sessionHost;

        private ScenarioSession session;
        private Vector2 logScroll;
        private Rect windowRect = new(480, 20, 440, 860);

        public DispatchResult LastDispatchResult => session?.LastDispatchResult;
        public CourtRulingRecord LastRuling => session?.CourtCase?.Ruling;
        public DevelopmentProposalCycleRuntime DevelopmentCycle => session?.DevelopmentCycle;
        public NewspaperCoverageRuntime Newspaper => session?.Newspaper;
        public AccountabilityReport FinalReport => session?.FinalReport;
        public System.Collections.Generic.IReadOnlyList<AccountabilityEvent> Events => session?.CityLog?.Events;

        // Start, not Awake: ScenarioSessionHost.Awake() is what constructs the
        // shared Session, and Unity doesn't guarantee Awake order across
        // different GameObjects — Start runs after every Awake in the scene.
        private void Start()
        {
            if (sessionHost == null || sessionHost.Session == null)
            {
                Debug.LogError("EmergencyDebugPanel: a ScenarioSessionHost with a ready Session must be assigned.");
                enabled = false;
                return;
            }

            session = sessionHost.Session;
        }

        public NewsArticle PublishEmergencyResponseStory() => session.PublishEmergencyResponseStory();

        public NewsArticle PublishCourtRulingStory() => session.PublishCourtRulingStory();

        public NewsArticle PublishDevelopmentRejectionStory() => session.PublishDevelopmentRejectionStory();

        public void ApproveDevelopmentProposal(DevelopmentProposal proposal) => session.ApproveDevelopmentProposal(proposal);

        public void RejectDevelopmentProposals() => session.RejectDevelopmentProposals();

        public void CondemnStructure() => session.CondemnStructure();

        public CourtRulingRecord IssueRuling() => session.IssueRuling();

        public AccountabilityReport GenerateFinalReport() => session.GenerateFinalReport();

        public bool HasSaveFile => session != null && session.HasSaveFile;

        public void SaveScenario() => session.SaveScenario();

        public void LoadScenario() => session.LoadScenario();

        public void RaiseWarningNow() => session.RaiseWarningNow();

        public void EscalateNow() => session.EscalateNow();

        public void RespondToWarning(PlayerResponseType response) => session.RespondToWarning(response);

        public void RespondToCritical(PlayerResponseType response) => session.RespondToCritical(response);

        private void OnGUI()
        {
            if (session == null)
            {
                return;
            }

            windowRect = GUILayout.Window(GetHashCode(), windowRect, DrawWindow, "Emergency Scenario Debug Panel");
        }

        private void DrawWindow(int id)
        {
            var incident = session.Config.EmergencyScenario.Incident;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                SaveScenario();
            }
            GUI.enabled = HasSaveFile;
            if (GUILayout.Button("Load"))
            {
                LoadScenario();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(6);

            if (session.DevelopmentCycle != null)
            {
                DrawDevelopmentListingSection();
                GUILayout.Space(10);
            }

            GUILayout.Label(incident.DisplayName, EditorBoldLabel());
            GUILayout.Space(4);

            if (!session.EmergencyRuntime.WarningRaised)
            {
                if (GUILayout.Button("Raise Warning"))
                {
                    RaiseWarningNow();
                }
            }
            else
            {
                GUILayout.Label("[Warning] " + session.CurrentWarning.Message);
                if (!session.WarningResponded)
                {
                    DrawResponseButtons(RespondToWarning);
                }
                else
                {
                    GUILayout.Label("Warning response recorded.");
                }
            }

            GUILayout.Space(6);

            if (session.EmergencyRuntime.WarningRaised && !session.EmergencyRuntime.CriticalRaised)
            {
                if (GUILayout.Button("Escalate to Critical"))
                {
                    EscalateNow();
                }
            }
            else if (session.EmergencyRuntime.CriticalRaised)
            {
                GUILayout.Label("[Critical] " + session.CurrentCritical.Message);
                if (session.EmergencyRuntime.HasDispatched)
                {
                    GUILayout.Label("Already dispatched — no further response needed.");
                }
                else if (!session.CriticalResponded)
                {
                    DrawResponseButtons(RespondToCritical);
                }
                else
                {
                    GUILayout.Label("Critical response recorded.");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Fire uncommitted units: " + session.RespondingDepartmentState.UncommittedUnitCount + " / " + session.RespondingDepartmentState.TotalUnitCount);

            foreach (var district in session.Config.EmergencyScenario.Districts)
            {
                var coverage = CoverageResolver.ResolveDistrictCoverage(session.RespondingDepartmentState, district, 0, session.Config.CitywideTravelTimeBands);
                GUILayout.Label(district.DisplayName + ": " + coverage);
            }

            if (session.LastDispatchResult != null)
            {
                GUILayout.Space(6);
                GUILayout.Label("Dispatch: " + session.LastDispatchResult.TargetDistrictCoverage + " coverage, " + session.LastDispatchResult.SeverityMultiplier + "x severity");
                foreach (var secondary in session.LastDispatchResult.SecondaryNotifications)
                {
                    GUILayout.Label("  [" + secondary.Level + "] " + secondary.Message);
                }
            }

            if (session.CourtCase != null)
            {
                DrawCourtCaseSection();
            }

            if (session.Newspaper != null)
            {
                DrawNewspaperSection();
            }

            if (session.CriticalResponded)
            {
                DrawFinalReportSection();
            }

            GUILayout.Space(10);
            GUILayout.Label("By category: " +
                "Ignored=" + session.CityLog.CountByCategory(AccountabilityCategory.IgnoredWarning) +
                ", Acknowledged (unresolved)=" + session.CityLog.CountByCategory(AccountabilityCategory.AcknowledgedUnresolved) +
                ", Actions completed=" + session.CityLog.CountByCategory(AccountabilityCategory.ActionCompleted));

            GUILayout.Space(6);
            GUILayout.Label("City Log (" + session.CityLog.Events.Count + ")");
            logScroll = GUILayout.BeginScrollView(logScroll, GUILayout.Height(160));
            var sb = new StringBuilder();
            foreach (var evt in session.CityLog.Events)
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
            var listing = session.Config.DevelopmentListing;
            var developmentCycle = session.DevelopmentCycle;

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
            var courtCaseDefinition = session.Config.CourtCase;
            var courtCase = session.CourtCase;

            GUILayout.Space(10);
            GUILayout.Label("Court Case", EditorBoldLabel());

            if (!session.EmergencyRuntime.CriticalRaised || !session.Config.EmergencyScenario.Incident.LifeSafetyRisk)
            {
                GUILayout.Label("(No condemnation grounds yet.)");
                return;
            }

            if (!session.StructureCondemned)
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
            var newspaper = session.Newspaper;

            GUILayout.Space(10);
            GUILayout.Label(session.Config.Newspaper.DisplayName, EditorBoldLabel());

            if (session.CriticalResponded && FindArticle("EmergencyResponse") == null)
            {
                if (GUILayout.Button("Publish Emergency Response Story"))
                {
                    PublishEmergencyResponseStory();
                }
            }

            if (session.CourtCase != null && session.CourtCase.HasRuling && FindArticle("CourtRuling") == null)
            {
                if (GUILayout.Button("Publish Court Ruling Story"))
                {
                    PublishCourtRulingStory();
                }
            }

            if (session.DevelopmentCycle != null && session.DevelopmentCycle.WasRejected && FindArticle("DevelopmentRejection") == null)
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

            var finalReport = session.FinalReport;
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
            foreach (var article in session.Newspaper.PublishedArticles)
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
