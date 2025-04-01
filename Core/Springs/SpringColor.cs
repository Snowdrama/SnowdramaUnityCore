using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Spring
{
    public class SpringColor
    {
        SpringList springCollection;
        int rID;
        int gID;
        int bID;
        int aID;

        public Color Value
        {
            get
            {
                return new Color(
                    springCollection.GetValue(rID),
                    springCollection.GetValue(gID),
                    springCollection.GetValue(bID),
                    springCollection.GetValue(aID)
                    );
            }
            set
            {
                springCollection.SetValue(rID, value.r);
                springCollection.SetValue(gID, value.g);
                springCollection.SetValue(bID, value.b);
                springCollection.SetValue(aID, value.a);
            }
        }
        public Color Target
        {
            get
            {
                return new Color(
                    springCollection.GetTarget(rID),
                    springCollection.GetTarget(gID),
                    springCollection.GetTarget(bID),
                    springCollection.GetTarget(aID)
                    );
            }
            set
            {
                springCollection.SetTarget(rID, value.r);
                springCollection.SetTarget(gID, value.g);
                springCollection.SetTarget(bID, value.b);
                springCollection.SetTarget(aID, value.a);
            }
        }
        public Color Velocity
        {
            get
            {
                return new Color(
                    springCollection.GetVelocity(rID),
                    springCollection.GetVelocity(gID),
                    springCollection.GetVelocity(bID),
                    springCollection.GetVelocity(aID)
                    );
            }
            set
            {
                springCollection.SetVelocity(rID, value.r);
                springCollection.SetVelocity(gID, value.g);
                springCollection.SetVelocity(bID, value.b);
                springCollection.SetVelocity(aID, value.a);
            }
        }

        public SpringConfiguration SpringConfig
        {
            set
            {
                springCollection.SetSpringConfig(rID, value);
                springCollection.SetSpringConfig(gID, value);
                springCollection.SetSpringConfig(bID, value);
                springCollection.SetSpringConfig(aID, value);
            }
        }

        public SpringColor(SpringConfiguration config, Color initialValue = default)
        {
            springCollection = new SpringList(3);
            rID = springCollection.Add(initialValue.r, config);
            gID = springCollection.Add(initialValue.g, config);
            bID = springCollection.Add(initialValue.b, config);
            aID = springCollection.Add(initialValue.a, config);
        }

        public void Update(float deltaTime)
        {
            springCollection.Update(deltaTime);
        }
    }
}