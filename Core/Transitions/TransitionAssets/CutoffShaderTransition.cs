using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class CutoffShaderTransition : Transition
    {
        [Header("Image and Material")]
        [SerializeField] Image transitionImage;
        Material transitionMaterial;

        [Header("Main Texture")]
        [SerializeField] bool changeMainTextureOnStart;
        [SerializeField] List<Sprite> transitionSprites;

        [Header("Patterns")]
        [SerializeField] bool changePatternOnStart;
        [SerializeField] bool changePatternMidTransition;
        [SerializeField] List<Texture2D> transitionPatterns;

        private void Start()
        {
            if (transitionImage == null)
            {
                transitionImage = GetComponent<Image>();
            }
            transitionMaterial = transitionImage.material;
        }
        public override void OnTransitionStarted()
        {
            if (changeMainTextureOnStart)
            {
                ChangeTexture();
            }


            if (changePatternOnStart)
            {
                ChangePattern();
            }
        }

        public override void OnHideCompleted()
        {
            if (changePatternMidTransition)
            {
                ChangePattern();
            }
        }

        public void ChangePattern()
        {

            if (transitionPatterns.Count > 0)
            {
                transitionMaterial.SetTexture("_PatternTex", transitionPatterns[Random.Range(0, transitionPatterns.Count)]);
            }
        }
        public void ChangeTexture()
        {
            if (transitionSprites.Count > 0)
            {
                transitionImage.sprite = transitionSprites[Random.Range(0, transitionSprites.Count)];
            }
        }
        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            transitionMaterial.SetFloat("_Cutoff", transitionValue);
        }

        public override void OnValidate()
        {
            base.OnValidate();
            if (transitionImage == null)
            {
                transitionImage = GetComponent<Image>();
            }
            transitionMaterial = transitionImage.material;
        }

    }
}