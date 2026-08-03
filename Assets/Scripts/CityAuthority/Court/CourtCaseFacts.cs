using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.Court
{
    // The structured input side of the restricted-outcome pattern (06 §4, §9):
    // relevant facts, the engine-fixed valid outcomes, and the judge's tag.
    // This is exactly what a future LLM call would receive — swapping the
    // deterministic selector for a real model call means changing nothing here.
    public sealed class CourtCaseFacts
    {
        public string CaseId { get; }
        public District TargetDistrict { get; }
        public float AssessedValue { get; }
        public CourtOutcomeOption InitialAssignment { get; }
        public IReadOnlyList<string> ClaimantNames { get; }
        public JudgePersonalityTag JudgePersonalityTag { get; }
        public IReadOnlyList<CourtOutcomeOption> ValidOutcomes { get; }

        public CourtCaseFacts(
            string caseId,
            District targetDistrict,
            float assessedValue,
            CourtOutcomeOption initialAssignment,
            IReadOnlyList<string> claimantNames,
            JudgePersonalityTag judgePersonalityTag,
            IReadOnlyList<CourtOutcomeOption> validOutcomes)
        {
            CaseId = caseId;
            TargetDistrict = targetDistrict;
            AssessedValue = assessedValue;
            InitialAssignment = initialAssignment;
            ClaimantNames = claimantNames;
            JudgePersonalityTag = judgePersonalityTag;
            ValidOutcomes = validOutcomes;
        }
    }
}
