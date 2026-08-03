using System.Collections.Generic;
using System.IO;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;
using CityAuthority.Report;
using CityAuthority.SaveLoad;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace CityAuthority.Tests.Editor
{
    public class SaveLoadTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void CityLog_FromEvents_PreservesOrderAndContent()
        {
            var original = new CityLog();
            original.Record(new AccountabilityEvent("First", "first", category: AccountabilityCategory.IgnoredWarning));
            original.Record(new AccountabilityEvent("Second", "second", relatedLevel: NotificationLevel.Critical));

            var restored = CityLog.FromEvents(original.Events);

            Assert.AreEqual(2, restored.Events.Count);
            Assert.AreEqual("First", restored.Events[0].EventType);
            Assert.AreEqual(AccountabilityCategory.IgnoredWarning, restored.Events[0].Category);
            Assert.AreEqual("Second", restored.Events[1].EventType);
            Assert.AreEqual(NotificationLevel.Critical, restored.Events[1].RelatedLevel);
        }

        [Test]
        public void DepartmentCoverageState_RestoreCommittedUnitCount_ClampsToTotal()
        {
            var slice = LoadSlice();
            var department = slice.EmergencyScenario.Incident.RespondingDepartment;
            var state = new DepartmentCoverageState(department);

            state.RestoreCommittedUnitCount(999);

            Assert.AreEqual(0, state.UncommittedUnitCount, "Restoring beyond total must clamp, not overflow negative");
            Assert.AreEqual(department, state.Department);
        }

        [Test]
        public void EmergencyIncidentRuntime_Restore_SetsFlagsWithoutWritingLogEntries()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var fireState = new DepartmentCoverageState(incident.RespondingDepartment);
            var dispatch = new DispatchResult(CoverageState.Adequate, 1f, new List<Notification>());
            var log = new CityLog();

            var restored = EmergencyIncidentRuntime.Restore(
                incident, fireState, slice.EmergencyScenario.Districts, slice.CitywideTravelTimeBands, log,
                warningRaised: true, criticalRaised: true, dispatchResult: dispatch);

            Assert.IsTrue(restored.WarningRaised);
            Assert.IsTrue(restored.CriticalRaised);
            Assert.IsTrue(restored.HasDispatched);
            Assert.AreSame(dispatch, restored.DispatchResultIfAny);
            Assert.AreEqual(0, log.Events.Count, "Restore must not itself record any City Log entries");
        }

        [Test]
        public void CondemnationCaseRuntime_Restore_NeverCallsSelectorAgain()
        {
            var slice = LoadSlice();
            var log = new CityLog();

            // LiabilityCautious deterministically maps to Split/Fifty per
            // JudicialRulingSelector — deliberately restore a contradicting
            // outcome (OwnerPays) to prove Restore returns exactly what was
            // stored rather than recomputing via the selector (06 §9).
            var restored = CondemnationCaseRuntime.Restore(
                slice.CourtCase, log,
                selectedOutcome: new CourtOutcomeOption(CourtOutcomeType.OwnerPays),
                explanation: "stored explanation text",
                cityAmount: 0f,
                ownerAmount: slice.CourtCase.AssessedValue,
                judgePersonalityTag: JudgePersonalityTag.LiabilityCautious);

            Assert.IsTrue(restored.HasRuling);
            Assert.AreEqual(CourtOutcomeType.OwnerPays, restored.Ruling.SelectedOutcome.OutcomeType);
            Assert.AreEqual("stored explanation text", restored.Ruling.Explanation);
            Assert.AreEqual(slice.CourtCase.AssessedValue, restored.Ruling.OwnerAmount, 0.01f);
            Assert.AreEqual(0, log.Events.Count, "Restore must not record a CourtRulingIssued entry");
        }

        [Test]
        public void CondemnationCaseRuntime_Restore_WithNoRuling_HasRulingIsFalse()
        {
            var slice = LoadSlice();
            var restored = CondemnationCaseRuntime.Restore(slice.CourtCase, new CityLog(), null, null, 0f, 0f, default);

            Assert.IsFalse(restored.HasRuling);
        }

        [Test]
        public void DevelopmentProposalCycleRuntime_Restore_ReconstructsApprovedProposalByReference()
        {
            var slice = LoadSlice();
            var listing = slice.DevelopmentListing;
            var expectedProposal = listing.Proposals[0];

            var restored = DevelopmentProposalCycleRuntime.Restore(listing, new CityLog(), 55f, expectedProposal, false);

            Assert.AreEqual(55f, restored.DeveloperInterestScore);
            Assert.AreSame(expectedProposal, restored.ApprovedProposal);
            Assert.IsTrue(restored.IsResolved);
            Assert.IsFalse(restored.WasRejected);
        }

        [Test]
        public void NewspaperCoverageRuntime_Restore_ReturnsExactlyTheGivenArticles()
        {
            var slice = LoadSlice();
            var article = new NewsArticle("EmergencyResponse", "Headline", "Body", slice.EmergencyScenario.Incident.TargetDistrict);

            var restored = NewspaperCoverageRuntime.Restore(slice.Newspaper, new CityLog(), new[] { article });

            Assert.AreEqual(1, restored.PublishedArticles.Count);
            Assert.AreSame(article, restored.PublishedArticles[0]);
        }

        [Test]
        public void ScenarioSaveData_JsonRoundTrip_PreservesAllFields()
        {
            var data = new ScenarioSaveData
            {
                structureCondemned = true,
                finalReportGenerated = true,
                hasCourtRuling = true,
                courtRuling = new CourtRulingData
                {
                    outcomeType = CourtOutcomeType.Split,
                    hasSplitIncrement = true,
                    splitIncrement = SplitIncrement.TwentyFive,
                    explanation = "explained",
                    cityAmount = 111f,
                    ownerAmount = 222f,
                    judgePersonalityTag = JudgePersonalityTag.BusinessFriendly
                }
            };
            data.logEvents.Add(new AccountabilityEventData
            {
                eventType = "Type", summary = "Summary", districtId = "downtown", hasLevel = true, level = NotificationLevel.Critical
            });
            data.development = new DevelopmentStateData { developerInterest = 42f, rejected = true, approvedProposalId = "" };
            data.articles.Add(new NewsArticleData { sourceEventType = "CourtRuling", headline = "H", body = "B", districtId = "downtown" });
            data.emergency = new EmergencyStateData
            {
                warningRaised = true,
                criticalRaised = true,
                warningResponded = true,
                criticalResponded = true,
                committedUnitCount = 1,
                hasDispatchResult = true,
                dispatchResult = new DispatchResultData { targetDistrictCoverage = CoverageState.Reduced, severityMultiplier = 1.25f }
            };

            var json = JsonUtility.ToJson(data);
            var roundTripped = JsonUtility.FromJson<ScenarioSaveData>(json);

            Assert.IsTrue(roundTripped.structureCondemned);
            Assert.IsTrue(roundTripped.finalReportGenerated);
            Assert.IsTrue(roundTripped.hasCourtRuling);
            Assert.AreEqual(CourtOutcomeType.Split, roundTripped.courtRuling.outcomeType);
            Assert.IsTrue(roundTripped.courtRuling.hasSplitIncrement);
            Assert.AreEqual(SplitIncrement.TwentyFive, roundTripped.courtRuling.splitIncrement);
            Assert.AreEqual(111f, roundTripped.courtRuling.cityAmount);
            Assert.AreEqual(1, roundTripped.logEvents.Count);
            Assert.AreEqual("downtown", roundTripped.logEvents[0].districtId);
            Assert.AreEqual(42f, roundTripped.development.developerInterest);
            Assert.AreEqual(1, roundTripped.articles.Count);
            Assert.IsTrue(roundTripped.emergency.hasDispatchResult);
            Assert.AreEqual(CoverageState.Reduced, roundTripped.emergency.dispatchResult.targetDistrictCoverage);
        }

        [Test]
        public void SaveFileIO_RoundTrip_ThroughActualDisk()
        {
            var path = Path.Combine(Application.temporaryCachePath, "save_load_test_" + System.Guid.NewGuid() + ".json");
            try
            {
                var data = new ScenarioSaveData { structureCondemned = true, finalReportGenerated = false };
                data.logEvents.Add(new AccountabilityEventData { eventType = "X", summary = "Y" });

                Assert.IsFalse(SaveFileIO.Exists(path));
                SaveFileIO.Save(path, data);
                Assert.IsTrue(SaveFileIO.Exists(path));

                var loaded = SaveFileIO.Load(path);
                Assert.IsTrue(loaded.structureCondemned);
                Assert.AreEqual(1, loaded.logEvents.Count);
                Assert.AreEqual("X", loaded.logEvents[0].eventType);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Test]
        public void ScenarioSaveService_CaptureThenRestore_FullMidScenarioState_IsEquivalent()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var log = new CityLog();
            var fireState = new DepartmentCoverageState(incident.RespondingDepartment);
            var emergencyRuntime = new EmergencyIncidentRuntime(
                incident, fireState, slice.EmergencyScenario.Districts, slice.CitywideTravelTimeBands, log);

            emergencyRuntime.RaiseWarning();
            emergencyRuntime.RecordAcknowledge(NotificationLevel.Warning);
            emergencyRuntime.EscalateToCritical();
            emergencyRuntime.RecordActAndDispatch(NotificationLevel.Critical);

            var courtCase = new CondemnationCaseRuntime(slice.CourtCase, log);
            var ruling = courtCase.IssueRuling();

            var developmentCycle = new DevelopmentProposalCycleRuntime(slice.DevelopmentListing, log);
            developmentCycle.ApproveProposal(slice.DevelopmentListing.Proposals[1]);

            var newspaper = new NewspaperCoverageRuntime(slice.Newspaper, log);
            newspaper.PublishEmergencyResponseStory(incident, emergencyRuntime.DispatchResultIfAny);
            newspaper.PublishCourtRulingStory(slice.CourtCase, ruling);

            var saveData = ScenarioSaveService.Capture(
                log, emergencyRuntime, fireState,
                warningResponded: true, criticalResponded: true, structureCondemned: true,
                courtCase, developmentCycle, newspaper, finalReportGenerated: true);

            // Round-trip through actual JSON, not just the in-memory DTO, so
            // the test exercises exactly what a real save file goes through.
            var json = JsonUtility.ToJson(saveData);
            var reloadedData = JsonUtility.FromJson<ScenarioSaveData>(json);

            var restored = ScenarioSaveService.Restore(reloadedData, slice);

            Assert.AreEqual(log.Events.Count, restored.CityLog.Events.Count);
            for (var i = 0; i < log.Events.Count; i++)
            {
                Assert.AreEqual(log.Events[i].EventType, restored.CityLog.Events[i].EventType);
                Assert.AreEqual(log.Events[i].Summary, restored.CityLog.Events[i].Summary);
            }

            Assert.IsTrue(restored.EmergencyRuntime.WarningRaised);
            Assert.IsTrue(restored.EmergencyRuntime.CriticalRaised);
            Assert.IsTrue(restored.EmergencyRuntime.HasDispatched);
            Assert.AreEqual(emergencyRuntime.DispatchResultIfAny.TargetDistrictCoverage, restored.EmergencyRuntime.DispatchResultIfAny.TargetDistrictCoverage);
            Assert.AreEqual(emergencyRuntime.DispatchResultIfAny.SeverityMultiplier, restored.EmergencyRuntime.DispatchResultIfAny.SeverityMultiplier);
            Assert.AreEqual(0, restored.RespondingDepartmentState.UncommittedUnitCount, "The one unit was committed on dispatch");
            Assert.IsTrue(restored.WarningResponded);
            Assert.IsTrue(restored.CriticalResponded);
            Assert.IsTrue(restored.StructureCondemned);

            Assert.IsTrue(restored.CourtCase.HasRuling);
            Assert.AreEqual(ruling.SelectedOutcome.OutcomeType, restored.CourtCase.Ruling.SelectedOutcome.OutcomeType);
            Assert.AreEqual(ruling.SelectedOutcome.SplitIncrement, restored.CourtCase.Ruling.SelectedOutcome.SplitIncrement);
            Assert.AreEqual(ruling.Explanation, restored.CourtCase.Ruling.Explanation);
            Assert.AreEqual(ruling.CityAmount, restored.CourtCase.Ruling.CityAmount, 0.01f);
            Assert.AreEqual(ruling.OwnerAmount, restored.CourtCase.Ruling.OwnerAmount, 0.01f);

            Assert.AreEqual(developmentCycle.DeveloperInterestScore, restored.DevelopmentCycle.DeveloperInterestScore);
            Assert.AreSame(developmentCycle.ApprovedProposal, restored.DevelopmentCycle.ApprovedProposal);

            Assert.AreEqual(2, restored.Newspaper.PublishedArticles.Count);
            Assert.AreEqual(newspaper.PublishedArticles[0].Headline, restored.Newspaper.PublishedArticles[0].Headline);
            Assert.AreEqual(newspaper.PublishedArticles[1].Headline, restored.Newspaper.PublishedArticles[1].Headline);

            Assert.IsTrue(restored.FinalReportGenerated);

            // The final report itself is a pure recomputation over restored
            // data (not separately stored) — verify it comes out identical.
            var outcome = ScenarioOutcomeResolver.Resolve(incident, emergencyRuntime.DispatchResultIfAny);
            var originalReport = FinalReportGenerator.Generate(log, outcome, ruling, newspaper.PublishedArticles);
            var restoredOutcome = ScenarioOutcomeResolver.Resolve(incident, restored.EmergencyRuntime.DispatchResultIfAny);
            var restoredReport = FinalReportGenerator.Generate(restored.CityLog, restoredOutcome, restored.CourtCase.Ruling, restored.Newspaper.PublishedArticles);

            Assert.AreEqual(originalReport.Outcome, restoredReport.Outcome);
            Assert.AreEqual(originalReport.Summary, restoredReport.Summary);
            Assert.AreEqual(originalReport.WarningsIssued.Count, restoredReport.WarningsIssued.Count);
            Assert.AreEqual(originalReport.ActionsTaken.Count, restoredReport.ActionsTaken.Count);
        }
    }
}
