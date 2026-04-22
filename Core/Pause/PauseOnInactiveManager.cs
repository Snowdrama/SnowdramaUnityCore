using UnityEngine;

public class PauseOnInactiveManager : MonoBehaviour
{
    private void OnEnable()
    {
        Application.runInBackground = Options.GetBoolValue("run_in_background", true);
        Options.RegisterBoolOptionCallback("run_in_background", this.RunInBackgroundChanged);
    }

    private void OnDisable()
    {
        Options.UnregisterBoolOptionCallback("run_in_background", this.RunInBackgroundChanged);
    }

    private void RunInBackgroundChanged(string option, bool changed)
    {
        if (option == "run_in_background")
        {
            Application.runInBackground = changed;
        }
    }
}
