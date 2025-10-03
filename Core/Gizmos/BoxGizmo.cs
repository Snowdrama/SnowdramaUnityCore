using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.SimpleTools
{
    public class BoxGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private bool wireBox = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            if (wireBox)
            {
                Gizmos.DrawWireCube(transform.position, size);
            }
            else
            {
                Gizmos.DrawCube(transform.position, size);
            }
        }
    }
}