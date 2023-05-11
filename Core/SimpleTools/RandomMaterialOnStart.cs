using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class RandomMaterialOnStart : MonoBehaviour
    {
        public List<Material> materialOptions;
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Renderer>().material = materialOptions[Random.Range(0, materialOptions.Count)];
        }
    }
}