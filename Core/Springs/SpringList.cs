using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snowdrama.Spring
{
    public class SpringList
    {
        public int Count => _states.Count;

        private List<SpringConfigurationData> _springConfigs;
        private List<SpringState> _states;

        public SpringList()
        {
            _springConfigs = new List<SpringConfigurationData>();
            _states = new List<SpringState>();
        }

        public SpringList(int capacity)
        {
            _springConfigs = new List<SpringConfigurationData>(capacity);
            _states = new List<SpringState>(capacity);
        }

        public int Add(float initialValue, SpringConfiguration springConfig)
        {
            var id = Count;
            var state = new SpringState(initialValue, initialValue, 0f);

            _springConfigs.Add(springConfig.GetConfigData());
            _states.Add(state);

            return id;
        }
        public bool IsResting(int id)
        {
            return _states[id].Resting;
        }

        public float GetValue(int id)
        {
            return _states[id].Current;
        }
        
        public float GetTarget(int id)
        {
            return _states[id].Target;
        }

        public float GetVelocity(int id)
        {
            return _states[id].Velocity;
        }

        public void SetValue(int id, float value)
        {
            var state = _states[id];
            state.Current = value;
            state.Velocity = 0f;
            _states[id] = state;
        }

        public void SetTarget(int id, float value)
        {
            var state = _states[id];
            state.Target = value;
            _states[id] = state;
        }

        public void SetVelocity(int id, float value)
        {
            var state = _states[id];
            state.Velocity = value;
            _states[id] = state;
        }

        public void SetSpringConfig(int id, SpringConfiguration springConfig)
        {
            _springConfigs[id] = springConfig.GetConfigData();
        }

        private void UpdateValue(int id, float deltaTime)
        {
            var state = _states[id];
            var config = _springConfigs[id];

            while (deltaTime >= Mathf.Epsilon)
            {
                var dt = Mathf.Min(deltaTime, 0.016f);

                var force = -config.Tension * (state.Current - state.Target);
                var damping = -config.Friction * state.Velocity;
                var acceleration = (force + damping) / config.Mass;
                state.Velocity = state.Velocity + (acceleration * dt);
                state.Current = state.Current + (state.Velocity * dt);

                if (Mathf.Abs(state.Velocity) < config.Precision && Mathf.Abs(state.Current - state.Target) < config.Precision)
                {
                    state.Current = state.Target;
                    state.Velocity = 0f;
                    state.Resting = true;
                    _states[id] = state;
                    return;
                }
                else
                {
                    state.Resting = false;
                }

                if (config.Clamp)
                {
                    state.Current = Mathf.Clamp(state.Current, config.ClampRange.x, config.ClampRange.y);
                }

                deltaTime -= dt;
            }


            _states[id] = state;
        }
        public void Update(float deltaTime)
        {
            for (var i = 0; i < Count; i++)
            {
                UpdateValue(i, deltaTime);
            }
        }
    }
}
