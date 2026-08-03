using UnityEngine;

namespace CityAuthority.Data
{
    // 05 §4, 08 §8: one newspaper outlet only for the slice — no outlet
    // personality drift, competing outlets, or city-owned media required.
    [CreateAssetMenu(fileName = "NewspaperOutlet_", menuName = "City Authority/Data/Newspaper Outlet")]
    public sealed class NewspaperOutlet : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea] [SerializeField] private string description;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
    }
}
