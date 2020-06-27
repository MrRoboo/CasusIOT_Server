using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Testserver
{
    class DbController
    {
        private String filePath = "/home/pi/";
        private StringBuilder csv = new StringBuilder();

        public void AddForceData(List<String> forceData)
        {
            String csvFile = "Force,";
            foreach (var force in forceData)
            {
                csvFile += force + ",";
            }
            csv.AppendLine(csvFile);
        }


        public void AddSpeedData(List<String> speedData)
        {
            String csvFile = "Speed,";
            foreach (var speed in speedData)
            {
                csvFile += speed + ",";
            }
            csv.AppendLine(csvFile);
        }

        public void WriteData()
        {
            File.WriteAllText(filePath, csv.ToString());
        }
    }
}
