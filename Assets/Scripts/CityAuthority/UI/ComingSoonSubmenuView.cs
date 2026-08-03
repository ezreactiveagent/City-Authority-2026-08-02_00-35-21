using TMPro;
using UnityEngine;

namespace CityAuthority.UI
{
    // Placeholder submenu for categories not wired to real functionality yet
    // in this first pass (Development, Court, Media, Reports) -- reused
    // instead of four near-identical hand-typed panels.
    public sealed class ComingSoonSubmenuView : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private string headerLabel = "Coming soon";

        private void OnEnable()
        {
            headerText.text = headerLabel;
        }
    }
}
