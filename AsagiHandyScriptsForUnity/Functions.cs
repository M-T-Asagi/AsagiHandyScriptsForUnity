using Vuforia;

namespace AsagiHandyScripts
{
    public static class Functions
    {
        public static bool IsGroundPlaneSupported()
        {
            return TrackerManager.Instance.GetTracker<PositionalDeviceTracker>() != null;
        }
    }
}
