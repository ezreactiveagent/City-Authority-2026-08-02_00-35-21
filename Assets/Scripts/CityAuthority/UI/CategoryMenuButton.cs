using UnityEngine;
using UnityEngine.UI;

namespace CityAuthority.UI
{
    // One left-bar category button: wraps a UGUI Toggle (selection state and
    // single-select-within-ToggleGroup behavior are Unity's, not reimplemented
    // here) and reports which category it represents to the MenuController.
    [RequireComponent(typeof(Toggle))]
    public sealed class CategoryMenuButton : MonoBehaviour
    {
        [SerializeField] private MenuCategory category;

        public MenuCategory Category => category;
        public Toggle Toggle { get; private set; }

        private MenuController controller;

        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
        }

        public void Initialize(MenuController owningController)
        {
            controller = owningController;
            Toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                controller.SelectCategory(category);
            }
            else if (controller.ActiveCategory == category)
            {
                controller.CloseMenu();
            }
        }
    }
}
