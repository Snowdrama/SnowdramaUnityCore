using UnityEngine;

namespace Snowdrama.SimpleTools
{
    public class LineGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private Transform other;
        [SerializeField] private bool alwaysShow = true;

        private void OnDrawGizmosSelected()
        {
            if (!alwaysShow)
            {
                Gizmos.color = color;
                if (other != null)
                {
                    Gizmos.DrawLine(transform.position, other.transform.position);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (alwaysShow)
            {
                Gizmos.color = color;
                if (other != null)
                {
                    Gizmos.DrawLine(transform.position, other.transform.position);
                }
            }
        }
    }
}