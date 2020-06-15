using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Testserver
{
    class Raspberry
    {
        public GpioController gpio;


        //############################################
        //*****************CONSTRUCTOR****************
        //############################################
        public Raspberry()
        {
            //GPIO-pinnen initialiseren zodat deze gebruikt kunnen worden
            gpio = GpioController.GetDefault();
        }
    }
}
