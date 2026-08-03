using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Emergency;
using CityAuthority.Media;
using CityAuthority.Report;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class FinalReportTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void ScenarioOutcomeResolver_NeverDispatched_IsFailure()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;

            var outcome = ScenarioOutcomeResolver.Resolve(incident, null);

            Assert.AreEqual(ScenarioResult.Failure, outcome, "A life-safety incident with no dispatch was never contained");
        }

        [Test]
        public void ScenarioOutcomeResolver_DispatchedWithAdequateCoverage_IsSuccess()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var dispatch = new DispatchResult(CoverageState.Adequate, 1f, new List<Notification>());

            var outcome = ScenarioOutcomeResolver.Resolve(incident, dispatch);

            Assert.AreEqual(ScenarioResult.Success, outcome);
        }

        [Test]
        public void ScenarioOutcomeResolver_DispatchedWithUncoveredTarget_IsFailure()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var dispatch = new DispatchResult(CoverageState.Uncovered, 1.6f, new List<Notification>());

            var outcome = ScenarioOutcomeResolver.Resolve(incident, dispatch);

            Assert.AreEqual(ScenarioResult.Failure, outcome, "Arriving too late for real coverage is not containment");
        }

        [Test]
        public void FinalReportGenerator_CategorizesLogEntriesCorrectly()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var log = new CityLog();
            var runtime = new EmergencyIncidentRuntime(
                incident, new DepartmentCoverageState(incident.RespondingDepartment), slice.EmergencyScenario.Districts, slice.CitywideTravelTimeBands, log);

            runtime.RaiseWarning();
            runtime.RecordIgnore(NotificationLevel.Warning);
            runtime.EscalateToCritical();
            var dispatch = runtime.RecordActAndDispatch(NotificationLevel.Critical);

            var outcome = ScenarioOutcomeResolver.Resolve(incident, dispatch);
            var report = FinalReportGenerator.Generate(log, outcome, null, new List<NewsArticle>());

            Assert.AreEqual(2, report.WarningsIssued.Count, "Both the Warning and Critical raise events count as warnings issued");
            Assert.AreEqual(1, report.IgnoredWarnings.Count);
            Assert.AreEqual(1, report.ActionsTaken.Count);
            Assert.AreEqual(0, report.AcknowledgedUnresolved.Count);
            Assert.IsNull(report.CourtRuling);
            Assert.AreEqual(0, report.MediaCoverage.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(report.Summary));
        }

        [Test]
        public void FinalReportGenerator_IncludesCourtRulingAndMediaCoverageWhenProvided()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var courtRuntime = new CondemnationCaseRuntime(slice.CourtCase, log);
            var ruling = courtRuntime.IssueRuling();
            var newspaper = new NewspaperCoverageRuntime(slice.Newspaper, log);
            newspaper.PublishCourtRulingStory(slice.CourtCase, ruling);

            var report = FinalReportGenerator.Generate(log, ScenarioResult.Success, ruling, newspaper.PublishedArticles);

            Assert.AreSame(ruling, report.CourtRuling);
            Assert.AreEqual(1, report.MediaCoverage.Count);
            StringAssert.Contains(ruling.SelectedOutcome.ToString(), report.Summary);
        }

        [Test]
        public void FullScenario_DirectResponse_ProducesSuccessWithCompleteReport()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var log = new CityLog();
            var runtime = new EmergencyIncidentRuntime(
                incident, new DepartmentCoverageState(incident.RespondingDepartment), slice.EmergencyScenario.Districts, slice.CitywideTravelTimeBands, log);

            runtime.RaiseWarning();
            runtime.RecordAcknowledge(NotificationLevel.Warning);
            runtime.EscalateToCritical();
            var dispatch = runtime.RecordActAndDispatch(NotificationLevel.Critical);

            var courtRuntime = new CondemnationCaseRuntime(slice.CourtCase, log);
            var ruling = courtRuntime.IssueRuling();

            var newspaper = new NewspaperCoverageRuntime(slice.Newspaper, log);
            newspaper.PublishEmergencyResponseStory(incident, dispatch);
            newspaper.PublishCourtRulingStory(slice.CourtCase, ruling);

            var outcome = ScenarioOutcomeResolver.Resolve(incident, dispatch);
            var report = FinalReportGenerator.Generate(log, outcome, ruling, newspaper.PublishedArticles);

            Assert.AreEqual(ScenarioResult.Success, report.Outcome, "Direct response resolves Adequate coverage at the target district (09 §7)");
            Assert.AreEqual(2, report.WarningsIssued.Count);
            Assert.AreEqual(1, report.AcknowledgedUnresolved.Count);
            Assert.AreEqual(1, report.ActionsTaken.Count);
            Assert.IsNotNull(report.CourtRuling);
            Assert.AreEqual(2, report.MediaCoverage.Count);
        }
    }
}
