using System;
using CityAuthority.Accountability;
using CityAuthority.Data;

namespace CityAuthority.Development
{
    // 08 §4, 11 §2's "Development proposal" decision node: approve one of the
    // two proposals (binding, per 02 §5) or reject both (developer-interest
    // penalty per 10 §2, negative PR per 08 §9 / 02 §11). No detailed
    // negotiation/counteroffer loop, per 02 §4.
    public sealed class DevelopmentProposalCycleRuntime
    {
        private readonly DevelopmentListingDefinition definition;
        private readonly IAccountabilityRecorder recorder;

        private float developerInterest = DeveloperInterest.Starting;
        private DevelopmentProposal approvedProposal;
        private bool rejected;

        public DevelopmentProposalCycleRuntime(DevelopmentListingDefinition definition, IAccountabilityRecorder recorder)
        {
            this.definition = definition;
            this.recorder = recorder;
        }

        public float DeveloperInterestScore => developerInterest;
        public bool IsResolved => approvedProposal != null || rejected;
        public DevelopmentProposal ApprovedProposal => approvedProposal;
        public bool WasRejected => rejected;

        public void ApproveProposal(DevelopmentProposal proposal)
        {
            if (IsResolved)
            {
                return;
            }

            if (!Contains(proposal))
            {
                throw new InvalidOperationException($"'{proposal.DeveloperName}' is not one of this listing's proposals.");
            }

            approvedProposal = proposal;
            recorder.Record(new AccountabilityEvent(
                "DevelopmentProposalApproved",
                $"{definition.DisplayName}: approved {proposal.DeveloperName}'s proposal ({proposal.Density} density, targeting {proposal.TargetIncomeBand} income).",
                definition.District));
        }

        public void RejectBoth()
        {
            if (IsResolved)
            {
                return;
            }

            rejected = true;
            var before = developerInterest;
            developerInterest = DeveloperInterest.ApplyRejectionPenalty(developerInterest);

            recorder.Record(new AccountabilityEvent(
                "DevelopmentProposalsRejected",
                $"{definition.DisplayName}: both proposals rejected. Developer interest fell from {before:N0} to {developerInterest:N0}.",
                definition.District));
        }

        // Save/load (08 §13 item 8): restores the resolved/unresolved state
        // directly rather than replaying ApproveProposal/RejectBoth, since
        // replaying would re-record City Log entries that already exist in
        // the restored log.
        public static DevelopmentProposalCycleRuntime Restore(
            DevelopmentListingDefinition definition,
            IAccountabilityRecorder recorder,
            float developerInterest,
            DevelopmentProposal approvedProposal,
            bool rejected)
        {
            return new DevelopmentProposalCycleRuntime(definition, recorder)
            {
                developerInterest = developerInterest,
                approvedProposal = approvedProposal,
                rejected = rejected
            };
        }

        private bool Contains(DevelopmentProposal proposal)
        {
            foreach (var candidate in definition.Proposals)
            {
                if (candidate == proposal)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
