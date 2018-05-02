// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;

using BuildAzure.IoT.Adafruit.BME280;

using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace SmartDevice
{
    public sealed partial class MainPage : Page
    {
        // 16 is the pin number that the proximity interrupt is connected to on the Raspberry Pi side
        private const int intPinNumber = 16;

        // interrupt pin from the proximity sensor
        private GpioPin interruptPin;

        private DispatcherTimer measureTimer;
       
        // temperature sensor
        private BME280Sensor bme280Sensor = new BME280Sensor();

        // proximity sensor
        private VNCL4010Sensor vncl4010Sensor = new VNCL4010Sensor();

        public MainPage()
        {  
            InitializeComponent();

            // set up timer to sample proximity and temperature values every 1000 milliseconds
            measureTimer = new DispatcherTimer();
            measureTimer.Interval = TimeSpan.FromMilliseconds(1000);
            // attach event handler on each 1000ms tick
            measureTimer.Tick += MeasureTimerTick;

            // initialize GPIO devices
            InitializeDevices();
        }

        #region Handlers
        private async void MeasureTimerTick(object sender, object e)
        {
            // read Temperature
            double temperature = await bme280Sensor.ReadTemperature();
            // convert to Fahrenheit
            double fahrenheitTemperature = temperature * 1.8 + 32.0;

            // read Proximity
            int proximity = vncl4010Sensor.ReadProximity();

            TemperatureStatus.Text = "The temperature is currently " + fahrenheitTemperature.ToString("n1") + "°F";

            await IoTHub.SendDeviceToCloudMessage(fahrenheitTemperature);
        }

        private async void OnInterrupt(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // only act on initial pull-down event from the interrupt; avoid rising edge trigger upon reset
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                // try to recognize viewer and greet them by name
                await RecognizeAndGreetViewer();
            }
        }
        #endregion

        #region Methods
        private async void InitializeDevices()
        {
            var gpio = GpioController.GetDefault();

            // show an error if there is no GPIO controller
            if (gpio is null)
            {
                interruptPin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            // set up interrupt pin for proximity sensor
            interruptPin = gpio.OpenPin(intPinNumber);

            // pull up interrupt pin as sensor will pull down to notify
            interruptPin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            // initialize BME280 Sensor + Intitialize VNCL4010
            await Task.WhenAll(bme280Sensor.Initialize(), vncl4010Sensor.Initialize());

            // listen to interrupt pin changes
            interruptPin.ValueChanged += OnInterrupt;

            GpioStatus.Text = "Connecting to IoT Hub...";

            await IoTHub.ConnectToIoTHub();

            GpioStatus.Text = "";

            // start measuring temperature and proximity 
            measureTimer.Start();
        }

        private async Task RecognizeAndGreetViewer()
        {
            // let viewer know we're recognizing them
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InterruptText.Text = "Recognizing...");

            // take their photo
            StorageFile photoFile = await Webcam.TakePhoto();

            // use Cognitive Services to identify them
            string name = await FaceAPI.GetViewerName(photoFile);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // clear waiting text
                InterruptText.Text = "";
                // greet viewer on screen
                GpioStatus.Text = "Greetings, " + name;
            });

            // clear the interrupt flag so that interrupt can occur again
            vncl4010Sensor.ClearInterruptFlag();

            // delete photo taken of viewer
            await photoFile.DeleteAsync();
        }
        #endregion
    }
}
