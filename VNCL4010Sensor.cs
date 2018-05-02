// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace SmartDevice
{
    class VNCL4010Sensor
    {
        private I2cDevice sensor = null;
        private const byte address = 0x13;

        public async Task Initialize()
        {
            // the 'friendly name' of the i2c controller
            const string I2CControllerName = "I2C1";

            // set the i2c address of the device
            I2cConnectionSettings settings = new I2cConnectionSettings(address);

            // this device supports fast i2c communication mode
            settings.BusSpeed = I2cBusSpeed.FastMode;

            // query device for the i2c controller we need
            string query = I2cDevice.GetDeviceSelector(I2CControllerName);
            DeviceInformationCollection deviceInfo = await DeviceInformation.FindAllAsync(query);

            // using the device id and the device address, get the final device interface for read/write communication
            sensor = await I2cDevice.FromIdAsync(deviceInfo[0].Id, settings);

            // Check if device was found and throw if not
            if (sensor is null)
            {
                throw new Exception("Proximity sensor device was not found");
            }

            // Get the product id of the i2c device
            var productId = GetProductId();

            // Check that the product id is the expected one and throw if not
            if ((productId & 0xF0) != 0x20)
            {
                throw new Exception("Proximity sensor product id was incorrect");
            }

            CalibrateProximity();
        }

        private byte GetProductId()
        {
            byte[] productIdCommand = { RegistersConstants.ProductId };
            byte[] productIdResponse = new byte[1];

            // write to the product id register to request the device's id, and then read response
            sensor.WriteRead(productIdCommand, productIdResponse);

            // the response byte is the product id
            return productIdResponse[0];
        }

        private void CalibrateProximity()
        {
            // set IR LED to power level of 20, 0x14 in hex
            byte[] setProximityIrLedCommand = { RegistersConstants.SetProximityIrLed, 0x14 };
            sensor.Write(new byte[] { RegistersConstants.SetProximityIrLed, 0x14 });

            // set sensor to perform 16.625 readings a second
            sensor.Write(new byte[] { RegistersConstants.SetProximityReadRate, 0x03 });

            // set interupt to happen when the proximity value falls below 3000
            // settings are 0x02 hex for having a proximity threshold only setting specifically or 00000010 in binary
            // see table 10 on page 9 of datasheet for more information: https://cdn-shop.adafruit.com/product-files/466/vcnl4010.pdf
            sensor.Write(new byte[] { RegistersConstants.SetInterruptThreshold, 0x02 });


            // write threshold value of 3000 to threshold registers
            // 3000 = 0x0bb8 in binary hex
            sensor.Write(new byte[] { RegistersConstants.SetInterruptHighByte, 0x0B });
            sensor.Write(new byte[] { RegistersConstants.SetInterruptLowByte, 0xB8 });
        }

        public int ReadProximity()
        {
            // this will hold our final reading
            int proximityReading = 0;

            // create bytes to store read responses from the device
            byte[] readyResponse = new byte[1];
            byte[] dataResponse0 = new byte[1];
            byte[] dataResponse1 = new byte[1];

            // prepare command sequences to send to the device
            byte[] proximityDataCommand = { RegistersConstants.MeasureProximityData };
            byte[] proximityMeasureCommand = { RegistersConstants.Command, RegistersConstants.MeasureProximity };

            // check if device is ready for us to read a proximity measurement
            sensor.WriteRead(proximityMeasureCommand, readyResponse);

            bool isReady = (readyResponse[0] & RegistersConstants.MeasureProximityReady) > 0;

            if (isReady)
            {
                // ask to read from the proximity data
                sensor.Write(proximityDataCommand);
                // read the high byte
                sensor.Read(dataResponse0);
                // read the low byte
                sensor.Read(dataResponse1);

                // store the proximity measurement results as a single int
                proximityReading = dataResponse0[0];
                proximityReading <<= 8;
                proximityReading |= dataResponse1[0];
            }

            return proximityReading;
        }

        public void ClearInterruptFlag()
        {
            // the interrupt won't fire again until we explicitly reset it on the device by setting the flag register
            sensor.Write(new byte[] { RegistersConstants.SetInterruptFlag, 0x01 });
        }
    }
}
