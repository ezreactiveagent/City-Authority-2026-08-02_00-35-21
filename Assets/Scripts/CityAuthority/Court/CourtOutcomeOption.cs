using System;
using CityAuthority.Data;

namespace CityAuthority.Court
{
    // A single valid ruling: an outcome kind, plus the split increment when the
    // kind is Split. Value-equatable so it can be looked up in the engine's
    // fixed valid-outcome list (06 §10's "valid outcome identifier" check).
    public readonly struct CourtOutcomeOption : IEquatable<CourtOutcomeOption>
    {
        public CourtOutcomeType OutcomeType { get; }
        public SplitIncrement? SplitIncrement { get; }

        public CourtOutcomeOption(CourtOutcomeType outcomeType, SplitIncrement? splitIncrement = null)
        {
            OutcomeType = outcomeType;
            SplitIncrement = splitIncrement;
        }

        public bool Equals(CourtOutcomeOption other) =>
            OutcomeType == other.OutcomeType && SplitIncrement == other.SplitIncrement;

        public override bool Equals(object obj) => obj is CourtOutcomeOption other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(OutcomeType, SplitIncrement);

        public override string ToString() =>
            OutcomeType == CourtOutcomeType.Split ? $"Split({SplitIncrement})" : OutcomeType.ToString();
    }
}
