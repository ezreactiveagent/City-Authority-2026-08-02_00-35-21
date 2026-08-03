using UnityEngine;

namespace CityAuthority.Data
{
    // 02 §4: one of exactly two competing proposals for the slice's one listed
    // parcel (08 §4). Description is a hand-authored "generic proposal
    // description" (06 §11's AI fallback), not generated at runtime.
    [CreateAssetMenu(fileName = "DevelopmentProposal_", menuName = "City Authority/Data/Development Proposal")]
    public sealed class DevelopmentProposal : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string developerName;
        [SerializeField] private ZoningCategory zoning;
        [SerializeField] private DevelopmentDensity density;
        [SerializeField] private IncomeBand targetIncomeBand;
        [Min(0)] [SerializeField] private float estimatedAnnualTaxRevenue;
        [SerializeField] private DevelopmentRisk risk;
        [TextArea] [SerializeField] private string description;

        public string Id => id;
        public string DeveloperName => developerName;
        public ZoningCategory Zoning => zoning;
        public DevelopmentDensity Density => density;
        public IncomeBand TargetIncomeBand => targetIncomeBand;
        public float EstimatedAnnualTaxRevenue => estimatedAnnualTaxRevenue;
        public DevelopmentRisk Risk => risk;
        public string Description => description;
    }
}
