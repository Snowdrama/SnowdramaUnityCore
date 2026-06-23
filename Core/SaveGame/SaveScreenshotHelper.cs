using UnityEngine;

public class SaveScreenshotHelper : MonoBehaviour
{
    public static Texture2D lastScreenshot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Bootstrap()
    {
        //set the screenshot to some defaultImage
        var defaultSaveTexture = Resources.Load<Texture2D>("DefaultSaveTexture");
        if (defaultSaveTexture != null)
        {
            lastScreenshot = defaultSaveTexture;
        }
    }
    private float currentTime = 0;
    private bool forceTakeScreenshot;
    private void LateUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0f)
        {
            currentTime += 5.0f;
            this.Screenshot();
        }

        if (forceTakeScreenshot)
        {
            forceTakeScreenshot = false;
            this.Screenshot();
        }

    }
    private TakeScreenshotMesage TakeScreenshotMesage;
    private void OnEnable()
    {
        TakeScreenshotMesage = Messages.Get<TakeScreenshotMesage>();
        TakeScreenshotMesage.AddListener(this.TakeScreenshot);
    }

    private void OnDisable()
    {
        TakeScreenshotMesage.RemoveListener(this.TakeScreenshot);
        TakeScreenshotMesage = null;
        Messages.Return<TakeScreenshotMesage>();
    }

    private void TakeScreenshot()
    {
        currentTime += 30.0f;
        forceTakeScreenshot = true;
    }

    private void Screenshot()
    {
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        if (texture != null)
        {
            //clean up memory
            if (lastScreenshot != null)
            {
                Destroy(lastScreenshot);
            }

            lastScreenshot = texture;
        }
    }

    private Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        var tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}

public class TakeScreenshotMesage : AMessage { }