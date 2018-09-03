using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace AsagiVuforiaScripts
{
    public class Multi2OneMarker : MonoBehaviour
    {
        [SerializeField]
        TrackStateManager topLeft;
        [SerializeField]
        TrackStateManager topRight;
        [SerializeField]
        TrackStateManager bottomLeft;
        [SerializeField]
        TrackStateManager bottomRight;

        [SerializeField]
        float length;

        [SerializeField]
        bool updateOnTrackedOnly = false;

        [SerializeField]
        bool dontUpdate = false;

        public enum PosOfMarker
        {
            topLeft = 0,
            topRight,
            bottomLeft,
            bottomRight,

            itemNum
        }

        public Dictionary<PosOfMarker, Vector3> points { get; private set; }
        public Vector3 center { get; private set; }
        public Quaternion rotation { get; private set; }
        public Vector3? forward
        {
            get
            {
                Vector3? _forward = null;
                if (points.ContainsKey(PosOfMarker.topLeft) && points.ContainsKey(PosOfMarker.bottomLeft))
                {
                    _forward = points[PosOfMarker.topLeft] - points[PosOfMarker.bottomLeft];
                }

                if (points.ContainsKey(PosOfMarker.topRight) && points.ContainsKey(PosOfMarker.bottomRight))
                {
                    _forward = (_forward.HasValue) ? (_forward.Value + (points[PosOfMarker.topRight] - points[PosOfMarker.bottomRight])) / 2f : points[PosOfMarker.topRight] - points[PosOfMarker.bottomRight];
                }

                return _forward;
            }
        }

        public Vector3? upward
        {
            get
            {
                Vector3? _upward = null;
                if (points.ContainsKey(PosOfMarker.topLeft) && points.ContainsKey(PosOfMarker.bottomLeft))
                {
                    _upward = (topLeft.ImageTarget.transform.up + bottomLeft.ImageTarget.transform.up) / 2f;
                }

                if (points.ContainsKey(PosOfMarker.topRight) && points.ContainsKey(PosOfMarker.bottomRight))
                {
                    _upward = (_upward.HasValue) ? (_upward + (bottomLeft.ImageTarget.transform.up + bottomRight.ImageTarget.transform.up) / 2f) / 2f : (bottomLeft.ImageTarget.transform.up + bottomRight.ImageTarget.transform.up) / 2f;
                }

                return _upward;
            }
        }

        public bool markerDetected { get; private set; }
        public int detectedMarkerNum { get { return points.Count; } }

        public bool DontUpdate
        {
            get { return dontUpdate; }
            set { dontUpdate = value; }
        }

        private void Start()
        {
            points = new Dictionary<PosOfMarker, Vector3>();
            center = Vector3.zero;
            rotation = new Quaternion();
            markerDetected = false;


            if (updateOnTrackedOnly)
            {
                topLeft.TrackedEvent += MarkerDetectedTopLeft;
                topRight.TrackedEvent += MarkerDetectedTopRight;
                bottomLeft.TrackedEvent += MarkerDetectedBottomLeft;
                bottomRight.TrackedEvent += MarkerDetectedBottomRight;
            }
        }

        private void Update()
        {
            if (!dontUpdate && !updateOnTrackedOnly)
            {
                if (topLeft.Tracked)
                    MarkerDetected(PosOfMarker.topLeft, topLeft.ImageTarget.transform.position);
                else
                    MarkerUnDetected(PosOfMarker.topLeft);
                if (topRight.Tracked)
                    MarkerDetected(PosOfMarker.topRight, topRight.ImageTarget.transform.position);
                else
                    MarkerUnDetected(PosOfMarker.topRight);
                if (bottomLeft.Tracked)
                    MarkerDetected(PosOfMarker.bottomLeft, bottomLeft.ImageTarget.transform.position);
                else
                    MarkerUnDetected(PosOfMarker.bottomLeft);
                if (bottomRight.Tracked)
                    MarkerDetected(PosOfMarker.bottomRight, bottomRight.ImageTarget.transform.position);
                else
                    MarkerUnDetected(PosOfMarker.bottomRight);
            }

        }

        void MarkerDetected(PosOfMarker _posOfMarker, Vector3 _position)
        {
            points[_posOfMarker] = _position;
            UpdateCenter();
            UpdateRotation();
        }

        void MarkerUnDetected(PosOfMarker _posOfMarker)
        {
            points.Remove(_posOfMarker);
        }

        void MarkerDetectedTopLeft(object sender, EventArgs args)
        {
            MarkerDetected(PosOfMarker.topLeft, ((TrackStateManager)sender).ImageTarget.transform.position);
        }

        void MarkerDetectedTopRight(object sender, EventArgs args)
        {
            MarkerDetected(PosOfMarker.topRight, ((TrackStateManager)sender).ImageTarget.transform.position);
        }

        void MarkerDetectedBottomLeft(object sender, EventArgs args)
        {
            MarkerDetected(PosOfMarker.bottomLeft, ((TrackStateManager)sender).ImageTarget.transform.position);
        }

        void MarkerDetectedBottomRight(object sender, EventArgs args)
        {
            MarkerDetected(PosOfMarker.bottomRight, ((TrackStateManager)sender).ImageTarget.transform.position);
        }

        void UpdateCenter()
        {
            center = Vector3.zero;
            Vector3[] buff = Enumerable.Range(0, (int)PosOfMarker.itemNum).Where(i => points.ContainsKey((PosOfMarker)i)).Select(i =>
            {
                float halfOfLenght = length / 2f;
                switch (i)
                {
                    case (int)PosOfMarker.topLeft:
                        return topLeft.ImageTarget.transform.position + (topLeft.ImageTarget.transform.rotation * new Vector3(halfOfLenght, 0, -halfOfLenght));
                    case (int)PosOfMarker.topRight:
                        return topRight.ImageTarget.transform.position + (topRight.ImageTarget.transform.rotation * new Vector3(-halfOfLenght, 0, -halfOfLenght));
                    case (int)PosOfMarker.bottomLeft:
                        return bottomLeft.ImageTarget.transform.position + (bottomLeft.ImageTarget.transform.rotation * new Vector3(halfOfLenght, 0, halfOfLenght));
                    case (int)PosOfMarker.bottomRight:
                        return bottomRight.ImageTarget.transform.position + (bottomRight.ImageTarget.transform.rotation * new Vector3(-halfOfLenght, 0, halfOfLenght));
                }
                return Vector3.zero;
            }).ToArray();
            center = new Vector3(buff.Average(b => b.x), buff.Average(b => b.y), buff.Average(b => b.z));
        }

        void UpdateRotation()
        {
            if (forward.HasValue && upward.HasValue)
            {
                rotation = Quaternion.LookRotation(forward.Value, upward.Value);
            }
            else
            {
                markerDetected = false;
            }
        }
    }
}
