using UnityEngine;
using UnityEngine.UI;
namespace Snowdrama
{
    public class SetOptionToggle : MonoBehaviour
    {
        public string toggleName;
        public bool defaultToggle;
        private Toggle toggle;
        private void Start()
        {
            toggle = this.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(this.OnValueChanged);
            toggle.SetIsOnWithoutNotify(Options.Get_bool(toggleName, defaultToggle));
        }

        public void OnValueChanged(bool value)
        {
            Options.Set_bool(toggleName, value);
        }
    }
}
