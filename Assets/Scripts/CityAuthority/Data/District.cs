using UnityEngine;

namespace CityAuthority.Data
{
    [CreateAssetMenu(fileName = "District_", menuName = "City Authority/Data/District")]
    public sealed class District : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        public string Id => id;
        public string DisplayName => displayName;
    }
}
