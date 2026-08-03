using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    // 02 §13, 03 §21, 10 §3: the slice's one court case — the emergency
    // condemnation / demolition payment dispute arising from the scripted incident.
    [CreateAssetMenu(fileName = "CondemnationCase_", menuName = "City Authority/Data/Condemnation Case Definition")]
    public sealed class CondemnationCaseDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private District targetDistrict;

        [Tooltip("Cost basis C: the condemned structure's assessed demolition/rebuild value (01 §10).")]
        [Min(0)] [SerializeField] private float assessedValue;

        [Tooltip("What the Emergency Commander assigned at condemnation time, per 02 §13, before any court challenge.")]
        [SerializeField] private CourtOutcomeType initialAssignmentOutcome;
        [SerializeField] private SplitIncrement initialAssignmentSplitIncrement;

        [SerializeField] private List<Citizen> claimants = new();
        [SerializeField] private JudgePersonality assignedJudge;

        public string Id => id;
        public string DisplayName => displayName;
        public District TargetDistrict => targetDistrict;
        public float AssessedValue => assessedValue;
        public CourtOutcomeType InitialAssignmentOutcome => initialAssignmentOutcome;
        public SplitIncrement InitialAssignmentSplitIncrement => initialAssignmentSplitIncrement;
        public IReadOnlyList<Citizen> Claimants => claimants;
        public JudgePersonality AssignedJudge => assignedJudge;
    }
}
