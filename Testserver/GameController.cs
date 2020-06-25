using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testserver
{
    class GameController
    {
        DbController dbController = new DbController();

        List<Dictionary<string, object>> forceData = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> speedData = new List<Dictionary<string, object>>();

        int sessionID;
        int gameDataID;

        public GameController(int patientID)
        {
            sessionID = dbController.CreateSessionFor(patientID, DateTime.Now);
        }

        public void setNewGame()
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

            resetData();
        }

        void resetData()
        {
            forceData.Clear();
            speedData.Clear();
        }

        public void AddForceData(int objectID, float force)
        {
            Dictionary<string, object> fd = new Dictionary<string, object>();
            fd.Add("ObjectID", objectID);
            fd.Add("Force", force);

            forceData.Add(fd);
        }

        public void AddSpeedData(int senderID, int receiverID, DateTime triggered, DateTime pressed, float distance)
        {
            Dictionary<string, object> sd = new Dictionary<string, object>();
            sd.Add("SenderID", senderID);
            sd.Add("ReceiverID", receiverID);
            sd.Add("TimeTriggered", triggered);
            sd.Add("TimePressed", pressed);
            sd.Add("Distance", distance);

            speedData.Add(sd);
        }
    }
}
