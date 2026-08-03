using System;
using System.Collections.Generic;
using CityAuthority.Data;

namespace CityAuthority.Court
{
    // 11 §3's deterministic stub for the restricted-outcome pattern (06 §4-5):
    // a rule keyed to the judge's personality tag stands in for an LLM call,
    // with the exact same {facts in} -> {outcome, explanation} shape, so a real
    // model call can later replace SelectOutcome without touching any caller.
    public static class JudicialRulingSelector
    {
        public static CourtRulingRecord Rule(CourtCaseFacts facts)
        {
            var chosen = SelectOutcome(facts.JudgePersonalityTag);

            // 06 §10: validate the selection is within the engine-provided valid
            // set before it enters the simulation. Deterministic output is valid
            // by construction today, but the gate stays in place so a future real
            // LLM call is checked the exact same way.
            if (!Contains(facts.ValidOutcomes, chosen))
            {
                throw new InvalidOperationException($"Selector produced an outcome outside the valid set: {chosen}");
            }

            var (cityShare, ownerShare) = CourtOutcomeCatalog.ComputeShares(chosen, facts.InitialAssignment);
            var cityAmount = facts.AssessedValue * cityShare;
            var ownerAmount = facts.AssessedValue * ownerShare;

            var explanation = BuildExplanation(chosen, facts, cityAmount, ownerAmount);

            return new CourtRulingRecord(facts, chosen, explanation, cityAmount, ownerAmount, facts.JudgePersonalityTag);
        }

        // Proposed default mapping (provisional design intent, not final balance),
        // per 11 §3's example: a public-safety-focused judge biases toward holding
        // someone accountable; a business-friendly judge biases the other way.
        private static CourtOutcomeOption SelectOutcome(JudgePersonalityTag tag) => tag switch
        {
            JudgePersonalityTag.PublicSafetyFocused => new CourtOutcomeOption(CourtOutcomeType.CityPays),
            JudgePersonalityTag.BusinessFriendly => new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.TwentyFive),
            JudgePersonalityTag.StrictProceduralist => new CourtOutcomeOption(CourtOutcomeType.InitialAssignmentUpheld),
            JudgePersonalityTag.LiabilityCautious => new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.Fifty),
            JudgePersonalityTag.CityFriendly => new CourtOutcomeOption(CourtOutcomeType.OwnerPays),
            JudgePersonalityTag.UnpredictableWithinLimits => new CourtOutcomeOption(CourtOutcomeType.Split, SplitIncrement.SeventyFive),
            _ => new CourtOutcomeOption(CourtOutcomeType.InitialAssignmentUpheld)
        };

        private static bool Contains(IReadOnlyList<CourtOutcomeOption> options, CourtOutcomeOption option)
        {
            foreach (var candidate in options)
            {
                if (candidate.Equals(option))
                {
                    return true;
                }
            }
            return false;
        }

        // "Rules-engine rulings" per 06 §11's AI failure fallback — one canned
        // template per outcome kind, parameterized with the case's actual facts.
        private static string BuildExplanation(CourtOutcomeOption outcome, CourtCaseFacts facts, float cityAmount, float ownerAmount)
        {
            var district = facts.TargetDistrict != null ? facts.TargetDistrict.DisplayName : "the district";

            return outcome.OutcomeType switch
            {
                CourtOutcomeType.CityPays =>
                    $"The Court finds the City bears full financial responsibility for the ${cityAmount:N0} in condemnation costs at {district}, citing its duty to maintain public safety standards.",
                CourtOutcomeType.OwnerPays =>
                    $"The Court finds the property owner bears full financial responsibility for the ${ownerAmount:N0} in condemnation costs at {district}, finding the hazard originated from conditions within the owner's control.",
                CourtOutcomeType.Split =>
                    $"The Court orders a split-responsibility ruling for {district}: the City covers ${cityAmount:N0} and the owner covers ${ownerAmount:N0} of the condemnation costs, citing shared contributing factors.",
                CourtOutcomeType.InitialAssignmentUpheld =>
                    $"The Court upholds the emergency commander's initial assignment of responsibility for {district}, finding no compelling reason presented at trial to disturb it.",
                _ => "The Court has issued a ruling."
            };
        }
    }
}
