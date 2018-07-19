using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace AsagiHandyScripts
{
    public class ARObjectManager : MonoBehaviour
    {
        [SerializeField]
        GameObject planeFinder;

        [SerializeField]
        GameObject groundAnchor;

        [SerializeField]
        GameObject imageAnhor;

        [SerializeField]
        bool markerBased = false;

        [SerializeField]
        TouchClickManager touchClickManager;

        [SerializeField]
        TrackStateManager trackStateManager;

        [SerializeField]
        LayerMask touchRayTargetLayers;

        List<GameObject> arObjects;

        Transform anchor;
        Transform _anchor;

        bool useGroundPlane = false;

        public class GroundPlaneHitEventArgs : EventArgs
        {
            public Vector3 position;
            public bool isGroundPlane;
            public GroundPlaneHitEventArgs(Vector3 _p, bool _g)
            {
                position = _p;
                isGroundPlane = _g;
            }
        }

        public event EventHandler<GroundPlaneHitEventArgs> OnGroundPlaneHit;

        // Use this for initialization
        void Start()
        {
            useGroundPlane = (Functions.IsGroundPlaneSupported() && planeFinder != null && groundAnchor != null);

            if (!useGroundPlane || markerBased)
                markerSettings();
            else
                groundPlaneSettings();

            arObjects = new List<GameObject>();
        }

        void Update()
        {
            if (useGroundPlane && markerBased && trackStateManager.Tracked)
            {
                _anchor.position = anchor.position;
                _anchor.rotation = anchor.rotation;
            }
        }

        void groundPlaneSettings()
        {
            if (!markerBased)
            {
                if (imageAnhor != null)
                    imageAnhor.SetActive(false);

                anchor = groundAnchor.transform;
            }
            else
            {
                _anchor = groundAnchor.transform;
            }

            planeFinder.GetComponent<PlaneFinderBehaviour>().OnInteractiveHitTest.AddListener(this.GroundPlaneHit);
        }

        void markerSettings()
        {
            if (planeFinder != null)
                planeFinder.SetActive(false);
            if (groundAnchor != null)
                groundAnchor.SetActive(false);

            touchClickManager.Tap += this.OnTap;
            anchor = imageAnhor.transform;
        }

        void _GroundPlaneHit(Vector3 pos, bool isGroundPlane = false)
        {
            if (OnGroundPlaneHit != null)
                OnGroundPlaneHit(this, new GroundPlaneHitEventArgs(pos, isGroundPlane));
        }

        public GameObject Arrange(GameObject _gameObject, Vector3 position, Quaternion rotation)
        {
            GameObject arranged = Instantiate(_gameObject, anchor, false);
            arObjects.Add(arranged);
            arObjects[arObjects.Count - 1].transform.position = position;
            arObjects[arObjects.Count - 1].transform.rotation = rotation;
            return arranged;
        }

        public void GroundPlaneHit(HitTestResult result)
        {
            _GroundPlaneHit(result.Position, true);
        }

        public void OnTap(object sender, TouchClickManager.TapEventArgs args)
        {
            if (trackStateManager.Tracked)
            {
                Ray ray = Camera.main.ScreenPointToRay(args.position);
                RaycastHit? hitInfo = Functions.RaycastTouchPosition(args.position, touchRayTargetLayers);
                if (hitInfo.HasValue)
                    _GroundPlaneHit(hitInfo.Value.point, false);
            }
        }
    }
}
