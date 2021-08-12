using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Rocklan.Zanstat
{
    public class Rcon : ZandronumQuery
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

        public Rcon(NetworkHelper networkHelper) : base(networkHelper) { }


        /// <summary>
        /// Connects to RCon
        /// </summary>
        /// <returns></returns>
        public void ConnectToRcon(string rconPassword, int keepAlivePeriod = 5)
        {
            string salt = _networkHelper.GetSaltFromServer();

            _networkHelper.ConnectToRconServer(salt, rconPassword);

            _keepaliveTimer = new System.Timers.Timer(keepAlivePeriod * 1000);
            _keepaliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            _keepaliveTimer.AutoReset = true;
            _keepaliveTimer.Enabled = true;

            _messageReceiver = new BackgroundWorker();
            _messageReceiver.DoWork += BackgroundWorker;
            _messageReceiver.RunWorkerAsync();
        }

        public void DisconnectFromRcon()
        {
            // stop with the keepalives
            _keepaliveTimer.Enabled = false;

            // stop receiving messages
            _messageReceiver.CancelAsync();

            // tell zan we are going to disconnect
            _networkHelper.SendMessage(MessageHelpers.GetRconDisconnectMessage());

            // shut off the UDP connection
            _networkHelper.CloseConnection();
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
                byte[] result = _networkHelper.ReceiveMessage();

                byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

                if (responseCode == (byte)RconServerResponseEnum.SVRC_UPDATE)
                    handleUpdate(result);
                else if (responseCode == (byte)RconServerResponseEnum.SVRC_MESSAGE)
                    handleMessage(result);
                else
                    Console.WriteLine("** Unknown server message code: " + responseCode);
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
