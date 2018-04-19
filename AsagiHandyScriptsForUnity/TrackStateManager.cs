using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackStateManager : MonoBehaviour, ITrackableEventHandler
{
    public bool Tracked { get; private set; }

    protected TrackableBehaviour mTrackableBehaviour;

    // Use this for initialization
    void Start()
    {
        Tracked = false;

        mTrackableBehaviour = transform.parent.gameObject.GetComponent<TrackableBehaviour>();
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
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Tracked = false;
        }
        else
        {
            Tracked = false;
        }
    }
}
