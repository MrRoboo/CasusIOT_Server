using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Windows.UI.Xaml;

namespace Testserver
{
    class ForceSensor

    {
        private const string SPI_CONTROLLER_NAME = "SPI0";
        private DispatcherTimer timer;

        /*mcp3002 10 bit output*/
        private byte[] readData = new byte[2] { 0x00, 0x00 };
        private byte[] writeData = new byte[2] { 0x68, 0x00 };
        private List<int> forceSensorData = new List<int>();

        private int resultData;
        private DateTime? timePressed;

        public GameController gameController;

        public SpiDevice spiPin;
        public GpioPin gpioPin;

        public ForceSensor(int SPI_ID, int GPIO_ID)
        {
            StartTimer();
            InitGPIO(GPIO_ID);
            InitSPI(SPI_ID);
        }

        private void InitGPIO(int GPIO_ID)
        {
            var gpio = GpioController.GetDefault();
            gpioPin = gpio.OpenPin(GPIO_ID);
        }


        //ChipSelectId: 0 or 1
        //NOTE - THIS IS AN ASYNC METHOD, IF YOU DON'T AWAIT IT WHEN CALLING THEN IT MAY NOT BE COMPELTE WHEN YOU FIRST TRY TO USE THE OTHER IO METHODS)
        public async void InitSPI(int ChipSelectId)
        {
            try
            {
                // Create SPI initialization settings
                var settings = new SpiConnectionSettings(ChipSelectId);         //CS Select line
                settings.ClockFrequency = 500000;                    //<<<<<SET BUS SPEED (10000000 = 10MHz - 100nS per bit).  10000 = Is slowest possible ClockFrequency
                settings.Mode = SpiMode.Mode0;

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
                spiPin = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SPI Initialisation Error", e.Message);
            }

            return;
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            HandleData();
        }

        private void HandleData()
        {

            spiPin.TransferFullDuplex(writeData, readData);
            resultData = ConvertToInt(readData);

            if (resultData > 25)
            {
                if (timePressed == null)
                {
                    timePressed = new DateTime();
                }
                Debug.WriteLine(resultData);
                CalculateForce(resultData);
            }
            else if (resultData < 25 && GetValue() == GpioPinValue.Low)
            {
                if (forceSensorData.Count > 0)
                {
                    SendData();
                }

            }
        }

        private void SendData()
        {
            var highestValue = forceSensorData.Max();
            gameController.AddForceData(highestValue);
            ClearData();

        }

        private void ClearData()
        {
            timePressed = null;
            forceSensorData.Clear();
        }

        private void CalculateForce(int resultData)
        {
            forceSensorData.Add(resultData);
        }

        public int ConvertToInt(byte[] data)
        {
            int result;
            result = data[0] & 0x03;
            result <<= 8;
            result += data[1];
            return result;
        }

        public GpioPinValue GetValue()
        {
            return gpioPin.Read();
        }
    }
}
