using CityAuthority.Session;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CityAuthority.UI
{
    // Save/Load, wired straight through to ScenarioSession -- the same
    // Save()/Load() calls EmergencyDebugPanel uses, against the same file.
    public sealed class SaveLoadSubmenuView : MonoBehaviour
    {
        [SerializeField] private ScenarioSessionHost sessionHost;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text statusText;

        private ScenarioSession session;

        private void Start()
        {
            session = sessionHost.Session;

            saveButton.onClick.AddListener(() =>
            {
                session.SaveScenario();
                statusText.text = "Saved.";
            });
            loadButton.onClick.AddListener(() =>
            {
                session.LoadScenario();
                statusText.text = "Loaded.";
            });

            Refresh();
        }

        private void Update()
        {
            if (session != null)
            {
                loadButton.interactable = session.HasSaveFile;
            }
        }

        private void Refresh()
        {
            loadButton.interactable = session.HasSaveFile;
            statusText.text = "";
        }
    }
}
