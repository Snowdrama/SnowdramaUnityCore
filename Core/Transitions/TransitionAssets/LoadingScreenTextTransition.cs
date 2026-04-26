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
        [SerializeField] private Color textColor;
        [SerializeField] private TMP_Text text;

        [SerializeField] private LoadingScreenTextObject loadingScreenText;

        [SerializeField] private string format = "Loading: [REPLACE]";
        [SerializeField] private string replace = "[REPLACE]";
        private void Start()
        {
            text = this.GetComponent<TMP_Text>();
            textColor = text.color;
            textColor.a = 0;
            text.color = textColor;
        }

        public override void OnValidate()
        {
            base.OnValidate();
            if (text == null)
            {
                text = this.GetComponent<TMP_Text>();
            }
            textColor = text.color;
            textColor.a = 0;
            text.color = textColor;
        }

        public override void OnTransitionStarted()
        {
            text.text = format.Replace(replace, loadingScreenText.GetLoadingScreenText());
        }


        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            textColor.a = transitionValue;
            text.color = textColor;
        }
    }
}
