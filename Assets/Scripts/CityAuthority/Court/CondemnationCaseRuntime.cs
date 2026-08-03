using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Data;

namespace CityAuthority.Court
{
    // Orchestrates the slice's one court case (08 §7): builds the structured
    // input, calls the restricted-outcome selector, and records the result to
    // the City Log. The ruling is computed once and cached — reloading a save
    // must show the stored ruling, never a re-run (06 §9, deferred to slice v1
    // for actual persistence, but the runtime already behaves this way).
    public sealed class CondemnationCaseRuntime
    {
        private readonly CondemnationCaseDefinition definition;
        private readonly IAccountabilityRecorder recorder;
        private CourtRulingRecord ruling;

        public CondemnationCaseRuntime(CondemnationCaseDefinition definition, IAccountabilityRecorder recorder)
        {
            this.definition = definition;
            this.recorder = recorder;
        }

        public bool HasRuling => ruling != null;
        public CourtRulingRecord Ruling => ruling;

        public CourtRulingRecord IssueRuling()
        {
            if (ruling != null)
            {
                return ruling;
            }

            ruling = JudicialRulingSelector.Rule(BuildFacts(definition));

            recorder.Record(new AccountabilityEvent(
                "CourtRulingIssued",
                $"Judge {definition.AssignedJudge.JudgeName} ruled: {ruling.SelectedOutcome} — {ruling.Explanation}",
                definition.TargetDistrict));

            return ruling;
        }

        // Save/load (08 §13 item 8): the ruling itself (selected outcome,
        // explanation, amounts, judge tag) is stored/restored data — this never
        // calls JudicialRulingSelector again, per 06 §9's "never re-generate a
        // potentially different one." Facts are rebuilt from the definition
        // (design-time data, always available) rather than also being stored,
        // since nothing in Facts is itself a decision.
        public static CondemnationCaseRuntime Restore(
            CondemnationCaseDefinition definition,
            IAccountabilityRecorder recorder,
            CourtOutcomeOption? selectedOutcome,
            string explanation,
            float cityAmount,
            float ownerAmount,
            JudgePersonalityTag judgePersonalityTag)
        {
            var runtime = new CondemnationCaseRuntime(definition, recorder);

            if (selectedOutcome.HasValue)
            {
                runtime.ruling = new CourtRulingRecord(
                    BuildFacts(definition),
                    selectedOutcome.Value,
                    explanation,
                    cityAmount,
                    ownerAmount,
                    judgePersonalityTag);
            }

            return runtime;
        }

        private static CourtCaseFacts BuildFacts(CondemnationCaseDefinition definition)
        {
            var claimantNames = new List<string>();
            foreach (var claimant in definition.Claimants)
            {
                claimantNames.Add(claimant.CitizenName);
            }

            var initialAssignment = new CourtOutcomeOption(
                definition.InitialAssignmentOutcome,
                definition.InitialAssignmentOutcome == CourtOutcomeType.Split
                    ? definition.InitialAssignmentSplitIncrement
                    : null);

            return new CourtCaseFacts(
                definition.Id,
                definition.TargetDistrict,
                definition.AssessedValue,
                initialAssignment,
                claimantNames,
                definition.AssignedJudge.PersonalityTag,
                CourtOutcomeCatalog.ValidOutcomesForCondemnationDispute);
        }
    }
}
