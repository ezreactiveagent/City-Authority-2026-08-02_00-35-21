using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    [CreateAssetMenu(fileName = "SliceConfig_", menuName = "City Authority/Data/Slice Config")]
    public sealed class SliceConfig : ScriptableObject
    {
        [SerializeField] private RegionPreset region;
        [SerializeField] private MayorPersonality mayor;
        [SerializeField] private List<DepartmentDefinition> departments = new();
        [SerializeField] private TravelTimeBandsConfig citywideTravelTimeBands;
        [SerializeField] private EmergencyScenarioConfig emergencyScenario;
        [SerializeField] private CondemnationCaseDefinition courtCase;

        public RegionPreset Region => region;
        public MayorPersonality Mayor => mayor;
        public IReadOnlyList<DepartmentDefinition> Departments => departments;
        public TravelTimeBandsConfig CitywideTravelTimeBands => citywideTravelTimeBands;
        public EmergencyScenarioConfig EmergencyScenario => emergencyScenario;
        public CondemnationCaseDefinition CourtCase => courtCase;
    }
}
