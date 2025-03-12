using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System.Text;

namespace AutoSense.Services.Services.Bluetooth;

public class BluetoothService
{
    private IBluetoothLE? _bleHandler;
    private IAdapter? _bleAdapter;
    private IList<IDevice> _deviceList;
    private IDevice? _selectedDevice;
    private ICharacteristic? _writeCharacteristic;
    private ICharacteristic? _readCharacteristic;
    private string _typeDevice = "OBD";

    public BluetoothService()
    {
        _bleHandler = CrossBluetoothLE.Current;
        _bleAdapter = CrossBluetoothLE.Current.Adapter;
        _deviceList = new List<IDevice>();
    }

    public async Task<IList<IDevice>> GetDevices()
    {
        _deviceList = _bleAdapter!.DiscoveredDevices.ToList();
        return _deviceList;
    }

    public async Task<DeviceState> ConnectDeviceAsync()
    {
        _selectedDevice = _deviceList.FirstOrDefault();
        if (_selectedDevice == null)
            throw new Exception("No device selected");

        await _bleAdapter!.ConnectToDeviceAsync(_selectedDevice);
        var services = await _selectedDevice.GetServicesAsync();
        foreach (var service in services)
        {
            var characteristics = await service.GetCharacteristicsAsync();
            foreach (var characteristic in characteristics)
            {
                if (characteristic.Properties.HasFlag(CharacteristicPropertyType.Write))
                {
                    _writeCharacteristic = characteristic;
                }
                if (characteristic.Properties.HasFlag(CharacteristicPropertyType.Read))
                {
                    _readCharacteristic = characteristic;
                }
            }
        }

        if (_writeCharacteristic == null || _readCharacteristic == null)
            throw new Exception("No suitable characteristics found");

        return _selectedDevice.State;
    }

    public async Task SendMessageAsync(byte[] message)
    {
        if (_writeCharacteristic == null)
            throw new Exception("No write characteristic available");

        await _writeCharacteristic.WriteAsync(message);
    }

    public async Task<string> ReceiveMessageAsync()
    {
        if (_readCharacteristic == null)
            throw new Exception("No read characteristic available");

        var bytes = await _readCharacteristic.ReadAsync();
        return Encoding.UTF8.GetString(bytes.data);
    }

    /*
           private async Task<bool> ValidateBluetoohPermissions()
           {
               var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
               if (status != PermissionStatus.Granted)
               {
                   status = await Permissions.RequestAsync<Permissions.Bluetooth>();
               }

               if(status == PermissionStatus.Granted)
                 return true;

               return false;
           }


           private async Task CheckAndRequestBluetoothPermissions()
           {
               var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
               if (status != PermissionStatus.Granted)
               {
                   status = await Permissions.RequestAsync<Permissions.Bluetooth>();
               }

               var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
               if (locationStatus != PermissionStatus.Granted)
               {
                   locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
               }

               if (status != PermissionStatus.Granted || locationStatus != PermissionStatus.Granted)
               {
                   //await DisplayAlert("Permisos", "Se requieren permisos de Bluetooth y ubicación para que la aplicación funcione correctamente.", "OK");
               }
           }

           private async void OnScanButtonClicked(object sender, EventArgs e)
           {
               if (!await ValidateBluetoohPermissions())
               {
                   await DisplayAlert("Error", "No se concedieron los permisos necesarios.", "OK");
                   return;
               }

               DeviceList.Clear();
               adapter.ScanMode = ScanMode.LowLatency;
               adapter.DeviceDiscovered += (s, a) =>
               {
                   DeviceList.Add(a.Device);
               };
               await adapter.StartScanningForDevicesAsync();
           }

           private async void OnConnectButtonClicked(object sender, EventArgs e)
           {
               if (SelectedDevice == null)
               {
                   await DisplayAlert("Error", "Por favor, selecciona un dispositivo", "OK");
                   return;
               }

               await adapter.ConnectToDeviceAsync(SelectedDevice);
               var services = await SelectedDevice.GetServicesAsync();
               foreach (var service in services)
               {
                   var characteristics = await service.GetCharacteristicsAsync();
                   foreach (var characteristic in characteristics)
                   {
                       if (characteristic.CanUpdate)
                       {
                           characteristic.ValueUpdated += (s, a) =>
                           {
                               MainThread.BeginInvokeOnMainThread(() =>
                               {
                                   ReceivedDataLabel.Text = BitConverter.ToString(a.Characteristic.Value);
                               });
                           };
                           await characteristic.StartUpdatesAsync();
                       }
                   }
               }
           }

           public event PropertyChangedEventHandler PropertyChanged;

           protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
           {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
           }


   */
}
