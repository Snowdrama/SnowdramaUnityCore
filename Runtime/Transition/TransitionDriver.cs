using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class TransitionDriver : MonoBehaviour
    {
        [SerializeField]
        private List<Transition> transitionList;
        [SerializeField]
        private Transition currentTransition;

        private void Start()
        {
        }

        private void Update()
        {
            currentTransition.UpdateTransition(SceneController.transitionValue);
        }
    }
}