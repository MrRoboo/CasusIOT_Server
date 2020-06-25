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
        private List<int> _forceSensorData = new List<int>();

        private int resultData;
        private DateTime? timePressed;

        public SpiDevice SpiPin;
        public GpioPin gpioPin;

        public ForceSensor(int SPI_ID, int GPIO_ID)
        {
            startTimer();
            initGPIO(GPIO_ID);
            Initialise(SPI_ID);
        }

        private void initGPIO(int GPIO_ID)
        {
            var gpio = GpioController.GetDefault();
            gpioPin = gpio.OpenPin(GPIO_ID);
            //gpioPin.ValueChanged += Value_Changed;
        }


        //ChipSelectId: 0 or 1
        //NOTE - THIS IS AN ASYNC METHOD, IF YOU DON'T AWAIT IT WHEN CALLING THEN IT MAY NOT BE COMPELTE WHEN YOU FIRST TRY TO USE THE OTHER IO METHODS)
        public async void Initialise(int ChipSelectId)
        {
            try
            {
                // Create SPI initialization settings
                var settings = new SpiConnectionSettings(ChipSelectId);         //CS Select line
                settings.ClockFrequency = 500000;                    //<<<<<SET BUS SPEED (10000000 = 10MHz - 100nS per bit).  10000 = Is slowest possible ClockFrequency
                settings.Mode = SpiMode.Mode0;

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
                SpiPin = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
                Debug.WriteLine(devicesInfo[0].Id);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SPI Initialisation Error", e.Message);
            }

            return;
        }

        private void startTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            HandleData();
        }

        private void HandleData()
        {

            SpiPin.TransferFullDuplex(writeData, readData);
            resultData = convertToInt(readData);

            if (resultData > 25)
            {
                if (timePressed == null)
                {
                    timePressed = new DateTime();
                }
                Debug.WriteLine(resultData);
                CalcForce(resultData);
            }
            else if (resultData < 25 && getValue() == GpioPinValue.Low)
            {
                if (_forceSensorData.Count > 0)
                {
                    SendData();
                }

            }
        }

        private void SendData()
        {
            Debug.WriteLine("Lijst laten zien");
            var highestValue = _forceSensorData.Max();
            Debug.WriteLine(highestValue);
            foreach (var data in _forceSensorData)
            {
                Debug.WriteLine(data);
            }
            ClearData();
            Debug.WriteLine("Lijst is leeg");

        }

        private void ClearData()
        {
            timePressed = null;
            _forceSensorData.Clear();
        }

        private void CalcForce(int resultData)
        {
            _forceSensorData.Add(resultData);
        }

        public int convertToInt(byte[] data)
        {
            int result;
            result = data[0] & 0x03;
            result <<= 8;
            result += data[1];
            return result;
        }

        public GpioPinValue getValue()
        {
            return gpioPin.Read();
        }

    }
}
