using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;

using Newtonsoft.Json;

using Windows.Storage;

namespace SmartDevice
{
    public static class IoTHub
    {
        // IoT Hub Device client
        static DeviceClient deviceClient;

        public static async Task ConnectToIoTHub()
        {
            // Azure IoT Hub connection
            deviceClient = DeviceClient.CreateFromConnectionString(AzureCredentials.centralConnectionString, TransportType.Mqtt);
            await deviceClient.SetMethodHandlerAsync("UploadPhoto", OnUploadPhoto, null);
        }

        public static async Task SendDeviceToCloudMessage(double temperature)
        {
            // create new telemetry message
            var telemetryDataPoint = new
            {
                time = DateTime.Now.ToString(),
                deviceId = AzureCredentials.centralDeviceId,
                temperature = temperature
            };

            // serialise message to a JSON string
            string messageString = JsonConvert.SerializeObject(telemetryDataPoint);

            // format JSON string into IoT Hub message
            Message message = new Message(Encoding.ASCII.GetBytes(messageString));

            // push message to IoT Hub
            await deviceClient.SendEventAsync(message);
        }


        public static async Task<MethodResponse> OnUploadPhoto(MethodRequest methodRequest, object userContext)
        {

            // take a photo
            StorageFile photoFile = await Webcam.TakePhoto();

            using (FileStream photoStream = new FileStream(photoFile.Path, FileMode.Open))
            {
                // upload photo via IoT Hub
                await deviceClient.UploadToBlobAsync(photoFile.Name, photoStream);

            }
            // delete photo taken of viewer
            await photoFile.DeleteAsync();

            return new MethodResponse(200);
        }
    }
}
