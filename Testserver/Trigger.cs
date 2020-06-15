using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testserver
{
    class Trigger
    {
        private String _sensorName;
        private bool sensorValue;
        private static bool _aanraakKnop;


        //############################################
        //*****************CONSTRUCTOR****************
        //############################################

        public Trigger(String sensorName)
        {
            _sensorName = sensorName;
            ToggleValue(_aanraakKnop);
        }


        //############################################
        //*******************METHODS******************
        //############################################

        public string SensorActivation()
        {
            if (_aanraakKnop)
            {
                return "Voetganger sensor heeft iets gedetecteerd";
            }
            else
            {
                return "Voetganger sensor is gedeactiveerd";
            }

        }

        public bool ToggleValue(bool value)
        {
            if (value)
            {
                this.sensorValue = false;
                return false;
            }
            else
            {
                this.sensorValue = true;
                return true;
            }
        }

        //############################################
        //*******************GETTERS******************
        //############################################

        public String GetSensorName()
        {
            return _sensorName;
        }

        public bool GetSensorValue()
        {
            return sensorValue;
        }
    }
}
