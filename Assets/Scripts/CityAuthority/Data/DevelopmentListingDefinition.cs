using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    // 08 §4: the slice's one land listing — a designated zoning and exactly two
    // competing proposals. The one parcel is hand-placed, per 08 §11.
    [CreateAssetMenu(fileName = "DevelopmentListing_", menuName = "City Authority/Data/Development Listing Definition")]
    public sealed class DevelopmentListingDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private District district;
        [SerializeField] private ZoningCategory zoning;
        [SerializeField] private List<DevelopmentProposal> proposals = new();

        public string Id => id;
        public string DisplayName => displayName;
        public District District => district;
        public ZoningCategory Zoning => zoning;
        public IReadOnlyList<DevelopmentProposal> Proposals => proposals;
    }
}
