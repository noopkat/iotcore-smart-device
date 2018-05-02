# “Hello, Smart IoT Device!”

We’ll create a simple Smart IoT Device application for deployment to your Windows 10 IoT Core device.

This is a headed sample. To better understand what headed mode is and how to configure your device to be headed, follow the instructions [here](https://docs.microsoft.com/en-us/windows/iot-core/learn-about-hardware/headlessmode).

Also, be aware that the GPIO APIs are only available on Windows 10 IoT Core, so this sample cannot run on your desktop.

## Load the project in Visual Studio

* * *

## Connect the LED to your Windows IoT device

* * *

You’ll need a few components:

*   a BME280 breakout board

*   a VCNL4010 breakout board

*   a breadboard and some connector wires

*	a Microsoft Lifecam

*	an ethernet cable


### For Raspberry Pi 2 or 3 (RPi2 or RPi3)




## Deploy your app

* * *

1.  With the application open in Visual Studio, set the architecture in the toolbar dropdown. If you’re building for MinnowBoard Max, select `x86`. If you’re building for Raspberry Pi 2 or 3 or the DragonBoard, select `ARM`.

2.  Next, in the Visual Studio toolbar, click on the `Local Machine` dropdown and select `Remote Machine`

    ![RemoteMachine Target](https://az835927.vo.msecnd.net/sites/iot/Resources/images/AppDeployment/cs-remote-machine-debugging.png)

3.  At this point, Visual Studio will present the **Remote Connections** dialog. If you previously used [PowerShell](https://docs.microsoft.com/en-us/windows/iot-core/connect-your-device/powershell) to set a unique name for your device, you can enter it here (in this example, we’re using **my-device**). Otherwise, use the IP address of your Windows IoT Core device. After entering the device name/IP select `Universal` for Windows Authentication, then click **Select**.

    ![Remote Machine Debugging](https://az835927.vo.msecnd.net/sites/iot/Resources/images/AppDeployment/cs-remote-connections.PNG)

4.  You can verify or modify these values by navigating to the project properties (select **Properties** in the Solution Explorer) and choosing the `Debug` tab on the left:

    ![Project Properties Debug Tab](https://az835927.vo.msecnd.net/sites/iot/Resources/images/AppDeployment/cs-debug-project-properties.PNG)

When everything is set up, you should be able to press F5 from Visual Studio. If there are any missing packages that you did not install during setup, Visual Studio may prompt you to acquire those now. The Blinky app will deploy and start on the Windows IoT device.

Congratulations! You controlled some advanced devices on your Windows IoT device.

## Let’s look at the code

* * *


## Additional resources
* [Windows 10 IoT Core home page](https://developer.microsoft.com/en-us/windows/iot/)
* [Documentation for all samples](https://developer.microsoft.com/en-us/windows/iot/samples)

This project has adopted the Microsoft Open Source Code of Conduct. For more information see the Code of Conduct FAQ or contact <opencode@microsoft.com> with any additional questions or comments.
