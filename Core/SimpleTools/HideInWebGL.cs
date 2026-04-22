using UnityEngine;

public class HideInWebGL : MonoBehaviour
{
#if UNITY_WEBGL
    private void OnEnable()
    {
        this.gameObject.SetActive(false);
    }
#endif
}
