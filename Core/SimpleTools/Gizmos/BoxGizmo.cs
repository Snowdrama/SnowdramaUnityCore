using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class BoxGizmo : MonoBehaviour
    {
        [SerializeField] Color color = Color.cyan;
        [SerializeField] Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] bool wireBox = false;

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