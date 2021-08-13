using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Rocklan.Zanstat
{
    public class Rcon
    {
        /// <summary>
        /// This timer keeps us alive with the server
        /// </summary>
        private System.Timers.Timer _keepaliveTimer;

        /// <summary>
        /// This sits in the background polling for messages
        /// </summary>
        private BackgroundWorker _messageReceiver;

        // We raise these events whenever anything happens

        public event EventHandler ServerMessage;
        public event EventHandler MapChange;
        public event EventHandler PlayerChange;
        public event EventHandler RconAdminsChange;

        private NetworkHelperStateful _networkHelper;
        private readonly string _hostname;
        private readonly int _port;

        public Rcon(string hostname, int port)
        {
            _networkHelper = new NetworkHelperStateful();
            this._hostname = hostname;
            this._port = port;
        }


        /// <summary>
        /// Connects to RCon
        /// </summary>
        /// <param name="rconPassword">Password to connect with</param>
        /// <param name="keepAlivePeriod">Seconds to pause between keepalives. 0 if you don't want any keepalives.</param>
        public int ConnectToRcon(string rconPassword, int keepAlivePeriod = 5)
        {
            _networkHelper.OpenConnection(_hostname, _port);

            string salt = _networkHelper.GetSaltFromServer();

            int receivePort = _networkHelper.AuthenticateToRconServer(salt, rconPassword);

            if (keepAlivePeriod > 0)
            {
                _keepaliveTimer = new System.Timers.Timer(keepAlivePeriod * 1000);
                _keepaliveTimer.Elapsed += KeepAliveTimer_Elapsed;
                _keepaliveTimer.AutoReset = true;
                _keepaliveTimer.Enabled = true;
            }

            _messageReceiver = new BackgroundWorker();
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.DoWork += BackgroundWorker;
            _messageReceiver.RunWorkerAsync();

            return receivePort;
        }

        public void DisconnectFromRcon()
        {
            if (_keepaliveTimer != null)
                _keepaliveTimer.Enabled = false;

            // tell zan we want to disconnect
            _networkHelper.SendMessage(MessageHelpers.GetRconDisconnectMessage());

            // shut off the UDP connection 
            _networkHelper.CloseConnection();

            // stop receiving messages
            _messageReceiver.CancelAsync();
        }

        public void SendCommand(string command)
        {
            _networkHelper.SendCommand(command);
        }
         
        private void BackgroundWorker(object sender, DoWorkEventArgs e)
        {
            while (!_messageReceiver.CancellationPending)
            {
                // This blocks until we get a message from the server
                try
                {
                    byte[] result = _networkHelper.ReceiveMessage();

                    byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

                    if (responseCode == (byte)RconServerResponseEnum.SVRC_UPDATE)
                        handleUpdate(result);

                    if (responseCode == (byte)RconServerResponseEnum.SVRC_MESSAGE)
                        handleMessage(result);
                }
                catch (SocketException)
                {
                    break;
                }
            }

            e.Cancel = true;
        }

        private void KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _networkHelper.SendMessage(MessageHelpers.GetRconKeepAliveMessage());
        }

        private void handleMessage(byte[] result)
        {
            string message = MessageHelpers.GetStringFromMessage(ref result);
            OnServerMessage(new ZandronumMessageEventArgs(message));
        }

        private void handleUpdate(byte[] result)
        {
            byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

            if (responseCode == (byte)RconServerUpdateEnum.SVRCU_ADMINCOUNT)
            {
                byte rconAdmins = MessageHelpers.GetByteFromMessage(ref result);
                OnRconAdminsChange(new ZandronumRconAdminsChangeEventArgs(rconAdmins));
            }
            
            if (responseCode == (byte)RconServerUpdateEnum.SVRCU_PLAYERDATA)
            {
                byte playerCount = MessageHelpers.GetByteFromMessage(ref result);
                string[] players = new string[playerCount];
                for (int i = 0; i < playerCount; i++)
                {
                    string name = MessageHelpers.GetStringFromMessage(ref result);
                    players[i] = name;
                }
                OnPlayerChange(new ZandronumPlayerChangeEventArgs(players));
            }
            
            if (responseCode == (byte)RconServerUpdateEnum.SVRCU_MAP)
            {
                string newMapName = MessageHelpers.GetStringFromMessage(ref result);
                OnMapChange(new ZandronumMapChangeEventArgs(newMapName));
            }
        }





        protected virtual void OnServerMessage(EventArgs e)
        {
            EventHandler handler = ServerMessage;
            handler?.Invoke(this, e);
        }
        protected virtual void OnMapChange(EventArgs e)
        {
            EventHandler handler = MapChange;
            handler?.Invoke(this, e);
        }
        protected virtual void OnPlayerChange(EventArgs e)
        {
            EventHandler handler = PlayerChange;
            handler?.Invoke(this, e);
        }
        protected virtual void OnRconAdminsChange(EventArgs e)
        {
            EventHandler handler = RconAdminsChange;
            handler?.Invoke(this, e);
        }
    }
}
