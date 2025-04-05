
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snowdrama.Transition
{

    public enum TransformTransitionEaseType
    {
        Linear,
        EaseInOut,
        EaseInOutElastic,
    }
    public class TransformTransition : Transition
    {
        public Transform startTransform;
        private Vector3 startPosition;
        private Quaternion startRotation;

        public Transform endTransform;
        private Vector3 endPosition;
        private Quaternion endRotation;

        public TransformTransitionEaseType easeInType;
        public TransformTransitionEaseType easeOutType;

        // Start is called before the first frame update
        void Start()
        {
            SetPositionAndRotation();
        }
        public override void OnValidate()
        {
            base.OnValidate();
#if UNITY_EDITOR
            if (startTransform == null)
            {
                var startGo = new GameObject();
                startGo.AddComponent<RectTransform>();
                startGo.name = this.name + "_StartPosition";
                startTransform = startGo.transform;
                startGo.transform.parent = this.transform.parent;
            }
            if (endTransform == null)
            {
                var endGo = new GameObject();
                endGo.AddComponent<RectTransform>();
                endGo.name = this.name + "_EndPosition";
                endTransform = endGo.transform;
                endGo.transform.parent = this.transform.parent;
            }
#endif
            SetPositionAndRotation();
        }
        public void SetPositionAndRotation()
        {
            if (startTransform)
            {
                startPosition = startTransform.position;
                startRotation = startTransform.rotation;
            }

            if (endTransform)
            {
                endPosition = endTransform.position;
                endRotation = endTransform.rotation;
            }
        }
        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            if (hiding)
            {
                Ease(transitionValue, easeInType);
            }
            else
            {
                Ease(transitionValue, easeOutType);
            }
        }

        public void Ease(float transitionValue, TransformTransitionEaseType easeType)
        {
            switch (easeType)
            {
                case TransformTransitionEaseType.Linear:
                    this.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, transitionValue);
                    this.transform.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, transitionValue);

                    break;
                case TransformTransitionEaseType.EaseInOut:
                    this.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, EaseInOut(transitionValue));
                    this.transform.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, EaseInOut(transitionValue));
                    break;
                case TransformTransitionEaseType.EaseInOutElastic:
                    this.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, EaseInOutElastic(transitionValue));
                    this.transform.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, EaseInOutElastic(transitionValue));
                    break;
            }
        }

        public Quaternion EaseInOutElastic(Quaternion start, Quaternion end, float x)
        {
            return Quaternion.LerpUnclamped(start, end, EaseInOutElastic(x));
        }
        public Vector3 EaseInOutElastic(Vector3 start, Vector3 end, float x)
        {
            return Vector3.LerpUnclamped(start, end, EaseInOutElastic(x));
        }
        public float EaseInOutElastic(float x)
        { 
            var c5 = (2.0f * Mathf.PI) / 4.5f;

            if (x == 0)
            {
                return 0;
            }
            else if(x == 1)
            {
                return 1;
            }
            else if(x < 0.5f)
            {
                return -Mathf.Pow(2.0f, 20.0f * x - 10.0f) * Mathf.Sin((20.0f * x - 11.125f) * c5) / 2;
            }
            else
            {
                return Mathf.Pow(2.0f, -20.0f * x + 10.0f) * Mathf.Sin((20.0f * x - 11.125f) * c5) / 2 + 1;
            }
        }









        public float EaseInOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        }
        public float EaseInOut(float x)
        {
            if (x < 0.5f)
            {
                return 4 * x * x * x;
            }
            else
            {
                return 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
            }
        }
    }
}