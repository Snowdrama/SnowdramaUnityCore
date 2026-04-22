using UnityEngine;

/// <summary>
/// Loads a random sprite on start for a sprite renderer, useful for decorating randomly.
/// </summary>
public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private void Start()
    {
        var renderer = this.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = sprites.GetRandom();
        }
    }
}
