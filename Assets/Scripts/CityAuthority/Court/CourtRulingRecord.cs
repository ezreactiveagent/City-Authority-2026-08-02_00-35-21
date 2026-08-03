using CityAuthority.Data;

namespace CityAuthority.Court
{
    // Everything 06 §9 requires stored for reproducibility: structured input,
    // allowed outcomes (carried on Facts), selected outcome, explanation,
    // mechanical result, and the personality state that produced it. Reloading
    // a save must show this stored record, never re-run the selector.
    public sealed class CourtRulingRecord
    {
        public CourtCaseFacts Facts { get; }
        public CourtOutcomeOption SelectedOutcome { get; }
        public string Explanation { get; }
        public float CityAmount { get; }
        public float OwnerAmount { get; }
        public JudgePersonalityTag JudgePersonalityTag { get; }

        public CourtRulingRecord(
            CourtCaseFacts facts,
            CourtOutcomeOption selectedOutcome,
            string explanation,
            float cityAmount,
            float ownerAmount,
            JudgePersonalityTag judgePersonalityTag)
        {
            Facts = facts;
            SelectedOutcome = selectedOutcome;
            Explanation = explanation;
            CityAmount = cityAmount;
            OwnerAmount = ownerAmount;
            JudgePersonalityTag = judgePersonalityTag;
        }
    }
}
