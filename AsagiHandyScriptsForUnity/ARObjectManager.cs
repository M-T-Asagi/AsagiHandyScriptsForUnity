using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using AsagiHandyScripts;

namespace AsagiHandyScriptsForUnity
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
            useGroundPlane = Functions.IsGroundPlaneSupported();
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
            planeFinder.SetActive(false);
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
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo, 100, touchRayTargetLayers))
                {
                    _GroundPlaneHit(hitInfo.point, false);
                }
            }
        }
    }

}
