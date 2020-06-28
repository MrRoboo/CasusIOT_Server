using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        //private DispatcherTimer timer;
        //private int counter = 20;
        private Random randomizer = new Random();
        public bool gameState = false;

        int sessionID;
        int gameDataID;

        private SocketServer server;
        private int? lastClientIndex;

        public GameController(int patientID, SocketServer server)
        {
            sessionID = dbController.CreateSessionFor(patientID, DateTime.Now);
            this.server = server;
        }



        public async void SendAwaitTouchClient(int clientIndex)
        {
            await Task.Delay(1000);
            server.VerstuurBericht("touch", clientIndex);
            await Task.Delay(1000);
            server.VerstuurBericht("publisher", 0);
            //server.VerstuurBericht("publisher", (int)lastClientIndex);

        }



        public void SendGameStart()
        {
            server.VerstuurBerichtIedereen("start");

        }



        public async void SendGameEnd()
        {
            await Task.Delay(1000);
            server.VerstuurBericht("off", 0);
            await Task.Delay(1000);
            server.VerstuurBerichtIedereen("end");
            //server.VerstuurBericht("off", (int)lastClientIndex);

        }



        public bool IsGameValid()
        {
            return gameState;
        }



        //private void StartTimer()
        //{
        //    timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMilliseconds(1000);
        //    timer.Tick += Timer_Tick;
        //    timer.Start();
        //}



        //private void Timer_Tick(object sender, object e)
        //{
        //    Debug.WriteLine(counter);
        //    counter--;
        //    if (counter == 0)
        //    {
        //        timer.Stop();
        //        EndGame();
        //        Debug.WriteLine("End game");
        //    }
        //}



        public async void RunGame()
        {
            //StartTimer();
            await Task.Delay(1000);
            SendGameStart();
            await Task.Delay(1000);
            DetermineTouchClient();
        }




        public void DetermineTouchClient()
        {
            int maxClientIndex = server.teams.Count - 1;
            int clientIndex = 0; //randomizer.Next(0, maxClientIndex);

            //if (lastClientIndex != null)
            //{
            //    while (clientIndex == lastClientIndex)
            //    {
            //        clientIndex = randomizer.Next(0, maxClientIndex);
            //    }
            //    lastClientIndex = clientIndex;
            //}

            SendAwaitTouchClient(clientIndex);
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



            SendGameEnd();
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
