using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Media;

namespace CityAuthority.Report
{
    // 08 §10, 01 §6: the end-of-scenario report, generated once, on demand,
    // from the accountability record — the categories the eventual Final
    // Failure Report also uses. Historical archiving (01 §7) is out of scope
    // for the slice.
    public sealed class AccountabilityReport
    {
        public ScenarioResult Outcome { get; }
        public IReadOnlyList<AccountabilityEvent> WarningsIssued { get; }
        public IReadOnlyList<AccountabilityEvent> AcknowledgedUnresolved { get; }
        public IReadOnlyList<AccountabilityEvent> IgnoredWarnings { get; }
        public IReadOnlyList<AccountabilityEvent> ActionsTaken { get; }
        public CourtRulingRecord CourtRuling { get; }
        public IReadOnlyList<NewsArticle> MediaCoverage { get; }
        public string Summary { get; }

        public AccountabilityReport(
            ScenarioResult outcome,
            IReadOnlyList<AccountabilityEvent> warningsIssued,
            IReadOnlyList<AccountabilityEvent> acknowledgedUnresolved,
            IReadOnlyList<AccountabilityEvent> ignoredWarnings,
            IReadOnlyList<AccountabilityEvent> actionsTaken,
            CourtRulingRecord courtRuling,
            IReadOnlyList<NewsArticle> mediaCoverage,
            string summary)
        {
            Outcome = outcome;
            WarningsIssued = warningsIssued;
            AcknowledgedUnresolved = acknowledgedUnresolved;
            IgnoredWarnings = ignoredWarnings;
            ActionsTaken = actionsTaken;
            CourtRuling = courtRuling;
            MediaCoverage = mediaCoverage;
            Summary = summary;
        }
    }
}
