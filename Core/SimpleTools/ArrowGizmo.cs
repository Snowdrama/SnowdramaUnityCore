using UnityEngine;

namespace Snowdrama.SimpleTools
{
    public class ArrowGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);
        [SerializeField] private bool alwaysShow = true;

        [SerializeField] private float scale = 0.25f;

        public static Vector3 north = new Vector3(-0.25f, 0, 0);
        public static Vector3 south = new Vector3(0.25f, 0, 0);
        public static Vector3 east = new Vector3(0, 0.25f, 0);
        public static Vector3 west = new Vector3(0, -0.25f, 0);

        public static Vector3 northEast = (north + east) * 0.68f;
        public static Vector3 northWest = (north + west) * 0.68f;
        public static Vector3 southEast = (south + east) * 0.68f;
        public static Vector3 southWest = (south + west) * 0.68f;

        public static Vector3 top = new Vector3(0, 0, 0.5f);
        public static Vector3 bottom = new Vector3(0, 0, -0.5f);

        private Vector3[] points = new Vector3[]
        {
            north + bottom,
            northEast + bottom,

            northEast + bottom,
            east + bottom,

            east + bottom,
            southEast + bottom,

            southEast + bottom,
            south + bottom,

            south + bottom,
            southWest + bottom,

            southWest + bottom,
            west + bottom,

            west + bottom,
            northWest + bottom,

            northWest + bottom,
            north + bottom,



            north + bottom,
            Vector3.zero,

            northEast + bottom,
            Vector3.zero,

            northEast + bottom,
            Vector3.zero,

            east + bottom,
            Vector3.zero,

            east + bottom,
            Vector3.zero,

            southEast + bottom,
            Vector3.zero,

            southEast + bottom,
            Vector3.zero,

            south + bottom,
            Vector3.zero,

            south + bottom,
            Vector3.zero,

            southWest + bottom,
            Vector3.zero,

            southWest + bottom,
            Vector3.zero,

            west + bottom,
            Vector3.zero,

            west + bottom,
            Vector3.zero,

            northWest + bottom,
            Vector3.zero,

            northWest + bottom,
            Vector3.zero,

            north + bottom,
            Vector3.zero,


            //north + bottom,
            //Vector3.zero,

            //east + bottom,
            //Vector3.zero,

            //east + bottom,
            //Vector3.zero,

            //south + bottom,
            //Vector3.zero,

            //south + bottom,
            //Vector3.zero,

            //west + bottom,
            //Vector3.zero,

            //west + bottom,
            //Vector3.zero,

            //north + bottom,
            //Vector3.zero,
        };

        private void OnDrawGizmosSelected()
        {
            if (!alwaysShow)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(transform.position, transform.position + direction);
                if (direction != Vector3.zero)
                {
                    Gizmos.matrix = Matrix4x4.TRS(transform.position + direction, Quaternion.LookRotation(direction), Vector3.one * scale);
                    Gizmos.DrawLineList(points);
                    Gizmos.matrix = Matrix4x4.identity;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (alwaysShow)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(transform.position, transform.position + direction);
                if (direction != Vector3.zero)
                {
                    Gizmos.matrix = Matrix4x4.TRS(transform.position + direction, Quaternion.LookRotation(direction), Vector3.one * scale);
                    Gizmos.DrawLineList(points);
                    Gizmos.matrix = Matrix4x4.identity;
                }
            }
        }
    }
}