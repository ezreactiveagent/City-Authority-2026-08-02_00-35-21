using UnityEngine;

namespace CityAuthority.Data
{
    // 03 §19, 06 §5: one preassigned judge with one visible personality profile —
    // no selection roster for the slice. PersonalityTag is what the deterministic
    // ruling stub (11 §3) keys its outcome bias off of; PhilosophyDescription is
    // narrative flavor only, not consumed by the ruling logic.
    [CreateAssetMenu(fileName = "JudgePersonality_", menuName = "City Authority/Data/Judge Personality")]
    public sealed class JudgePersonality : ScriptableObject
    {
        [SerializeField] private string judgeName;
        [SerializeField] private JudgePersonalityTag personalityTag;
        [TextArea] [SerializeField] private string philosophyDescription;

        public string JudgeName => judgeName;
        public JudgePersonalityTag PersonalityTag => personalityTag;
        public string PhilosophyDescription => philosophyDescription;
    }
}
