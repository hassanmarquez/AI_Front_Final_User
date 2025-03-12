using AutoSense.Services.Services.Bluetooth;
using System.Text;

namespace AutoSense.Services.Services.Diagnostic;

public class MessageHandler
{
    private readonly BluetoothService _bluetoothService;

    public MessageHandler()
    {
        _bluetoothService = new BluetoothService();
    }

    public async Task ConnectToOBD2DeviceAsync()
    {
        await _bluetoothService.GetDevices();
        await _bluetoothService.ConnectDeviceAsync();
    }

    public async Task<string> RequestVINAsync()
    {
        // Comando OBD-II para solicitar el VIN (09 02)
        byte[] vinRequestCommand = Encoding.ASCII.GetBytes("0902");
        await _bluetoothService.SendMessageAsync(vinRequestCommand);

        // Recibir la respuesta
        string response = await _bluetoothService.ReceiveMessageAsync();
        return DecodeVIN(response);
    }

    public async Task<List<string>> RequestErrorCodesAsync()
    {
        // Comando OBD-II para solicitar los códigos de error (03)
        byte[] errorCodesRequestCommand = Encoding.ASCII.GetBytes("03");
        await _bluetoothService.SendMessageAsync(errorCodesRequestCommand);

        // Recibir la respuesta
        string response = await _bluetoothService.ReceiveMessageAsync();
        return DecodeErrorCodes(response);
    }

    private string DecodeVIN(string response)
    {
        // Asumimos que la respuesta contiene el VIN directamente
        string vin = response.Trim();

        if (vin.Length != 17)
        {
            return "Invalid VIN length";
        }

        // Ejemplo de decodificación del VIN
        string wmi = vin.Substring(0, 3); // World Manufacturer Identifier
        string vds = vin.Substring(3, 6); // Vehicle Descriptor Section
        string vis = vin.Substring(9, 8); // Vehicle Identifier Section

        string manufacturer = GetManufacturer(wmi);
        string model = GetModel(vds);

        return $"VIN: {vin}, Manufacturer: {manufacturer}, Model: {model}";
    }

    private string GetManufacturer(string wmi)
    {
        // Aquí puedes agregar una lógica más compleja o una base de datos para obtener el fabricante
        // Este es un ejemplo simple
        return wmi switch
        {
            "1HG" => "Honda",
            "1C4" => "Chrysler",
            "1FT" => "Ford",
            "1G1" => "Chevrolet",
            _ => "Unknown Manufacturer"
        };
    }

    private string GetModel(string vds)
    {
        // Aquí puedes agregar una lógica más compleja o una base de datos para obtener el modelo
        // Este es un ejemplo simple
        return vds switch
        {
            "CM826" => "Civic",
            "CM827" => "Accord",
            "CM828" => "CR-V",
            _ => "Unknown Model"
        };
    }

    private List<string> DecodeErrorCodes(string response)
    {
        // Implementar la lógica para decodificar los códigos de error de la respuesta
        // Este es un ejemplo simple que asume que la respuesta contiene los códigos de error separados por comas
        return response.Split(',').ToList();
    }

    static string DecodeISO15765_2Message(byte[] data)
    {
        if (data.Length < 2)
        {
            return "Invalid message: too short";
        }

        // ISO-TP header analysis
        int pciType = data[0] >> 4;

        switch (pciType)
        {
            case 0x0: // Single Frame
                int messageLength = data[0] & 0x0F;
                if (data.Length - 1 < messageLength)
                {
                    return "Invalid single frame: data length mismatch";
                }
                return $"Single Frame: {BitConverter.ToString(data.Skip(1).Take(messageLength).ToArray())}";

            case 0x1: // First Frame
                int totalLength = (data[0] & 0x0F) << 8 | data[1];
                return $"First Frame: Total Length: {totalLength}, Data: {BitConverter.ToString(data.Skip(2).ToArray())}";

            case 0x2: // Consecutive Frame
                int sequenceNumber = data[0] & 0x0F;
                return $"Consecutive Frame: Sequence Number: {sequenceNumber}, Data: {BitConverter.ToString(data.Skip(1).ToArray())}";

            case 0x3: // Flow Control Frame
                int flowStatus = data[0] & 0x0F;
                return $"Flow Control Frame: Flow Status: {flowStatus}";

            default:
                return "Unknown frame type";
        }
    }
}

