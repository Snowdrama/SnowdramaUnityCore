using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.SimpleTools
{
    public class SphereGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private bool wireSphere = false;
        [SerializeField] private bool alwaysShow = true;

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