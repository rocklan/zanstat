using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Zanlib
{
    public class Rcon : ZandronumQuery
    {
        private System.Timers.Timer keepaliveTimer;

        public Rcon(NetworkHelper networkHelper) : base(networkHelper) { }


        /// <summary>
        /// Connects to RCon
        /// </summary>
        /// <returns></returns>
        public void DisplayData(string rconPassword)
        {
            string salt = _networkHelper.GetSaltFromServer();

            _networkHelper.ConnectToRconServer(salt, rconPassword);

            keepaliveTimer = new System.Timers.Timer(5000);
            keepaliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            keepaliveTimer.AutoReset = true;
            keepaliveTimer.Enabled = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BackgroundWorker;
            bw.RunWorkerAsync();

            string line = null;
            while (line != "quit")
            {
                Console.WriteLine("Please enter your command to send:");
                line = Console.ReadLine();

                _networkHelper.SendCommand(line);
            }
        }

        private void BackgroundWorker(object sender, DoWorkEventArgs e)
        {
            while (1==1)
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
        }

        private void KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _networkHelper.SendMessage(MessageHelpers.GetRconKeepAliveMessage());
        }




        private void handleMessage(byte[] result)
        {
            string message = MessageHelpers.GetStringFromMessage(ref result);
            int colorTag = message.IndexOf("\\c-");
            if (colorTag != -1)
            {
                string firstBit = message.Substring(0, colorTag);
                string endBit = message.Substring(colorTag + 3, message.Length - colorTag - 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(firstBit);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(endBit);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(message);
            }

        }

        private void handleUpdate(byte[] result)
        {
            byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

            if (responseCode == (byte)RconServerUpdateEnum.SVRCU_ADMINCOUNT)
            {
                byte rconAdmins = MessageHelpers.GetByteFromMessage(ref result);
                Console.WriteLine("New number of rcon admins: " + rconAdmins);
            }
            else
            if (responseCode == (byte)RconServerUpdateEnum.SVRCU_PLAYERDATA)
            {
                byte playerCount = MessageHelpers.GetByteFromMessage(ref result);
                Console.WriteLine("Number of players is now: " + playerCount);
                for (int i = 0; i < playerCount; i++)
                {
                    string name = MessageHelpers.GetStringFromMessage(ref result);
                    Console.WriteLine("    " + name);
                }
            }
            else if (responseCode == (byte)RconServerUpdateEnum.SVRCU_MAP)
            {
                string newMapName = MessageHelpers.GetStringFromMessage(ref result);
                Console.WriteLine("New map: " + newMapName);
            }
            else
            {
                Console.WriteLine("** Unrecognised server update code: " + responseCode);
            }
        }
    }
}
