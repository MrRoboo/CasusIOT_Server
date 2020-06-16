/************************************************
 * Author: Martijn                              *
 * Date: 06-2020                                *
 *                                              *
 * Server Testserver project                    *
 * WifiCommunication between Raspberries        *
 * Based on project from Rianne Boumans         *
 ************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Testserver
{
    public sealed partial class MainPage : Page
    {
        private SocketServer server;

        Raspberry raspberry = new Raspberry();
        SocketClient client = new SocketClient();


        private Button _button;
        private bool _isObjectTouched = false;
        private string _objectTouchmessage;


        //############################################
        //********************MAIN********************
        //############################################

        public MainPage()
        {
            this.InitializeComponent();

            //Creer een nieuwe server die luistert naar poort 9000
            //Poortnummer mag anders zijn, maar de clients moeten naar hetzelfde nummer luisteren
            server = new SocketServer(9000);

            //Koppel OnDataOntvangen aan de methode die uitgevoerd worden:
            server.OnDataOntvangen += server.Server_OnDataOntvangen;

            //initialiseren van hardware
            InitButtons();

            //Logica die de flow van de app bepaald
            Aansturen();
        }


        //############################################
        //*******************METHODS******************
        //############################################

        //****************PROGRAM_FLOW****************

        private void Aansturen()
        {
            while (true)
            {
                //Information will be sent to leds if data is received
                if (server.GetDataReceived())
                {
                    //Set bool false to check if new data is received
                    server.SetDataReceived(false);

                    //Get received data -> SocketServer.cs
                    String dataString = server.GetData();
                    Debug.WriteLine(dataString);

                    //Check the values of the sensors and send to host
                    //_objectTouchmessage = "test communication message";
                    //Debug.WriteLine(_objectTouchmessage);

                    //De delay voorkomt de spam van berichten in de console tijdens het testen. 
                    //omdat de data van client naar server en visa versa wordt verstuurd.
                    Task.Delay(500).Wait();
                    server.VerstuurBericht("bericht vanuit de server");

                }
            }
        }

        //*****************INITIALIZE*****************

        private void InitButtons()
        {
            //GPIO-pin 5 voor button
            _button = new Button(5);
            _button.buttonID.ValueChanged += ButtonID_ValueChanged;
        }

        //############################################
        //*******************EVENTS*******************
        //############################################

        //check what button pressed and return correct sensor data
        private void ButtonID_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                Trigger sensor = new Trigger("force sensor");
                _isObjectTouched = sensor.GetSensorValue();

                Debug.WriteLine("{0} button pressed. {1}", sensor.GetSensorName(), sensor.SensorActivation());
            }
        }
    }
}
