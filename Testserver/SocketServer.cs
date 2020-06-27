using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Testserver
{
    class SocketServer
    {

        //Poortnummer waar de server op luistert (zie constructor)
        private readonly int _port;
        //Listner die luistert voor nieuwe data op de meegegeven poort        
        private StreamSocketListener listener;

        public delegate void DataOntvangenDelegate(string data);
        public DataOntvangenDelegate OnDataOntvangen;

        public List<SocketClient> teams;
        private List<string> teamIP = new List<string>();

        private String _data;
        private static bool _dataReceived;

        /// <summary>
        /// Initialiseer en start de server
        /// </summary>
        /// <param name="port"></param>
        public SocketServer(int port)
        {
            teams = new List<SocketClient>();
            _port = port;
            Start();

        }


        /// <summary>
        /// Start de listner die luistert naar inkomende connecties
        /// </summary>
        public async void Start()
        {
            listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;

            Debug.WriteLine("Wacht op connecties..");
            await listener.BindServiceNameAsync(_port.ToString());
        }

        /// <summary>
        /// Zodra een bericht binnen komt, wordt via de listner deze methode aangeroepen.
        /// </summary>
        /// <param name="bericht"></param>
        public async void Server_OnDataOntvangen(string bericht)
        {
            IPAddress addr;
            //Wanneer het bericht een IP-adress is wordt deze geregistreerd en toegevoegd aan een lijst.
            if (IPAddress.TryParse(bericht, out addr))
            {
                Debug.WriteLine("Connectie gemaakt met client: " + bericht);
                //Als hij niet in de lijst voorkomt, is het een nieuw team
                Task.Delay(150).Wait();
                if (!teamIP.Contains(bericht))
                {
                    teamIP.Add(bericht);
                    SocketClient tmp = new SocketClient(bericht, 9000);
                    teams.Add(tmp);
                    int clientIndex = teams.Count - 1;
                    await Task.Delay(1000);
                    VerstuurBericht("connectie gemaakt", clientIndex);
                }
                else
                {
                    Debug.WriteLine("IP adres heeft al connectie, niks mee doen");
                }
            }
            //Wanneer het bericht geen IP-adress is zal het berichten worden ontvangen als data.
            else
            {
                _data = bericht;
                _dataReceived = true;
                //Debug.WriteLine("Bericht ontvangen van de server: " + bericht);
            }
        }


        /// <summary>
        /// Zodra de listner een nieuwe connectie binnen krijgt, wordt deze methode aangeroepen
        /// Deze zal het bericht controleren op compleetheid en daarna doorsturen naar de OnDataOntvangen methode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint)) return; //Disconnect
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength) return; //Disconnect

                    //Zodra data binnen is en er is een functie gekoppeld aan het event:                    
                    if (OnDataOntvangen != null)
                    {
                        //Trigger het event, zodat er iets gedaan wordt met de ontvangen data
                        OnDataOntvangen(reader.ReadString(actualStringLength));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// In dit geval wordt hetzelfde bericht gestuurd naar iedere socket die aangemeld wordt bij de server
        /// </summary>
        /// <param name="bericht">Het te versturen bericht</param>
        public void VerstuurBerichtIedereen(string bericht)
        {
            foreach (SocketClient sc in teams)
            {
                if (sc != null) sc.Verstuur(bericht);
            }
        }

        public void VerstuurBericht(string bericht, int clientIndex)
        {
            teams[clientIndex].Verstuur(bericht);
        }

        public String GetData()
        {
            return _data;
        }

        public bool GetDataReceived()
        {
            return _dataReceived;
        }

        public void SetDataReceived(bool value)
        {
            _dataReceived = false;
        }
    }
}
