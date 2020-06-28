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
        private GameController gameController;

        //############################################
        //*****************CONSTRUCTOR****************
        //############################################

        //Initialize button
        public Button(int buttonID, GameController gameController)
        {
            var gpio = GpioController.GetDefault();
            this.buttonID = gpio.OpenPin(buttonID);
            //this.buttonID.SetDriveMode(GpioPinDriveMode.Output);
            this.buttonID.SetDriveMode(GpioPinDriveMode.InputPullUp);
            this.buttonID.DebounceTimeout = TimeSpan.FromMilliseconds(30);
            this.buttonID.ValueChanged += ButtonHandler;
            this.gameController = gameController;
        }


        private void ButtonHandler(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                if (gameController.gameState)
                {
                    gameController.gameState = false;
                }
                else
                {
                    gameController.gameState = true;
                }

                //} else
                //{
                //    gameController.gameState = false;
                //}
            }

            //private void ButtonID_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
            //{
            //if (args.Edge == GpioPinEdge.FallingEdge)
            //{
            //        Trigger sensor = new Trigger("force sensor");
            //        _isObjectTouched = sensor.GetSensorValue();

            //        Debug.WriteLine("{0} button pressed. {1}", sensor.GetSensorName(), sensor.SensorActivation());
            //    }
            //}



        }


    }
}
