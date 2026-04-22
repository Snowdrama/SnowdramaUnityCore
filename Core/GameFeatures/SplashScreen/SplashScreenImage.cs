using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SplashScreenImage : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [Header("Timing")]
    [SerializeField] private float fadeInTime = 0.2f;
    [SerializeField] private float holdTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.2f;

    public float TotalDuration => fadeInTime + holdTime + fadeOutTime;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fadeInClip;
    [SerializeField] private AudioClip fadeOutClip;

    [SerializeField] private float audioDelay = 0f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Behavior")]
    [SerializeField] private float peakThreshold = 0.95f;

    [Header("Scale Animation")]
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float scalePopAmount = 0.1f;
    [SerializeField] private float scaleSmoothSpeed = 8f;

    private bool hasPlayedFadeInAudio = false;
    private bool hasPlayedFadeOutAudio = false;

    private float previousAlpha = 0f;

    private RectTransform rectTransform;
    private Vector3 baseScale;

    protected void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        baseScale = rectTransform.localScale;
        this.UpdateSplashImage(0.0f);
    }

    // Called by controller with local time
    public void UpdateSplashImage(float localTime)
    {
        var alpha = this.EvaluateAlpha(localTime);

        // Apply alpha
        var c = targetImage.color;
        c.a = alpha;
        targetImage.color = c;

        this.HandleAudio(alpha);
        this.HandleScale(alpha);

        previousAlpha = alpha;
    }

    private float EvaluateAlpha(float t)
    {
        if (t < 0f || t > this.TotalDuration)
            return 0f;

        // Fade in
        if (t < fadeInTime)
        {
            var x = t / fadeInTime;
            return Mathf.SmoothStep(0f, 1f, x);
        }

        // Hold
        if (t < fadeInTime + holdTime)
        {
            return 1f;
        }

        // Fade out
        var fadeOutT = (t - (fadeInTime + holdTime)) / fadeOutTime;
        return Mathf.SmoothStep(1f, 0f, fadeOutT);
    }

    private void HandleAudio(float alpha)
    {
        if (audioSource == null) return;

        if (alpha <= 0f)
        {
            hasPlayedFadeInAudio = false;
            hasPlayedFadeOutAudio = false;
            return;
        }

        // Fade-in audio at peak
        if (!hasPlayedFadeInAudio && alpha >= peakThreshold)
        {
            hasPlayedFadeInAudio = true;
            if (fadeInClip != null)
                this.PlayWithVariation(fadeInClip);
        }

        // Fade-out audio
        if (!hasPlayedFadeOutAudio && previousAlpha >= peakThreshold && alpha < peakThreshold)
        {
            hasPlayedFadeOutAudio = true;
            if (fadeOutClip != null)
                this.PlayWithVariation(fadeOutClip);
        }
    }

    private void PlayWithVariation(AudioClip clip)
    {
        var originalPitch = audioSource.pitch;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

        if (audioDelay > 0f)
            audioSource.PlayDelayed(audioDelay);
        else
            audioSource.PlayOneShot(clip);

        audioSource.pitch = originalPitch;
    }

    private void HandleScale(float alpha)
    {
        if (!animateScale || rectTransform == null)
            return;

        var t = Mathf.SmoothStep(0f, 1f, alpha);
        var scaleMultiplier = 1f + (scalePopAmount * t);

        var targetScale = baseScale * scaleMultiplier;

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * scaleSmoothSpeed
        );
    }
}