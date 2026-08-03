using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityAuthority.UI
{
    // Pure UI coordination for the left-side toggle menu: tracks which
    // category is open and shows/hides the matching submenu root. Owns no
    // scenario logic at all — that lives in ScenarioSession, read/acted on
    // directly by each submenu view.
    public sealed class MenuController : MonoBehaviour
    {
        [Serializable]
        private sealed class CategoryEntry
        {
            public CategoryMenuButton button;
            public GameObject submenuRoot;
        }

        [SerializeField] private List<CategoryEntry> entries = new();

        public MenuCategory? ActiveCategory { get; private set; }

        private void Start()
        {
            foreach (var entry in entries)
            {
                entry.button.Initialize(this);
                entry.submenuRoot.SetActive(false);
            }
        }

        public void SelectCategory(MenuCategory category)
        {
            ActiveCategory = category;
            foreach (var entry in entries)
            {
                entry.submenuRoot.SetActive(entry.button.Category == category);
            }
        }

        public void CloseMenu()
        {
            ActiveCategory = null;
            foreach (var entry in entries)
            {
                entry.submenuRoot.SetActive(false);
            }
        }
    }
}
