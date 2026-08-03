using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    // Ties the slice's districts to its one scripted incident, per 08 §6 / 09 §7.
    [CreateAssetMenu(fileName = "EmergencyScenario_", menuName = "City Authority/Data/Emergency Scenario Config")]
    public sealed class EmergencyScenarioConfig : ScriptableObject
    {
        [SerializeField] private List<District> districts = new();
        [SerializeField] private EmergencyIncidentDefinition incident;

        public IReadOnlyList<District> Districts => districts;
        public EmergencyIncidentDefinition Incident => incident;
    }
}
