using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snowdrama.Spring
{
    public struct SpringState
    {
        public float Target;
        public float Current;
        public float Velocity;
        public bool Resting;

        public SpringState(float target, float current, float velocity)
        {
            Target = target;
            Current = current;
            Velocity = velocity;
            Resting = true;
        }
    }
}
