using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A 2D representation of a spring
namespace Snowdrama.Spring
{
    public class Spring4D
    {
        SpringList springCollection;
        int xID;
        int yID;
        int zID;
        int wID;

        public Vector4 Value
        {
            get
            {
                return new Vector4(
                    springCollection.GetValue(xID), 
                    springCollection.GetValue(yID),
                    springCollection.GetValue(zID),
                    springCollection.GetValue(wID)
                    );
            }
            set
            {
                springCollection.SetValue(xID, value.x);
                springCollection.SetValue(yID, value.y);
                springCollection.SetValue(zID, value.y);
                springCollection.SetValue(wID, value.w);
            }
        }
        public Vector4 Target
        {
            get
            {
                return new Vector4(
                    springCollection.GetTarget(xID),
                    springCollection.GetTarget(yID),
                    springCollection.GetTarget(zID),
                    springCollection.GetTarget(wID)
                    );
            }
            set
            {
                springCollection.SetTarget(xID, value.x);
                springCollection.SetTarget(yID, value.y);
                springCollection.SetTarget(zID, value.y);
                springCollection.SetTarget(wID, value.w);
            }
        }
        public Vector4 Velocity
        {
            get
            {
                return new Vector4(
                    springCollection.GetVelocity(xID),
                    springCollection.GetVelocity(yID),
                    springCollection.GetVelocity(zID),
                    springCollection.GetVelocity(wID)
                    );
            }
            set
            {
                springCollection.SetVelocity(xID, value.x);
                springCollection.SetVelocity(yID, value.y);
                springCollection.SetVelocity(zID, value.y);
                springCollection.SetVelocity(wID, value.w);
            }
        }

        public SpringConfiguration SpringConfig
        {
            set
            {
                springCollection.SetSpringConfig(xID, value);
                springCollection.SetSpringConfig(yID, value);
                springCollection.SetSpringConfig(zID, value);
                springCollection.SetSpringConfig(wID, value);
            }
        }

        public Spring4D(SpringConfiguration config, Vector4 initialValue = default)
        {
            springCollection = new SpringList(3);
            xID = springCollection.Add(initialValue.x, config);
            yID = springCollection.Add(initialValue.y, config);
            zID = springCollection.Add(initialValue.z, config);
            wID = springCollection.Add(initialValue.w, config);
        }

        public void Update(float deltaTime)
        {
            springCollection.Update(deltaTime);
        }
    }
}