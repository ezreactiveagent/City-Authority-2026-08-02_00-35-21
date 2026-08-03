using System.IO;
using CityAuthority.Data;
using CityAuthority.Session;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace CityAuthority.Tests.Editor
{
    public class ScenarioSessionTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        private static string SavePath => Path.Combine(Application.persistentDataPath, "scenario_save.json");

        [SetUp]
        [TearDown]
        public void ClearAnySaveFile()
        {
            // ScenarioSession.SaveScenario/LoadScenario use the same fixed
            // persistentDataPath file the debug panel and UGUI menu use in
            // Play mode — clear it before and after so these tests neither
            // read stale state left by a previous run nor leave any behind.
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }

        [Test]
        public void Constructor_BootstrapsEveryRuntimeFromSliceConfig()
        {
            var session = new ScenarioSession(LoadSlice());

            Assert.IsNotNull(session.CityLog);
            Assert.IsNotNull(session.EmergencyRuntime);
            Assert.IsNotNull(session.RespondingDepartmentState);
            Assert.IsNotNull(session.CourtCase);
            Assert.IsNotNull(session.DevelopmentCycle);
            Assert.IsNotNull(session.Newspaper);
            Assert.IsFalse(session.HasSaveFile, "No save file should exist for a freshly constructed session in this test");
        }

        [Test]
        public void FullOrchestration_RaiseAcknowledgeEscalateAct_ProducesExpectedState()
        {
            var session = new ScenarioSession(LoadSlice());

            session.RaiseWarningNow();
            session.RespondToWarning(PlayerResponseType.Acknowledge);
            session.EscalateNow();
            session.RespondToCritical(PlayerResponseType.Act);

            Assert.IsTrue(session.EmergencyRuntime.WarningRaised);
            Assert.IsTrue(session.EmergencyRuntime.CriticalRaised);
            Assert.IsTrue(session.WarningResponded);
            Assert.IsTrue(session.CriticalResponded);
            Assert.IsNotNull(session.LastDispatchResult);
            Assert.AreEqual(CoverageState.Adequate, session.LastDispatchResult.TargetDistrictCoverage,
                "Direct response to the incident location resolves Adequate coverage, per 09 §7");
            Assert.AreEqual(0, session.RespondingDepartmentState.UncommittedUnitCount);
            Assert.GreaterOrEqual(session.CityLog.Events.Count, 4, "Warning, Critical, Act, and dispatch-resolved events must all be recorded");
        }

        [Test]
        public void SaveThenLoad_RevertsLiveStateToTheSavedSnapshot_NotFurtherProgress()
        {
            var session = new ScenarioSession(LoadSlice());

            session.RaiseWarningNow();
            session.RespondToWarning(PlayerResponseType.Acknowledge);
            session.EscalateNow();
            session.RespondToCritical(PlayerResponseType.Act);
            session.CondemnStructure();
            session.IssueRuling();
            session.PublishEmergencyResponseStory();

            var savedRulingOutcome = session.CourtCase.Ruling.SelectedOutcome;
            var savedArticleCount = session.Newspaper.PublishedArticles.Count;
            var savedLogCount = session.CityLog.Events.Count;

            session.SaveScenario();
            Assert.IsTrue(session.HasSaveFile);

            // Progress further after the save so live state diverges from the file.
            session.PublishCourtRulingStory();
            session.GenerateFinalReport();
            Assert.AreNotEqual(savedArticleCount, session.Newspaper.PublishedArticles.Count);
            Assert.IsNotNull(session.FinalReport);

            session.LoadScenario();

            Assert.AreEqual(savedRulingOutcome, session.CourtCase.Ruling.SelectedOutcome);
            Assert.AreEqual(savedArticleCount, session.Newspaper.PublishedArticles.Count,
                "Load must revert to the saved article count, not the diverged post-save count");
            Assert.AreEqual(savedLogCount, session.CityLog.Events.Count,
                "Load must revert to the saved log count, not the diverged post-save count");
            Assert.IsNull(session.FinalReport, "The final report was not generated at save time, so load must leave it unset");
        }
    }
}
