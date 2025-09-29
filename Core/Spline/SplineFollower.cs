using UnityEngine;

namespace Snowdrama.Spline
{
    [RequireComponent(typeof(Transform))]
    [ExecuteAlways]
    public class SplineFollower : MonoBehaviour
    {
        public enum FollowMode { Once, Loop, PingPong }
        public enum SpeedMode { Normalized, WorldUnits }

        [Header("Spline Settings")]
        public Spline spline;

        [Tooltip("Normalized [0..1] position on the spline.")]
        [Range(0f, 1f)]
        public float normalizedT = 0f;

        public FollowMode followMode = FollowMode.Loop;

        [Header("Movement")]
        public SpeedMode speedMode = SpeedMode.WorldUnits;
        [Tooltip("If SpeedMode == Normalized, this is normalized units/sec. If WorldUnits, this is meters/sec.")]
        public float speed = 1f;

        [Tooltip("How many samples to build the arc-length table. More samples => more accurate world-unit movement.")]
        [Range(16, 8192)]
        public int arcSamples = 1024;

        [Header("Orientation")]
        public bool orientToTangent = true;
        [Tooltip("If true, use the spline's normal as the up vector (smoothed to avoid flips). Otherwise uses world up.")]
        public bool useNormalForUp = true;
        [Tooltip("Degrees per second the follower will rotate to match the spline orientation.")]
        public float rotationSpeed = 360f;

        [Header("Misc")]
        [Tooltip("Automatically rebuild the arc-length table when the spline or its control points change.")]
        public bool autoRebuildOnChange = true;

        public bool followInEditor = false;

        // internal
        private float direction = 1f;
        private Spline _lastSpline = null;
        private int _lastControlPointsChecksum = 0;
        private int _lastArcSamples = 0;
        private float[] _tSamples;
        private float[] _cumLengths;
        private float _totalLength = 0f;

        // track previous up to avoid flipping
        private Vector3 _prevUp;

        private void Awake()
        {
            if (spline == null)
                return;

            EnsureTable();
            // Initialize prevUp from current transform or spline normal if available
            _prevUp = (useNormalForUp && spline != null) ? spline.GetNormal(normalizedT).normalized : transform.up;
        }

        private void OnValidate()
        {
            // keep values valid in editor
            arcSamples = Mathf.Clamp(arcSamples, 16, 8192);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            speed = Mathf.Max(0f, speed);
            // mark rebuild if inspector changes
            _lastArcSamples = -1;
        }

        private void Update()
        {
            if (spline == null) return;

            // Rebuild arc-length table when needed (safeguard)
            if (autoRebuildOnChange)
                EnsureTable();

            if (followInEditor || Application.isPlaying)
            {
                // Movement
                if (speedMode == SpeedMode.Normalized)
                {
                    float deltaT = speed * Time.deltaTime * direction;
                    AdvanceNormalized(deltaT);
                }
                else // WorldUnits
                {
                    if (_totalLength <= 1e-6f)
                    {
                        // fallback: if length is zero, do nothing
                    }
                    else
                    {
                        float distanceToMove = speed * Time.deltaTime * direction;
                        float newT = GetTAtDistanceFrom(normalizedT, distanceToMove);
                        // Compute candidate normalizedT; Set direction changes for pingpong are handled inside GetTAtDistanceFrom
                        normalizedT = newT;
                        // Keep normalized in [0,1] for safety
                        if (followMode == FollowMode.Loop)
                            normalizedT = Mathf.Repeat(normalizedT, 1f);
                        else
                            normalizedT = Mathf.Clamp01(normalizedT);
                    }
                }
            }

            // Position
            Vector3 pos = spline.GetPosition(Mathf.Clamp01(normalizedT));
            transform.position = pos;

            // Orientation (smooth)
            if (orientToTangent)
            {
                Vector3 tangent = spline.GetTangent(Mathf.Clamp01(normalizedT)).normalized;
                if (tangent.sqrMagnitude > 1e-6f)
                {
                    Vector3 desiredUp = useNormalForUp ? spline.GetNormal(Mathf.Clamp01(normalizedT)).normalized : Vector3.up;

                    // Ensure continuity: pick the normal that is closest to previous up to avoid 180 flips
                    if (Vector3.Dot(_prevUp, desiredUp) < 0f)
                        desiredUp = -desiredUp;

                    Quaternion targetRot = Quaternion.LookRotation(tangent, desiredUp);

                    // Smooth rotate toward target by rotationSpeed (deg/sec)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

                    // Update prevUp from actual applied rotation so smoothing remains consistent
                    _prevUp = transform.rotation * Vector3.up;
                }
            }
        }

        private void AdvanceNormalized(float deltaT)
        {
            normalizedT += deltaT;

            switch (followMode)
            {
                case FollowMode.Once:
                    normalizedT = Mathf.Clamp01(normalizedT);
                    break;
                case FollowMode.Loop:
                    normalizedT = Mathf.Repeat(normalizedT, 1f);
                    break;
                case FollowMode.PingPong:
                    if (normalizedT > 1f)
                    {
                        normalizedT = 1f - (normalizedT - 1f);
                        direction *= -1f;
                    }
                    else if (normalizedT < 0f)
                    {
                        normalizedT = -normalizedT;
                        direction *= -1f;
                    }
                    break;
            }
        }

        #region Arc-length table and helpers

        [ContextMenu("Rebuild Arc Length Table")]
        public void RebuildArcLengthTable()
        {
            BuildArcLengthTable();
        }

        private void EnsureTable()
        {
            if (spline == null) return;

            // detect changes (spline object change or control points checksum or sample count changed)
            bool needRebuild = false;

            if (_lastSpline != spline) needRebuild = true;
            if (_lastArcSamples != arcSamples) needRebuild = true;

            int checksum = ComputeControlPointsChecksum();
            if (_lastControlPointsChecksum != checksum) needRebuild = true;

            if (needRebuild)
                BuildArcLengthTable();

            // update trackers
            _lastSpline = spline;
            _lastArcSamples = arcSamples;
            _lastControlPointsChecksum = checksum;
        }

        private int ComputeControlPointsChecksum()
        {
            if (spline == null || spline.ControlPoints == null) return 0;
            unchecked
            {
                int h = spline.ControlPoints.Count;
                for (int i = 0; i < spline.ControlPoints.Count; i++)
                {
                    // use each Vector3's hashcode to generate a checksum; GetHashCode on float bits is fine for change detection
                    h = (h * 397) ^ spline.ControlPoints[i].GetHashCode();
                }
                // include twist count as well (in case twists change)
                if (spline.ControlPointTwists != null)
                {
                    for (int i = 0; i < spline.ControlPointTwists.Count; i++)
                        h = (h * 397) ^ spline.ControlPointTwists[i].GetHashCode();
                }
                // include closed/resolution
                h = (h * 397) ^ spline.Closed.GetHashCode();
                h = (h * 397) ^ spline.Resolution.GetHashCode();
                return h;
            }
        }

        private void BuildArcLengthTable()
        {
            if (spline == null) return;
            int samples = Mathf.Clamp(arcSamples, 16, 8192);

            _tSamples = new float[samples + 1];
            _cumLengths = new float[samples + 1];

            Vector3 prev = spline.GetPosition(0f);
            _tSamples[0] = 0f;
            _cumLengths[0] = 0f;
            float step = 1f / samples;
            float acc = 0f;

            for (int i = 1; i <= samples; i++)
            {
                float t = i * step;
                if (t > 1f) t = 1f;
                _tSamples[i] = t;
                Vector3 p = spline.GetPosition(t);
                float d = Vector3.Distance(prev, p);
                acc += d;
                _cumLengths[i] = acc;
                prev = p;
            }

            _totalLength = _cumLengths[samples];
            if (_totalLength <= 0f)
            {
                // protect against zero length spline
                _totalLength = 0f;
            }
        }

        /// <summary>
        /// Returns the cumulative arc length at normalized t (interpolated).
        /// </summary>
        private float LengthAtT(float t)
        {
            if (_cumLengths == null || _cumLengths.Length == 0) return 0f;
            t = Mathf.Clamp01(t);
            int samples = _tSamples.Length - 1;
            float raw = t * samples;
            int idx = Mathf.FloorToInt(raw);
            idx = Mathf.Clamp(idx, 0, samples - 1);
            float frac = raw - idx;
            return Mathf.Lerp(_cumLengths[idx], _cumLengths[idx + 1], frac);
        }

        /// <summary>
        /// Finds normalized t such that cumulative length == targetLength (clamped or wrapped depending on followMode).
        /// </summary>
        private float TAtLength(float targetLength)
        {
            if (_cumLengths == null || _cumLengths.Length == 0) return normalizedT;
            if (_totalLength <= 0f) return normalizedT;

            // Clamp/wrap depending on follow mode & direction
            if (followMode == FollowMode.Loop)
            {
                // wrap to [0,total)
                targetLength = Mathf.Repeat(targetLength, _totalLength);
            }
            else if (followMode == FollowMode.Once)
            {
                targetLength = Mathf.Clamp(targetLength, 0f, _totalLength);
            }
            else if (followMode == FollowMode.PingPong)
            {
                // reflect and flip direction if necessary
                if (targetLength > _totalLength)
                {
                    float overflow = targetLength - _totalLength;
                    targetLength = _totalLength - overflow;
                    direction *= -1f;
                }
                else if (targetLength < 0f)
                {
                    float under = -targetLength;
                    targetLength = under;
                    direction *= -1f;
                }

                // Clamp in case overflow still larger than length
                targetLength = Mathf.Clamp(targetLength, 0f, _totalLength);
            }

            // Binary search in _cumLengths to find interval
            int lo = 0;
            int hi = _cumLengths.Length - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                float val = _cumLengths[mid];
                if (val < targetLength) lo = mid + 1;
                else hi = mid - 1;
            }

            int index = Mathf.Clamp(lo - 1, 0, _cumLengths.Length - 2);
            float segLen = _cumLengths[index + 1] - _cumLengths[index];
            float frac = (segLen > 1e-6f) ? (targetLength - _cumLengths[index]) / segLen : 0f;
            float t = Mathf.Lerp(_tSamples[index], _tSamples[index + 1], frac);
            return Mathf.Clamp01(t);
        }

        /// <summary>
        /// Returns the normalized t after moving <distance> world units from startT along the spline.
        /// Handles loop/pingpong/once behavior and updates direction for PingPong.
        /// </summary>
        private float GetTAtDistanceFrom(float startT, float distance)
        {
            if (_cumLengths == null || _cumLengths.Length == 0 || _totalLength <= 1e-6f) return startT;

            // current length
            float curLen = LengthAtT(Mathf.Clamp01(startT));

            float targetLen = curLen + distance;

            // For Loop mode we allow wrap; for PingPong we reflect and flip direction inside TAtLength; for Once we clamp
            float newT = TAtLength(targetLen);

            return newT;
        }

        #endregion
    }



}
