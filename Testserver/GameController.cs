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

        List<Dictionary<string, object>> forceData = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> speedData = new List<Dictionary<string, object>>();

        private DispatcherTimer timer;
        private int counter = 60;

        int sessionID;
        int gameDataID;

        public GameController(int patientID)
        {
            sessionID = dbController.CreateSessionFor(patientID, DateTime.Now);
        }

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


        public void SetNewGame()
        {
            gameDataID = dbController.CreateGameDataFor(sessionID);
        }

        public void EndGame()
        {
            foreach (var fd in forceData)
            {
                dbController.CreateForceDataFor(gameDataID, (float)fd["Force"]);
            }

            foreach (var sd in speedData)
            {
                dbController.CreateSpeedDataFor(gameDataID, (DateTime)sd["TimeTriggered"], (DateTime)sd["TimePressed"], (float)sd["Distance"]);
            }

            ResetData();
        }

        void ResetData()
        {
            forceData.Clear();
            speedData.Clear();
        }

        public void AddForceData(float force)
        {
            Dictionary<string, object> fd = new Dictionary<string, object>();
            fd.Add("Force", force);

            forceData.Add(fd);
        }

        public void AddSpeedData(DateTime triggered, DateTime pressed, float distance)
        {
            Dictionary<string, object> sd = new Dictionary<string, object>();
            sd.Add("TimeTriggered", triggered);
            sd.Add("TimePressed", pressed);
            sd.Add("Distance", distance);

            speedData.Add(sd);
        }
    }
}
