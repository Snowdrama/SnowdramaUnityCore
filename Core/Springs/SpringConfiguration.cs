using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Snowdrama.Spring
{
    [CreateAssetMenu(fileName = "Spring", menuName = "Snowdrama/Springs/SpringConfigurationObject", order = 1)]
    public class SpringConfiguration : ScriptableObject
    {
        public float Mass = 1f;
        public float Tension = 170.0f;
        public float Friction = 26.0f;
        public float Precision = 0.01f;
        public bool Clamp = false;
        public Vector2 ClampRange;

        public SpringConfigurationData GetConfigData()
        {
            return new SpringConfigurationData(Mass, Tension, Friction, Precision, Clamp, ClampRange);
        }
    }
    
    //re-added the struct because I want the data to not be allowed to be null. 
    [System.Serializable]
    public struct SpringConfigurationData
    {
        public float Mass;
        public float Tension;
        public float Friction;
        public float Precision;
        public bool Clamp;
        public Vector2 ClampRange;
        public SpringConfigurationData(float Mass = 1f, float Tension = 170.0f, float Friction = 26.0f, float Precision = 0.01f, bool Clamp = false, Vector2 ClampRange = new Vector2())
        {
            this.Mass = Mass;
            this.Tension = Tension;
            this.Friction = Friction;
            this.Precision = Precision;
            this.Clamp = Clamp;
            this.ClampRange = ClampRange;
        }
    }

}
