using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.SimpleTools
{
    public class RandomMaterialColorOnStart : MonoBehaviour
    {
        [SerializeField] Gradient gradient;
        // Start is called before the first frame update
        void Start()
        {
            if (this.GetComponent<Renderer>() != null)
            {
                this.GetComponent<Renderer>().material.color = gradient.Evaluate(Random.Range(0.0f, 1.0f));
            }
            if (this.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<SpriteRenderer>().color = gradient.Evaluate(Random.Range(0.0f, 1.0f));
            }
        }
    }
}
