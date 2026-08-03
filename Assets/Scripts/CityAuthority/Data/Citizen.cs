using UnityEngine;

namespace CityAuthority.Data
{
    // 11 §4's minimal named-citizen carve-out: no simulation loop, just enough
    // to give the condemnation court case concrete claimants instead of an
    // abstract population count.
    [CreateAssetMenu(fileName = "Citizen_", menuName = "City Authority/Data/Citizen")]
    public sealed class Citizen : ScriptableObject
    {
        [SerializeField] private string citizenName;
        [SerializeField] private District residenceDistrict;
        [SerializeField] private HousingStatus housingStatus;
        [Range(0, 100)] [SerializeField] private int satisfaction = 50;
        [Range(0, 100)] [SerializeField] private int trust = 50;

        public string CitizenName => citizenName;
        public District ResidenceDistrict => residenceDistrict;
        public HousingStatus HousingStatus => housingStatus;
        public int Satisfaction => satisfaction;
        public int Trust => trust;
    }
}
