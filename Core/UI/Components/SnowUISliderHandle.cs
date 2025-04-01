using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    public class SnowUISliderHandle : MonoBehaviour
    {
        public void SetColor(Color color)
        {
            this.GetComponent<Image>().color = color;
        }
    }
}