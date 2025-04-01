using Snowdrama.Transition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Snowdrama.Transition
{
    [RequireComponent(typeof(TMP_Text))]
    public class LoadingScreenTextTransition : Transition
    {
        public Color textColor;
        private TMP_Text text;

        public LoadingScreenTextObject loadingScreenText;

        void Start()
        {
            text = this.GetComponent<TMP_Text>();
            textColor = text.color;
            textColor.a = 0;
        }

        public override void OnValidate()
        {
            base.OnValidate();
            if (text == null)
            {
                text = this.GetComponent<TMP_Text>();
            }
            textColor.a = 0;
        }

        public override void OnTransitionStarted()
        {
            text.text = loadingScreenText.GetLoadingScreenText();
        }


        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            textColor.a = transitionValue;
            text.color = textColor;
        }
    }
}
