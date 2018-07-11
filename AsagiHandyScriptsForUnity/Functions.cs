using Vuforia;

namespace AsagiHandyScriptsForUnity
{
    public static class Functions
    {
        public static bool IsGroundPlaneSupported()
        {
            return TrackerManager.Instance.GetTracker<PositionalDeviceTracker>() != null;
        }
    }
}
