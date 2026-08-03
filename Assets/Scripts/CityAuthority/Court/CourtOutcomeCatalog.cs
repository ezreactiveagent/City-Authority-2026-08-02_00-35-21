using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.Court
{
    // 06 §2: the engine owns valid choices and penalty ranges. 10 §3 fixes both
    // for the slice's one case type — this is that fixed table in code.
    public static class CourtOutcomeCatalog
    {
        public static IReadOnlyList<CourtOutcomeOption> ValidOutcomesForCondemnationDispute { get; } = new[]
        {
            new CourtOutcomeOption(CourtOutcomeType.CityPays),
            new CourtOutcomeOption(CourtOutcomeType.OwnerPays),
            new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.TwentyFive),
            new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.Fifty),
            new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.SeventyFive),
            new CourtOutcomeOption(CourtOutcomeType.InitialAssignmentUpheld),
        };

        // Returns (cityShare, ownerShare) as fractions of C, per 10 §3's table.
        // InitialAssignmentUpheld defers to whatever the emergency commander
        // originally assigned (02 §13), passed in as initialAssignment.
        public static (float cityShare, float ownerShare) ComputeShares(
            CourtOutcomeOption option,
            CourtOutcomeOption initialAssignment)
        {
            var resolved = option.OutcomeType == CourtOutcomeType.InitialAssignmentUpheld
                ? initialAssignment
                : option;

            return resolved.OutcomeType switch
            {
                CourtOutcomeType.CityPays => (1f, 0f),
                CourtOutcomeType.OwnerPays => (0f, 1f),
                CourtOutcomeType.Split => SplitShares(resolved.SplitIncrement ?? SplitIncrement.Fifty),
                _ => (0f, 0f)
            };
        }

        private static (float cityShare, float ownerShare) SplitShares(SplitIncrement increment) => increment switch
        {
            SplitIncrement.TwentyFive => (0.25f, 0.75f),
            SplitIncrement.Fifty => (0.5f, 0.5f),
            SplitIncrement.SeventyFive => (0.75f, 0.25f),
            _ => (0.5f, 0.5f)
        };
    }
}
