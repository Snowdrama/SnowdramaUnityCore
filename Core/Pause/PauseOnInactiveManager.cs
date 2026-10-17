using UnityEngine;
namespace Snowdrama
{
    public class PauseOnInactiveManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Application.runInBackground = Options.Get<bool>("run_in_background", true);
            Options.SetCallback<bool>("run_in_background", this.RunInBackgroundChanged);
        }

        private void OnDisable()
        {
            Options.UnsetCallback<bool>("run_in_background", this.RunInBackgroundChanged);
        }

        private void RunInBackgroundChanged(bool changed)
        {
            Application.runInBackground = changed;
        }
    }
}
