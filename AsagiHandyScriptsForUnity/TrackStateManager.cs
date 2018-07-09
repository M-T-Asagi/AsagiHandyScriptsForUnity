using System;
using UnityEngine;
using Vuforia;

namespace AsagiHandyScripts
{
    public class TrackStateManager : MonoBehaviour, ITrackableEventHandler
    {
        [SerializeField]
        GameObject imageTarget;

        public event EventHandler<EventArgs> TrackedEvent;
        public event EventHandler<EventArgs> UnTrackedEvent;

        public bool Tracked { get; private set; }

        protected TrackableBehaviour mTrackableBehaviour;

        // Use this for initialization
        void Start()
        {
            Tracked = false;

            mTrackableBehaviour = imageTarget.GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        public void OnTrackableStateChanged(
           TrackableBehaviour.Status previousStatus,
           TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                Tracked = true;

                if (TrackedEvent != null)
                    TrackedEvent(this, new EventArgs());
            }
            else
            {
                Tracked = false;

                if (UnTrackedEvent != null)
                    UnTrackedEvent(this, new EventArgs());
            }
        }
    }
}