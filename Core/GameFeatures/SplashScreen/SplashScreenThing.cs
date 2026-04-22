using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private SplashScreenImage[] images;

    [Header("Wait after Images")]
    [SerializeField] private float _endDelay = 0.3f;

    [Header("Fade To Black")]
    [SerializeField] private Image _fadeToBlackImage;
    [SerializeField] private float _fadeToBlackTime = 0.5f;

    [Header("SceneTransition")]
    [SerializeField] private SceneController_TransitionKeyObject _goToSceneObject;


    //local stuff
    [Header("Debug")]
    [SerializeField, EditorReadOnly] private float _currentEndDelay = 0.3f;
    [SerializeField, EditorReadOnly] private float _currentFadeToBlackTime = 0.3f;
    [SerializeField, EditorReadOnly] private bool hasTransitioned = false;
    [SerializeField, EditorReadOnly] private int splashIndex = 0;
    [SerializeField, EditorReadOnly] private float localTime = 0.0f;
    [SerializeField, EditorReadOnly] private SplashScreenImage currentImage;

    private void Start()
    {
        splashIndex = 0;
        localTime = 0.0f;
        _currentEndDelay = 0.0f;
        _currentFadeToBlackTime = 0.0f;
    }


    private void Update()
    {
        if (images == null || images.Length == 0)
            return;

        //get the current image if there's null
        if (currentImage == null)
        {
            currentImage = images[splashIndex];
        }

        //this should be okay
        if (currentImage == null)
        {
            Debug.LogError($"Splash thing Ran out of images to play!");
            return;
        }

        //count up to the duration of the image
        if (localTime <= currentImage.TotalDuration)
        {
            localTime += Time.deltaTime;
            currentImage.UpdateSplashImage(localTime);
            return;
        }

        //can we go to the next one?
        if (localTime >= currentImage.TotalDuration && splashIndex + 1 < images.Length)
        {
            localTime = 0.0f;
            currentImage = images[++splashIndex];
            return;
        }

        //we're out of images
        //start the wait
        if (_currentEndDelay <= _endDelay)
        {
            _currentEndDelay += Time.deltaTime;
            return;
        }

        //we're done waiting
        //skip fading to black if we don't assign it
        if (_fadeToBlackImage != null)
        {
            if (_currentFadeToBlackTime <= _fadeToBlackTime)
            {
                _currentFadeToBlackTime += Time.deltaTime;
                var color = _fadeToBlackImage.color;
                color.a = Mathf.Lerp(0, 1.0f, _currentFadeToBlackTime / _fadeToBlackTime);
                _fadeToBlackImage.color = color;
                return;
            }
        }

        //we're done fading to black. 
        // Transition
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            _goToSceneObject.GoToScene();
        }
    }
}