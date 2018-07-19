using Vuforia;
using UnityEngine;

namespace AsagiHandyScripts
{
    public static class Functions
    {
        public static bool IsGroundPlaneSupported()
        {
            return TrackerManager.Instance.GetTracker<PositionalDeviceTracker>() != null;
        }

        public static RaycastHit? RaycastTouchPosition(Vector3 touchPos, LayerMask target)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo, 100, target))
            {
                return hitInfo;
            }
            return null;
        }
    }
}
