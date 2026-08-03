using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Media;

namespace CityAuthority.Report
{
    // 08 §10, 11 §2: assembles the final report by reading the City Log graph
    // built in Step 3, rather than being hand-assembled by whichever system
    // finishes last. The court ruling and published articles aren't recovered
    // from the log's free-text summaries — they're passed in as the same
    // structured objects Court/Media already produced, per 06 §9's
    // reproducibility requirement that stored records are read, never re-derived.
    public static class FinalReportGenerator
    {
        private const string WarningRaisedEventType = "EmergencyWarningRaised";
        private const string CriticalRaisedEventType = "EmergencyCriticalRaised";

        public static AccountabilityReport Generate(
            CityLog log,
            ScenarioResult outcome,
            CourtRulingRecord courtRuling,
            IReadOnlyList<NewsArticle> mediaCoverage)
        {
            var warningsIssued = new List<AccountabilityEvent>();
            foreach (var evt in log.Events)
            {
                if (evt.EventType == WarningRaisedEventType || evt.EventType == CriticalRaisedEventType)
                {
                    warningsIssued.Add(evt);
                }
            }

            var acknowledgedUnresolved = new List<AccountabilityEvent>(log.ByCategory(AccountabilityCategory.AcknowledgedUnresolved));
            var ignoredWarnings = new List<AccountabilityEvent>(log.ByCategory(AccountabilityCategory.IgnoredWarning));
            var actionsTaken = new List<AccountabilityEvent>(log.ByCategory(AccountabilityCategory.ActionCompleted));

            var summary = BuildSummary(outcome, warningsIssued.Count, acknowledgedUnresolved.Count, ignoredWarnings.Count, actionsTaken.Count, courtRuling, mediaCoverage.Count);

            return new AccountabilityReport(
                outcome, warningsIssued, acknowledgedUnresolved, ignoredWarnings, actionsTaken, courtRuling, mediaCoverage, summary);
        }

        private static string BuildSummary(
            ScenarioResult outcome,
            int warningCount,
            int acknowledgedCount,
            int ignoredCount,
            int actionsCount,
            CourtRulingRecord courtRuling,
            int articleCount)
        {
            var resultText = outcome == ScenarioResult.Success
                ? "The life-safety threat was contained."
                : "The life-safety threat was not contained.";

            var courtText = courtRuling != null
                ? $" The court ruled {courtRuling.SelectedOutcome}: {courtRuling.Explanation}"
                : " No court ruling was issued.";

            var articleWord = articleCount == 1 ? "story was" : "stories were";

            return $"{resultText} {warningCount} notification(s) issued, {acknowledgedCount} acknowledged without action, " +
                   $"{ignoredCount} ignored, {actionsCount} action(s) completed.{courtText} {articleCount} news {articleWord} published.";
        }
    }
}
