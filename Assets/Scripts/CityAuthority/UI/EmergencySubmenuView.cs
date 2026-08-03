using System.Text;
using CityAuthority.Data;
using CityAuthority.Emergency;
using CityAuthority.Session;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CityAuthority.UI
{
    // The one fully-wired submenu proving the menu pattern against real game
    // state: raise -> escalate -> Act/Acknowledge/Ignore -> dispatch. Mirrors
    // EmergencyDebugPanel's incident/response/coverage section only (not its
    // Court/Development/Newspaper/Report sections) and, deliberately, its
    // exact gating rules -- including that escalating doesn't require having
    // responded to the warning first -- so the two UIs never disagree about
    // what's currently allowed.
    public sealed class EmergencySubmenuView : MonoBehaviour
    {
        [SerializeField] private ScenarioSessionHost sessionHost;
        [SerializeField] private TMP_Text incidentNameText;
        [SerializeField] private TMP_Text warningStatusText;
        [SerializeField] private TMP_Text criticalStatusText;
        [SerializeField] private TMP_Text coverageText;
        [SerializeField] private Button raiseWarningButton;
        [SerializeField] private Button escalateButton;
        [SerializeField] private Button warningActButton;
        [SerializeField] private Button warningAcknowledgeButton;
        [SerializeField] private Button warningIgnoreButton;
        [SerializeField] private Button criticalActButton;
        [SerializeField] private Button criticalAcknowledgeButton;
        [SerializeField] private Button criticalIgnoreButton;

        private ScenarioSession session;

        private void Start()
        {
            session = sessionHost.Session;

            raiseWarningButton.onClick.AddListener(() => session.RaiseWarningNow());
            escalateButton.onClick.AddListener(() => session.EscalateNow());
            warningActButton.onClick.AddListener(() => session.RespondToWarning(PlayerResponseType.Act));
            warningAcknowledgeButton.onClick.AddListener(() => session.RespondToWarning(PlayerResponseType.Acknowledge));
            warningIgnoreButton.onClick.AddListener(() => session.RespondToWarning(PlayerResponseType.Ignore));
            criticalActButton.onClick.AddListener(() => session.RespondToCritical(PlayerResponseType.Act));
            criticalAcknowledgeButton.onClick.AddListener(() => session.RespondToCritical(PlayerResponseType.Acknowledge));
            criticalIgnoreButton.onClick.AddListener(() => session.RespondToCritical(PlayerResponseType.Ignore));

            Refresh();
        }

        // Redraws every frame while the submenu is visible so an action taken
        // in the parallel OnGUI debug panel is reflected here immediately
        // (and vice versa) -- the two never show stale, contradictory state.
        private void Update()
        {
            if (session != null)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var incident = session.Config.EmergencyScenario.Incident;
            var runtime = session.EmergencyRuntime;
            incidentNameText.text = incident.DisplayName;

            raiseWarningButton.gameObject.SetActive(!runtime.WarningRaised);
            if (runtime.WarningRaised)
            {
                var warningAwaitingResponse = !session.WarningResponded;
                warningStatusText.text = warningAwaitingResponse
                    ? "[Warning] " + session.CurrentWarning.Message
                    : "Warning response recorded.";
                warningActButton.gameObject.SetActive(warningAwaitingResponse);
                warningAcknowledgeButton.gameObject.SetActive(warningAwaitingResponse);
                warningIgnoreButton.gameObject.SetActive(warningAwaitingResponse);
            }
            else
            {
                warningStatusText.text = "";
                warningActButton.gameObject.SetActive(false);
                warningAcknowledgeButton.gameObject.SetActive(false);
                warningIgnoreButton.gameObject.SetActive(false);
            }

            escalateButton.gameObject.SetActive(runtime.WarningRaised && !runtime.CriticalRaised);

            if (runtime.CriticalRaised)
            {
                if (runtime.HasDispatched)
                {
                    criticalStatusText.text = "[Critical] " + session.CurrentCritical.Message + "\nAlready dispatched — no further response needed.";
                    criticalActButton.gameObject.SetActive(false);
                    criticalAcknowledgeButton.gameObject.SetActive(false);
                    criticalIgnoreButton.gameObject.SetActive(false);
                }
                else
                {
                    var criticalAwaitingResponse = !session.CriticalResponded;
                    criticalStatusText.text = criticalAwaitingResponse
                        ? "[Critical] " + session.CurrentCritical.Message
                        : "Critical response recorded.";
                    criticalActButton.gameObject.SetActive(criticalAwaitingResponse);
                    criticalAcknowledgeButton.gameObject.SetActive(criticalAwaitingResponse);
                    criticalIgnoreButton.gameObject.SetActive(criticalAwaitingResponse);
                }
            }
            else
            {
                criticalStatusText.text = "";
                criticalActButton.gameObject.SetActive(false);
                criticalAcknowledgeButton.gameObject.SetActive(false);
                criticalIgnoreButton.gameObject.SetActive(false);
            }

            var state = session.RespondingDepartmentState;
            var sb = new StringBuilder();
            sb.Append("Fire units: ").Append(state.UncommittedUnitCount).Append(" / ").Append(state.TotalUnitCount).Append('\n');
            foreach (var district in session.Config.EmergencyScenario.Districts)
            {
                var coverage = CoverageResolver.ResolveDistrictCoverage(state, district, 0, session.Config.CitywideTravelTimeBands);
                sb.Append(district.DisplayName).Append(": ").Append(coverage).Append('\n');
            }
            if (session.LastDispatchResult != null)
            {
                sb.Append("Dispatch: ").Append(session.LastDispatchResult.TargetDistrictCoverage)
                  .Append(" coverage, ").Append(session.LastDispatchResult.SeverityMultiplier).Append("x severity");
            }
            coverageText.text = sb.ToString();
        }
    }
}
