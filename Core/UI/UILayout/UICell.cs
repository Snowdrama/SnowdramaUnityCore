using UnityEngine;

namespace Snowdrama.UI
{
    [System.Serializable]
    public class UICell
    {
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _size;
        [SerializeField] private Vector2 _pivot;
        [SerializeField] private Vector2 _min;
        [SerializeField] private Vector2 _max;
        public Vector2 position
        {
            get { return new Vector2(Mathf.Lerp(min.x, max.x, pivot.x), Mathf.Lerp(min.y, max.y, pivot.y)); }
        }
        public Vector2 size
        {
            get { return new Vector2((max.x - min.x), (max.y - min.y)); }
        }
        public Vector2 pivot
        {
            get { return _pivot; }
        }
        public Vector2 min
        {
            get { return _min; }
        }
        public Vector2 max
        {
            get { return _max; }
        }
        public Bounds bounds
        {
            get
            {
                return new Bounds(position, size);
            }
        }

        [SerializeField]private Vector2 _positionAnchor;
        [SerializeField]private Vector2 _sizeAnchor;
        [SerializeField]private Vector2 _minAnchor;
        [SerializeField]private Vector2 _maxAnchor;
        public Vector2 positionAnchor
        {
            get { return new Vector2(Mathf.Lerp(minAnchor.x, maxAnchor.x, pivot.x), Mathf.Lerp(minAnchor.y, maxAnchor.y, pivot.y)); }
        }
        public Vector2 sizeAnchor
        {
            get { return new Vector2((maxAnchor.x - minAnchor.x), (maxAnchor.y - minAnchor.y)); }
        }
        public Vector2 minAnchor
        {
            get { return _minAnchor; }
        }
        public Vector2 maxAnchor
        {
            get { return _maxAnchor; }
        }

        public Bounds boundsAnchor
        {
            get
            {
                return new Bounds(positionAnchor, sizeAnchor);
            }
            set
            {
                _minAnchor = value.min;
                _maxAnchor = value.max;
            }
        }

        public Vector2 dumb;

        public UICell(Vector2 minAnchor, Vector2 maxAnchor, Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
            _minAnchor = minAnchor;
            _maxAnchor = maxAnchor;
        }
    }
}