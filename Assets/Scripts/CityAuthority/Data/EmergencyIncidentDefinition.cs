using UnityEngine;

namespace CityAuthority.Data
{
    // 08 §6: the slice's single scripted incident.
    [CreateAssetMenu(fileName = "EmergencyIncident_", menuName = "City Authority/Data/Emergency Incident Definition")]
    public sealed class EmergencyIncidentDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private District targetDistrict;
        [SerializeField] private DepartmentDefinition respondingDepartment;
        [SerializeField] private bool lifeSafetyRisk;
        [TextArea] [SerializeField] private string warningMessage;
        [TextArea] [SerializeField] private string criticalMessage;

        public string Id => id;
        public string DisplayName => displayName;
        public District TargetDistrict => targetDistrict;
        public DepartmentDefinition RespondingDepartment => respondingDepartment;
        public bool LifeSafetyRisk => lifeSafetyRisk;
        public string WarningMessage => warningMessage;
        public string CriticalMessage => criticalMessage;
    }
}
