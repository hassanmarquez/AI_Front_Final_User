using System.ComponentModel;

namespace AutoSense.ViewModels;

internal partial class BluetoothDeviceModel : INotifyPropertyChanged
{
    private string deviceId;
    private string deviceName;
    private string deviceMAC;

    public string DeviceId 
    { 
        get => deviceId;
        set 
        { 
            deviceId = value; 
            NotifyPropertyChanged(nameof(DeviceId));
        }
    }
    public string DeviceName 
    { 
        get => deviceName;
        set
        {
            deviceName = value;
            NotifyPropertyChanged(nameof(DeviceName));
        } 
    }
    public string DeviceMAC 
    { 
        get => deviceMAC;
        set
        {
            deviceMAC = value;
            NotifyPropertyChanged(nameof(DeviceMAC));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
