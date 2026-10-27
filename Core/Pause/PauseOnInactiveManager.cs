using UnityEngine;
namespace Snowdrama
{
    public class PauseOnInactiveManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Application.runInBackground = Options.Get_bool("run_in_background", true);
            Options.RegisterCallback_bool("run_in_background", this.RunInBackgroundChanged);
        }

        private void OnDisable()
        {
            Options.UnregisterCallback_bool("run_in_background", this.RunInBackgroundChanged);
        }

        private void RunInBackgroundChanged(bool changed)
        {
            Application.runInBackground = changed;
        }
    }
}
