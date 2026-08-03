using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    [Serializable]
    public sealed class RegionNarrativeProfile
    {
        [TextArea] [SerializeField] private string visualIdentityNotes;
        [TextArea] [SerializeField] private string climateNotes;
        [TextArea] [SerializeField] private string infrastructureAssumptions;
        [TextArea] [SerializeField] private string planningRules;
        [TextArea] [SerializeField] private string economicTendencies;
        [TextArea] [SerializeField] private string environmentalPressures;
        [TextArea] [SerializeField] private string developmentPatterns;

        public string VisualIdentityNotes => visualIdentityNotes;
        public string ClimateNotes => climateNotes;
        public string InfrastructureAssumptions => infrastructureAssumptions;
        public string PlanningRules => planningRules;
        public string EconomicTendencies => economicTendencies;
        public string EnvironmentalPressures => environmentalPressures;
        public string DevelopmentPatterns => developmentPatterns;
    }

    [CreateAssetMenu(fileName = "RegionPreset_", menuName = "City Authority/Data/Region Preset")]
    public sealed class RegionPreset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private List<ZoningCategory> availableZoningCategories = new();
        [Min(0)] [SerializeField] private float baseLandValueIndex = 1f;
        [SerializeField] private TravelTimeBandsConfig travelTimeBandsOverride;
        [SerializeField] private EmergencyGenerationMode disasterGenerationMode = EmergencyGenerationMode.ScriptedOnly;
        [SerializeField] private RegionNarrativeProfile narrativeProfile = new();

        public string Id => id;
        public string DisplayName => displayName;
        public IReadOnlyList<ZoningCategory> AvailableZoningCategories => availableZoningCategories;
        public float BaseLandValueIndex => baseLandValueIndex;
        public TravelTimeBandsConfig TravelTimeBandsOverride => travelTimeBandsOverride;
        public EmergencyGenerationMode DisasterGenerationMode => disasterGenerationMode;
        public RegionNarrativeProfile NarrativeProfile => narrativeProfile;
    }
}
