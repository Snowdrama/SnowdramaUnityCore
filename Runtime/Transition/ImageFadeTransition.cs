using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class ImageFadeTransition : Transition
    {
        [SerializeField] private float transisionValue;
        [SerializeField] private float hideTransitionSpeed;
        [SerializeField] private float showTransitionSpeed;

        public Image transitionImage;
        public Color transitionColor;

        public override void HideScene(Action completeCallback)
        {
            transisionValue += Time.deltaTime * hideTransitionSpeed;

            transitionColor.a = transisionValue;
            transitionImage.color = transitionColor;

            if (transisionValue >= 1.0f)
            {
                completeCallback?.Invoke();
            }
        }


        public override void ShowScene(Action completeCallback)
        {
            transisionValue -= Time.deltaTime * hideTransitionSpeed;
            transitionColor.a = transisionValue;
            transitionImage.color = transitionColor;
            if (transisionValue <= 0.0f)
            {
                completeCallback?.Invoke();
            }
        }
    }
}