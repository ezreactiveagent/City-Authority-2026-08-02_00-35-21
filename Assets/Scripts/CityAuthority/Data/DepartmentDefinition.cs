using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.Data
{
    [Serializable]
    public sealed class DepartmentResourceDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [Min(0)] [SerializeField] private int count;
        [TextArea] [SerializeField] private string notes;

        public string Id => id;
        public string DisplayName => displayName;
        public int Count => count;
        public string Notes => notes;
    }

    // 10 §4: "home coverage spans both the incident's location and one adjacent
    // district within the Reduced band" — the base (uncommitted, no concurrent
    // demand) travel time from this department's facility to a given district.
    [Serializable]
    public sealed class DistrictCoverage
    {
        [SerializeField] private District district;
        [Min(0)] [SerializeField] private float baseTravelTimeMinutes;

        public District District => district;
        public float BaseTravelTimeMinutes => baseTravelTimeMinutes;
    }

    [CreateAssetMenu(fileName = "Department_", menuName = "City Authority/Data/Department Definition")]
    public sealed class DepartmentDefinition : ScriptableObject
    {
        [SerializeField] private DepartmentType departmentType;
        [SerializeField] private string displayName;
        [SerializeField] private string facilityTypeName;
        [Min(1)] [SerializeField] private int startingFacilityCount = 1;
        [SerializeField] private OperatingPolicyLevel operatingPolicy = OperatingPolicyLevel.Standard;
        [SerializeField] private List<DepartmentResourceDefinition> resources = new();
        [SerializeField] private List<DistrictCoverage> coverageAreas = new();
        [TextArea] [SerializeField] private string roleDescription;

        public DepartmentType DepartmentType => departmentType;
        public string DisplayName => displayName;
        public string FacilityTypeName => facilityTypeName;
        public int StartingFacilityCount => startingFacilityCount;
        public OperatingPolicyLevel OperatingPolicy => operatingPolicy;
        public IReadOnlyList<DepartmentResourceDefinition> Resources => resources;
        public IReadOnlyList<DistrictCoverage> CoverageAreas => coverageAreas;
        public string RoleDescription => roleDescription;
    }
}
