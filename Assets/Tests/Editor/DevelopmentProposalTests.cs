using CityAuthority.Accountability;
using CityAuthority.Data;
using CityAuthority.Development;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class DevelopmentProposalTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void DeveloperInterest_RejectionPenalty_MatchesReport10Defaults()
        {
            Assert.AreEqual(60f, DeveloperInterest.ApplyRejectionPenalty(100f));
            Assert.AreEqual(10f, DeveloperInterest.ApplyRejectionPenalty(30f), "Must not drop below the floor of 10");
            Assert.AreEqual(10f, DeveloperInterest.ApplyRejectionPenalty(10f));
        }

        [Test]
        public void DeveloperInterest_Recovery_MatchesReport10Defaults()
        {
            Assert.AreEqual(65f, DeveloperInterest.RecoverOverWeeks(60f, 1f));
            Assert.AreEqual(100f, DeveloperInterest.RecoverOverWeeks(95f, 3f), "Must not exceed the ceiling of 100");
        }

        [Test]
        public void DevelopmentListing_SliceInstance_HasExactlyTwoProposals()
        {
            var slice = LoadSlice();
            Assert.IsNotNull(slice.DevelopmentListing, "SliceConfig must reference a DevelopmentListingDefinition");

            var listing = slice.DevelopmentListing;
            Assert.AreEqual(2, listing.Proposals.Count, "08 §4: exactly two competing proposals");

            foreach (var proposal in listing.Proposals)
            {
                Assert.AreEqual(listing.Zoning, proposal.Zoning, "Both proposals must comply with the listing's designated zoning");
                Assert.IsFalse(string.IsNullOrWhiteSpace(proposal.Description));
                Assert.GreaterOrEqual(proposal.EstimatedAnnualTaxRevenue, 0f);
            }

            Assert.AreNotEqual(
                listing.Proposals[0].Density + listing.Proposals[0].TargetIncomeBand.ToString(),
                listing.Proposals[1].Density + listing.Proposals[1].TargetIncomeBand.ToString(),
                "The two proposals should meaningfully differ, per 02 §4");
        }

        [Test]
        public void ApproveProposal_ResolvesWithoutInterestPenalty()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var runtime = new DevelopmentProposalCycleRuntime(slice.DevelopmentListing, log);
            var chosen = slice.DevelopmentListing.Proposals[0];

            runtime.ApproveProposal(chosen);

            Assert.IsTrue(runtime.IsResolved);
            Assert.AreEqual(chosen, runtime.ApprovedProposal);
            Assert.IsFalse(runtime.WasRejected);
            Assert.AreEqual(DeveloperInterest.Starting, runtime.DeveloperInterestScore, "Approval must not apply the rejection penalty");

            var approvalEvents = 0;
            foreach (var evt in log.Events)
            {
                if (evt.EventType == "DevelopmentProposalApproved") approvalEvents++;
            }
            Assert.AreEqual(1, approvalEvents);
        }

        [Test]
        public void RejectBoth_AppliesPenaltyAndLogsIt()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var runtime = new DevelopmentProposalCycleRuntime(slice.DevelopmentListing, log);

            runtime.RejectBoth();

            Assert.IsTrue(runtime.IsResolved);
            Assert.IsTrue(runtime.WasRejected);
            Assert.IsNull(runtime.ApprovedProposal);
            Assert.AreEqual(60f, runtime.DeveloperInterestScore);

            var rejectionEvents = 0;
            foreach (var evt in log.Events)
            {
                if (evt.EventType == "DevelopmentProposalsRejected") rejectionEvents++;
            }
            Assert.AreEqual(1, rejectionEvents, "08 §9: the rejection penalty must be visible in the City Log");
        }

        [Test]
        public void Resolution_IsIdempotent()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var runtime = new DevelopmentProposalCycleRuntime(slice.DevelopmentListing, log);

            runtime.ApproveProposal(slice.DevelopmentListing.Proposals[0]);
            runtime.RejectBoth(); // must be a no-op once resolved

            Assert.IsFalse(runtime.WasRejected);
            Assert.AreEqual(slice.DevelopmentListing.Proposals[0], runtime.ApprovedProposal);
            Assert.AreEqual(DeveloperInterest.Starting, runtime.DeveloperInterestScore);
        }
    }
}
