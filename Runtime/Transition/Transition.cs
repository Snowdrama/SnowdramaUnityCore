using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public abstract class Transition : MonoBehaviour
    {
        public abstract void UpdateTransition(float transitionValue);
    }
}