using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class SphereGizmo : MonoBehaviour
    {
        [SerializeField] Color color = Color.cyan;
        [SerializeField] float radius = 0.5f;
        [SerializeField] bool wireSphere = false;
        [SerializeField] bool alwaysShow = true;

        private void OnDrawGizmosSelected()
        {
            if (!alwaysShow)
            {
                Gizmos.color = color;
                if (wireSphere)
                {
                    Gizmos.DrawWireSphere(transform.position, radius);
                }
                else
                {
                    Gizmos.DrawSphere(transform.position, radius);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (alwaysShow)
            {
                Gizmos.color = color;
                if (wireSphere)
                {
                    Gizmos.DrawWireSphere(transform.position, radius);
                }
                else
                {
                    Gizmos.DrawSphere(transform.position, radius);
                }
            }
        }
    }
}