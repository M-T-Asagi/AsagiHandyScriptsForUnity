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

        public bool markerDetected { get; private set; }


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
                if (topRight.Tracked)
                    MarkerDetected(PosOfMarker.topRight, topRight.ImageTarget.transform.position);
                if (bottomLeft.Tracked)
                    MarkerDetected(PosOfMarker.bottomLeft, bottomLeft.ImageTarget.transform.position);
                if (bottomRight.Tracked)
                    MarkerDetected(PosOfMarker.bottomRight, bottomRight.ImageTarget.transform.position);
            }

        }

        void MarkerDetected(PosOfMarker _posOfMarker, Vector3 _position)
        {
            if (dontUpdate)
                return;

            points[_posOfMarker] = _position;
            UpdateCenter();
            UpdateRotation();
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
            Vector3 forward = Vector3.zero;
            Vector3 upward = Vector3.zero;
            markerDetected = true;

            if (points.ContainsKey(PosOfMarker.topLeft) && points.ContainsKey(PosOfMarker.bottomLeft))
            {
                forward = points[PosOfMarker.topLeft] - points[PosOfMarker.bottomLeft];
                upward = (topLeft.ImageTarget.transform.up + bottomLeft.ImageTarget.transform.up) / 2f;
            }
            else if (points.ContainsKey(PosOfMarker.topRight) && points.ContainsKey(PosOfMarker.bottomRight))
            {
                forward = points[PosOfMarker.topRight] - points[PosOfMarker.bottomRight];
                upward = (bottomLeft.ImageTarget.transform.up + bottomRight.ImageTarget.transform.up) / 2f;
            }
            else
            {
                markerDetected = false;
            }

            if (markerDetected)
            {
                rotation = Quaternion.LookRotation(forward, upward);
            }
        }
    }
}
