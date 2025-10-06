using UnityEngine;
using UnityEngine.UI;

public class TrackedHealthBarElementExample : MonoBehaviour, ITrackedObject
{
    [SerializeField] private Image filledHPImage;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private float currentHealth = 100.0f;
    [SerializeField, EditorReadOnly] private float maxHealth = 100.0f;
    [SerializeField, EditorReadOnly] private float percent = 1.0f;

    public void TrackObject(string trackedName, Vector3 pos, string[] args)
    {
        if (args.Length < 2)
        {
            Debug.LogError($"Message passed to {this.name} needs to have 2 float params, only has {args.Length}");
            return;
        }

        if (!float.TryParse(args[0], out currentHealth))
        {
            Debug.LogError($"Arg[0] in {this.name} could not be parsed into float");
            return;
        }

        if (!float.TryParse(args[1], out maxHealth))
        {
            Debug.LogError($"Arg[1] in {this.name} could not be parsed into float");
            return;
        }

        this.gameObject.SetActive(true);
        //make sure we can never /0 or negative
        currentHealth = currentHealth.Clamp(0.001f, float.MaxValue);
        maxHealth = maxHealth.Clamp(0.001f, float.MaxValue);
        percent = currentHealth / maxHealth;
        filledHPImage.fillAmount = percent;

    }

    public void StopTrackObject(string trackedName)
    {
        this.gameObject.SetActive(false);
    }
}
