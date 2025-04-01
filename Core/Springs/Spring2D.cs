using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A 2D representation of a spring
namespace Snowdrama.Spring
{
    public class Spring2D
    {
        SpringList springCollection;
        int xID;
        int yID;

        public Vector2 Value
        {
            get
            {
                return new Vector2(springCollection.GetValue(xID), springCollection.GetValue(yID));
            }
            set
            {
                springCollection.SetValue(xID, value.x);
                springCollection.SetValue(yID, value.y);
            }
        }
        public Vector2 Target
        {
            get
            {
                return new Vector2(springCollection.GetTarget(xID), springCollection.GetTarget(yID));
            }
            set
            {
                springCollection.SetTarget(xID, value.x);
                springCollection.SetTarget(yID, value.y);
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return new Vector2(springCollection.GetVelocity(xID), springCollection.GetVelocity(yID));
            }
            set
            {
                springCollection.SetVelocity(xID, value.x);
                springCollection.SetVelocity(yID, value.y);
            }
        }

        public SpringConfiguration SpringConfig
        {
            set
            {
                springCollection.SetSpringConfig(xID, value);
                springCollection.SetSpringConfig(yID, value);
            }
        }

        public Spring2D(SpringConfiguration config, Vector2 initialValue = default)
        {
            springCollection = new SpringList();
            xID = springCollection.Add(initialValue.x, config);
            yID = springCollection.Add(initialValue.y, config);
        }

        public void Update(float deltaTime)
        {
            springCollection.Update(deltaTime);
        }
    }
}
