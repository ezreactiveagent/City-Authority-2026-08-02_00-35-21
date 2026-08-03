using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    [Serializable]
    public sealed class MayorReactionLine
    {
        [SerializeField] private MayorOutcomeType outcomeType;
        [TextArea] [SerializeField] private string line;

        public MayorOutcomeType OutcomeType => outcomeType;
        public string Line => line;
    }

    [CreateAssetMenu(fileName = "MayorPersonality_", menuName = "City Authority/Data/Mayor Personality")]
    public sealed class MayorPersonality : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private List<string> personalityTraits = new();
        [SerializeField] private List<MayorReactionLine> reactionLines = new();

        public string Id => id;
        public string DisplayName => displayName;
        public IReadOnlyList<string> PersonalityTraits => personalityTraits;
        public IReadOnlyList<MayorReactionLine> ReactionLines => reactionLines;
    }
}
