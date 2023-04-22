using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class ImageFadeTransition : Transition
    {
        [SerializeField] private float hideTransitionSpeed;
        [SerializeField] private float showTransitionSpeed;

        public Image transitionImage;
        public Color transitionColor;

        public override void UpdateTransition(float transitionValue)
        {

            transitionColor.a = transitionValue;
            transitionImage.color = transitionColor;
        }
    }
}