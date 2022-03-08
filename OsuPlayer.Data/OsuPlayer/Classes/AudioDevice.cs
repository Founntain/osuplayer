using ManagedBass;

namespace OsuPlayer.Data.OsuPlayer.Classes
{
    /// <summary>
    /// Wrapper class for <see cref="ManagedBass.DeviceInfo"/>
    /// </summary>
    public sealed class AudioDevice
    {
        private DeviceInfo DeviceInfo { get; set; }
        public string DeviceName => DeviceInfo.Name;
        public bool IsEnabled => DeviceInfo.IsEnabled;
        public bool IsDefault => DeviceInfo.IsDefault;
        public bool IsInitialized => DeviceInfo.IsInitialized;
        public string Driver => DeviceInfo.Driver;
        public string DeviceToString => DeviceInfo.ToString();

        public AudioDevice(DeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public override string ToString() => DeviceName;
    }
}