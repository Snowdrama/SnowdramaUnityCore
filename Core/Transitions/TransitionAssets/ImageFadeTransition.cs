using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class ImageFadeTransition : Transition
    {
        [SerializeField] Image transitionImage;
        [SerializeField] Color transitionColor;
        private void Start()
        {
            if(transitionImage == null)
            {
                transitionImage = GetComponent<Image>();
            }
        }
        public override void UpdateTransition(float transitionValue, bool hiding)
        {

            transitionColor.a = transitionValue;
            transitionImage.color = transitionColor;
        }
        public override void OnValidate()
        {
            base.OnValidate();
            if (transitionImage == null)
            {
                transitionImage = GetComponent<Image>();
            }
        }
    }
}