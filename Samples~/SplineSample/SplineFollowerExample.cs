using UnityEngine;
using Snowdrama.Spline;

[ExecuteAlways]
public class SplineFollowerExample : MonoBehaviour
{
    [SerializeField] private Spline spline;
    [SerializeField] private float speed = 1f;

    private enum FollowerLoopType { None, Loop, PingPong }
    [SerializeField] private FollowerLoopType loopType = FollowerLoopType.Loop;
    private bool pingPongDirection;

    [SerializeField, Range(0, 1)] private float t = 0f; // normalized position along spline

    [Header("Debug Gizmos")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private float gizmoSize = 0.5f;

    private void Update()
    {
        if (spline == null || spline.ControlPoints.Count < 2) return;

        if (Application.isPlaying)
        {
            switch (loopType)
            {
                case FollowerLoopType.None:
                    t += Time.deltaTime * speed;
                    t = Mathf.Clamp01(t);
                    break;
                case FollowerLoopType.Loop:
                    t += Time.deltaTime * speed;
                    t %= 1.0f;
                    break;
                case FollowerLoopType.PingPong:
                    if (pingPongDirection)
                    {
                        t += Time.deltaTime * speed;
                        if (t >= 1.0f)
                        {
                            t = 1.0f;
                            pingPongDirection = false;
                        }
                    }
                    else
                    {
                        t -= Time.deltaTime * speed;
                        if (t <= 0.0f)
                        {
                            t = 0.0f;
                            pingPongDirection = true;
                        }

                    }
                    break;
            }
        }
        UpdateFollower();
    }
    private Vector3 forward;
    private Vector3 up;
    private Vector3 right;
    private void UpdateFollower()
    {
        this.transform.position = spline.GetPosition(t);
        forward = spline.GetTangent(t);
        up = spline.GetNormal(t);
        //right = spline.EvaluateBinormal(0, t);
        if (forward != Vector3.zero)
        {
            //if somehow we're zero we shouldn't rotate... <.<
            this.transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebug || spline == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + forward * gizmoSize);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + up * gizmoSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + right * gizmoSize);
    }
}
