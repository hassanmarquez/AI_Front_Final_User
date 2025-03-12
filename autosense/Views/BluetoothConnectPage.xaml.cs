using AutoSense.Services;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.BLE;
using System.Collections.ObjectModel;
using System.Text;

namespace AutoSense.Pages;

public partial class BluetoothConnectPage : ContentPage, INotifyPropertyChanged
{
    private IBluetoothLE ble;
    private IAdapter adapter;
    private IDevice selectedDevice;
    private ObservableCollection<IDevice> deviceList;

    public ObservableCollection<IDevice> DeviceList
    {
        get => deviceList;
        set
        {
            deviceList = value;
            OnPropertyChanged();
        }
    }

    public IDevice SelectedDevice
    {
        get => selectedDevice;
        set
        {
            selectedDevice = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAndRequestBluetoothPermissions();
    }
    public BluetoothConnectPage()
	{
        InitializeComponent();

        BindingContext = this;

        ble = CrossBluetoothLE.Current;
        adapter = CrossBluetoothLE.Current.Adapter;
        DeviceList = new ObservableCollection<IDevice>();

        /*
        string devicesNames = "";
        var bluetooth = new BluetoothService();

        var list = bluetooth.GetDevices();
        foreach (var item in list.Result)
        {
            devicesNames += item.Name + " - ";
        }
        */
        //LabelNames.Text = devicesNames;

    }

    private async Task<bool> ValidateBluetoohPermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Bluetooth>();
        }

        if (status == PermissionStatus.Granted)
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
            await DisplayAlert("Permisos", "Se requieren permisos de Bluetooth y ubicaci�n para que la aplicaci�n funcione correctamente.", "OK");
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
        adapter.DeviceDiscovered += (sender, args) =>
        {
            List<IDevice> IsDeviceExisting = (from devices in DeviceList
                                                where devices.Id == args.Device.Id
                                                select devices).ToList();
            if(!string.IsNullOrWhiteSpace(args.Device.Name) && !IsDeviceExisting.Any())
            {
                DeviceList.Add(args.Device);
            }
        };
        await adapter.StartScanningForDevicesAsync();
    }

    private async void OnConnectButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (SelectedDevice == null)
            {
                await DisplayAlert("Error", "Por favor, selecciona un dispositivo", "OK");
                return;
            }

            if(adapter.IsScanning)
            {
                await adapter.StopScanningForDevicesAsync();
            }

            //using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            //var param = new ConnectParameters(true, true);
            //await adapter.ConnectToKnownDeviceAsync(SelectedDevice.Id, param, cts.Token);

            //var cts1 = new CancellationTokenSource();
            //var param1 = new ConnectParameters(true, true);
            //await adapter.ConnectToDeviceAsync(SelectedDevice, param1, cts1.Token);
            await adapter.ConnectToDeviceAsync(SelectedDevice);
            
            var services = await SelectedDevice.GetServicesAsync();
            string message = "";
            foreach (var service in services)
            {                
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var characteristic in characteristics)
                {
                    string characteristicString = "characteristic ";
                    if(characteristic.CanRead)
                    {
                        characteristicString += characteristic.Name + "-" + characteristic.StringValue + "-";
                        (byte[] data, int resultCode) = await characteristic.ReadAsync();
                        characteristicString += (data is not null) ? Encoding.UTF8.GetString(data) + "\n" : "";
                        System.Diagnostics.Debug.WriteLine($"Data: {resultCode} - {characteristicString}");
                    }
                    /*
                    if (characteristic.CanUpdate)
                    {
                        characteristic.ValueUpdated += (s, a) =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                string message = BitConverter.ToString(a.Characteristic.Value);
                                DisplayAlert("Error", message, "OK");
                                ReceivedDataBluetoothLabel.Text = message;
                            });
                            string message = BitConverter.ToString(a.Characteristic.Value);
                            ReceivedDataBluetoothLabel.Text = message;
                        };
                        await characteristic.StartUpdatesAsync();
                    }
                    */
                    message += service.Name + "\n" + characteristicString + "\n";
                }
            }

            DisplayAlert("Info", message, "OK");
            ReceivedDataBluetoothLabel.Text = message;

        }
        catch (Exception ex)
        {
            var Error = ex.Message;
            var stact = ex.StackTrace;
            DisplayAlert("Error", ex.Message, "OK");
            DisplayAlert("Error", ex.StackTrace, "OK");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}