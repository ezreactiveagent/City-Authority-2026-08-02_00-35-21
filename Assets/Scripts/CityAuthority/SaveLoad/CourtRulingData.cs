using System;
using CityAuthority.Data;

namespace CityAuthority.SaveLoad
{
    // 06 §9: the stored form of the one AI-driven decision in the slice — the
    // fields that must survive a reload verbatim, never be recomputed by
    // JudicialRulingSelector again. hasSplitIncrement stands in for the
    // nullable SplitIncrement? on CourtOutcomeOption.
    [Serializable]
    public sealed class CourtRulingData
    {
        public CourtOutcomeType outcomeType;
        public bool hasSplitIncrement;
        public SplitIncrement splitIncrement;
        public string explanation;
        public float cityAmount;
        public float ownerAmount;
        public JudgePersonalityTag judgePersonalityTag;
    }
}
