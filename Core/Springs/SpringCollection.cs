
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snowdrama.Spring
{
    public class SpringCollection
    {
        public int Count => _states.Count;

        private Dictionary<string, SpringConfigurationData> _springConfigs;
        private Dictionary<string, SpringState> _states;

        public SpringCollection()
        {
            _springConfigs = new Dictionary<string, SpringConfigurationData>();
            _states = new Dictionary<string, SpringState>();
        }

        public SpringCollection(int capacity)
        {
            _springConfigs = new Dictionary<string, SpringConfigurationData>(capacity);
            _states = new Dictionary<string, SpringState>(capacity);
        }

        public int Add(string springName, float initialValue, SpringConfiguration springConfig)
        {
            var id = Count;
            var state = new SpringState(initialValue, initialValue, 0f);

            _springConfigs.Add(springName, springConfig.GetConfigData());
            _states.Add(springName, state);

            return id;
        }
        public bool IsResting(string id)
        {
            return _states[id].Resting;
        }

        public float GetValue(string id)
        {
            return _states[id].Current;
        }

        public float GetTarget(string id)
        {
            return _states[id].Target;
        }

        public float GetVelocity(string id)
        {
            return _states[id].Velocity;
        }

        public void SetValue(string name, float value)
        {
            var state = _states[name];
            state.Current = value;
            state.Velocity = 0f;
            _states[name] = state;
        }

        public void SetTarget(string name, float value)
        {
            var state = _states[name];
            state.Target = value;
            _states[name] = state;
        }

        public void SetVelocity(string name, float value)
        {
            var state = _states[name];
            state.Velocity = value;
            _states[name] = state;
        }

        public void SetSpringConfig(string name, SpringConfiguration springConfig)
        {
            _springConfigs[name] = springConfig.GetConfigData();
        }

        private void UpdateValue(string name, float deltaTime)
        {
            var state = _states[name];
            var config = _springConfigs[name];

            while (deltaTime >= Mathf.Epsilon)
            {
                var dt = Mathf.Min(deltaTime, 0.016f);

                var force = -config.Tension * (state.Current - state.Target);
                var damping = -config.Friction * state.Velocity;
                var acceleration = (force + damping) / config.Mass;
                state.Velocity = state.Velocity + (acceleration * dt);
                state.Current = state.Current + (state.Velocity * dt);

                if (config.Clamp)
                {
                    if (Mathf.Abs(state.Current - state.Target) < config.Precision)
                    {
                        state.Current = state.Target;
                        state.Velocity = 0f;
                        state.Resting = true;
                        _states[name] = state;
                        return;
                    }
                }
                else
                {
                    if (Mathf.Abs(state.Velocity) < config.Precision && Mathf.Abs(state.Current - state.Target) < config.Precision)
                    {
                        state.Current = state.Target;
                        state.Velocity = 0f;
                        state.Resting = true;
                        _states[name] = state;
                        return;
                    }
                }

                deltaTime -= dt;
            }

            state.Resting = false;

            _states[name] = state;
        }
        public void Update(float deltaTime)
        {
            foreach (var key in _states.Keys)
            {
                UpdateValue(key, deltaTime);
            }
        }
    }
}
