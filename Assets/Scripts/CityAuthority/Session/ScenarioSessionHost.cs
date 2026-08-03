using CityAuthority.Data;
using UnityEngine;

namespace CityAuthority.Session
{
    // Thin scene bridge so both the OnGUI debug panel and the UGUI menu can
    // reference the same live ScenarioSession instance without either one
    // owning or constructing it — a shared peer, not a singleton.
    public sealed class ScenarioSessionHost : MonoBehaviour
    {
        [SerializeField] private SliceConfig sliceConfig;

        public ScenarioSession Session { get; private set; }

        private void Awake()
        {
            if (sliceConfig == null || sliceConfig.EmergencyScenario == null)
            {
                Debug.LogError("ScenarioSessionHost: SliceConfig with an EmergencyScenario must be assigned.");
                enabled = false;
                return;
            }

            Session = new ScenarioSession(sliceConfig);
        }
    }
}
