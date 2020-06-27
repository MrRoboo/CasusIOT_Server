using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Testserver
{
    class GameController
    {
        DbController dbController = new DbController();

        List<String> forceData = new List<String>();
        List<String> speedData = new List<String>();

        private DispatcherTimer timer;
        private int counter = 60;

        public bool IsGameValid()
        {
            return counter == 0;
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            counter--;
            if (counter == 0)
            {
                timer.Stop();
                EndGame();
            }
        }

        public void RunGame()
        {
            StartTimer();
        }


        public void EndGame()
        {
            dbController.AddForceData(forceData);
            dbController.AddSpeedData(speedData);
            dbController.WriteData();

            ResetData();
        }

        void ResetData()
        {
            forceData.Clear();
            speedData.Clear();
        }

        public void AddForceData(float force)
        {
            forceData.Add(force.ToString());
        }

        public void AddSpeedData(float speed)
        {
            speedData.Add(speed.ToString());
        }
    }
}
