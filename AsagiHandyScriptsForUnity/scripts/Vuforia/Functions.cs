using Vuforia;

namespace AsagiVuforiaScripts
{
    public static class Functions
    {
        public static bool IsGroundPlaneSupported()
        {
            return TrackerManager.Instance.GetTracker<PositionalDeviceTracker>() != null;
        }
    }
}
