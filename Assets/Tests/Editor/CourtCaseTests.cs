using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class CourtCaseTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void CourtOutcomeCatalog_DefinesExactlySixValidOutcomes()
        {
            var outcomes = CourtOutcomeCatalog.ValidOutcomesForCondemnationDispute;
            Assert.AreEqual(6, outcomes.Count);

            var seen = new HashSet<string>();
            foreach (var outcome in outcomes)
            {
                Assert.IsTrue(seen.Add(outcome.ToString()), "Every valid outcome must be distinct: " + outcome);
            }
        }

        [TestCase(CourtOutcomeType.CityPays, null, 1f, 0f)]
        [TestCase(CourtOutcomeType.OwnerPays, null, 0f, 1f)]
        [TestCase(CourtOutcomeType.Split, SplitIncrement.TwentyFive, 0.25f, 0.75f)]
        [TestCase(CourtOutcomeType.Split, SplitIncrement.Fifty, 0.5f, 0.5f)]
        [TestCase(CourtOutcomeType.Split, SplitIncrement.SeventyFive, 0.75f, 0.25f)]
        public void ComputeShares_MatchesReport10Table(CourtOutcomeType type, SplitIncrement? increment, float expectedCityShare, float expectedOwnerShare)
        {
            var option = new CourtOutcomeOption(type, increment);
            var initialAssignment = new CourtOutcomeOption(CourtOutcomeType.OwnerPays);

            var (cityShare, ownerShare) = CourtOutcomeCatalog.ComputeShares(option, initialAssignment);

            Assert.AreEqual(expectedCityShare, cityShare, 0.001f);
            Assert.AreEqual(expectedOwnerShare, ownerShare, 0.001f);
        }

        [Test]
        public void ComputeShares_InitialAssignmentUpheld_DefersToInitialAssignment()
        {
            var upheld = new CourtOutcomeOption(CourtOutcomeType.InitialAssignmentUpheld);
            var initialAssignment = new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.TwentyFive);

            var (cityShare, ownerShare) = CourtOutcomeCatalog.ComputeShares(upheld, initialAssignment);

            Assert.AreEqual(0.25f, cityShare, 0.001f);
            Assert.AreEqual(0.75f, ownerShare, 0.001f);
        }

        [TestCase(JudgePersonalityTag.PublicSafetyFocused, CourtOutcomeType.CityPays)]
        [TestCase(JudgePersonalityTag.BusinessFriendly, CourtOutcomeType.Split)]
        [TestCase(JudgePersonalityTag.StrictProceduralist, CourtOutcomeType.InitialAssignmentUpheld)]
        [TestCase(JudgePersonalityTag.LiabilityCautious, CourtOutcomeType.Split)]
        [TestCase(JudgePersonalityTag.CityFriendly, CourtOutcomeType.OwnerPays)]
        [TestCase(JudgePersonalityTag.UnpredictableWithinLimits, CourtOutcomeType.Split)]
        public void JudicialRulingSelector_MapsEveryPersonalityTagToAValidOutcome(JudgePersonalityTag tag, CourtOutcomeType expectedKind)
        {
            var facts = new CourtCaseFacts(
                "test-case",
                null,
                100000f,
                new CourtOutcomeOption(CourtOutcomeType.OwnerPays),
                new List<string> { "Test Claimant" },
                tag,
                CourtOutcomeCatalog.ValidOutcomesForCondemnationDispute);

            var ruling = JudicialRulingSelector.Rule(facts);

            Assert.AreEqual(expectedKind, ruling.SelectedOutcome.OutcomeType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(ruling.Explanation));
            Assert.AreEqual(100000f, ruling.CityAmount + ruling.OwnerAmount, 0.01f, "City + Owner amounts must sum to the full assessed value");
        }

        [Test]
        public void CondemnationCaseRuntime_IssueRuling_IsIdempotentAndRecordsExactlyOneLogEntry()
        {
            var slice = LoadSlice();
            Assert.IsNotNull(slice.CourtCase, "SliceConfig must reference a CondemnationCaseDefinition");

            var log = new CityLog();
            var runtime = new CondemnationCaseRuntime(slice.CourtCase, log);

            var first = runtime.IssueRuling();
            var second = runtime.IssueRuling();

            Assert.AreSame(first, second, "A second call must return the stored ruling, not regenerate one (06 §9)");

            var rulingEvents = 0;
            foreach (var evt in log.Events)
            {
                if (evt.EventType == "CourtRulingIssued") rulingEvents++;
            }
            Assert.AreEqual(1, rulingEvents, "Exactly one CourtRulingIssued log entry must be recorded");
        }

        [Test]
        public void CondemnationCase_SliceInstance_HasOwnerClaimant()
        {
            var slice = LoadSlice();
            var courtCase = slice.CourtCase;

            Assert.IsNotNull(courtCase.AssignedJudge);
            Assert.GreaterOrEqual(courtCase.AssessedValue, 0f);
            Assert.GreaterOrEqual(courtCase.Claimants.Count, 1);

            var hasOwnerClaimant = false;
            foreach (var claimant in courtCase.Claimants)
            {
                if (claimant.HousingStatus == HousingStatus.Owner)
                {
                    hasOwnerClaimant = true;
                }
            }
            Assert.IsTrue(hasOwnerClaimant, "02 §13/11 §4: ownership determines claimant status — at least one claimant must be an Owner");
        }
    }
}
