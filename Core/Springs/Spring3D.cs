using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A 2D representation of a spring
namespace Snowdrama.Spring
{
    public class Spring3D
    {
        SpringList springCollection;
        int xID;
        int yID;
        int zID;

        public Vector3 Value
        {
            get
            {
                return new Vector3(springCollection.GetValue(xID), springCollection.GetValue(yID), springCollection.GetValue(zID));
            }
            set
            {
                springCollection.SetValue(xID, value.x);
                springCollection.SetValue(yID, value.y);
                springCollection.SetValue(zID, value.y);
            }
        }
        public Vector3 Target
        {
            get
            {
                return new Vector3(springCollection.GetTarget(xID), springCollection.GetTarget(yID), springCollection.GetTarget(zID));
            }
            set
            {
                springCollection.SetTarget(xID, value.x);
                springCollection.SetTarget(yID, value.y);
                springCollection.SetTarget(zID, value.y);
            }
        }
        public Vector3 Velocity
        {
            get
            {
                return new Vector3(springCollection.GetVelocity(xID), springCollection.GetVelocity(yID), springCollection.GetVelocity(zID));
            }
            set
            {
                springCollection.SetVelocity(xID, value.x);
                springCollection.SetVelocity(yID, value.y);
                springCollection.SetVelocity(zID, value.y);
            }
        }

        public SpringConfiguration SpringConfig
        {
            set
            {
                springCollection.SetSpringConfig(xID, value);
                springCollection.SetSpringConfig(yID, value);
                springCollection.SetSpringConfig(zID, value);
            }
        }

        public Spring3D(SpringConfiguration config, Vector3 initialValue = default)
        {
            springCollection = new SpringList(3);
            xID = springCollection.Add(initialValue.x, config);
            yID = springCollection.Add(initialValue.y, config);
            zID = springCollection.Add(initialValue.z, config);
        }

        public void Update(float deltaTime)
        {
            springCollection.Update(deltaTime);
        }
    }
}
