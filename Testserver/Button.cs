using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Testserver
{
    class Button
    {
        public GpioPin buttonID;


        //############################################
        //*****************CONSTRUCTOR****************
        //############################################

        public Button(int buttonID)
        {
            var gpio = GpioController.GetDefault();
            this.buttonID = gpio.OpenPin(buttonID);
            this.buttonID.SetDriveMode(GpioPinDriveMode.InputPullUp);
            this.buttonID.DebounceTimeout = TimeSpan.FromMilliseconds(30);
        }
    }
}
