using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Zanlib
{
    public class NetworkHelper
    {
        private readonly string _hostname;
        private readonly int _port;
        private readonly UdpClient _udpClient;
        private readonly Huffman _huffman;

        private IPEndPoint _endpoint;
        private System.Timers.Timer keepaliveTimer;


        public NetworkHelper(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;

            _udpClient = new UdpClient();
            _huffman = new Huffman();
        }

        private void SendMessage(byte[] message)
        {
            var compressedMessage = _huffman.Encode(message);
            _udpClient.Send(compressedMessage, compressedMessage.Length);
        }

        private byte[] ReceiveMessage()
        {
            if (_endpoint == null)
                _endpoint = new IPEndPoint(IPAddress.Any, 0);

            var compressedResult = _udpClient.Receive(ref _endpoint);
            var result = _huffman.Decode(compressedResult);
            return result;
        }

        public byte[] GetLauncherMessageFromServer(int serverQueryFlag)
        {
            _udpClient.Connect(_hostname, _port);

            SendMessage(MessageHelpers.GetLauncherMessage(serverQueryFlag));
            byte[] result = ReceiveMessage();
            
            result = MessageHelpers.GetIntFromMessage(result, out int response);
            if (response != 5660023)
            {
                _udpClient.Close();
                throw ServerDeniedException.Make(response);
            }

            result = MessageHelpers.GetIntFromMessage(result, out int time);
            string serverVersion = MessageHelpers.GetStringFromMessage(ref result);
            result = MessageHelpers.GetIntFromMessage(result, out int flags);

            _udpClient.Close();

            return result;
           
        }


        public string GetSaltFromServer()
        {
            _udpClient.Connect(_hostname, _port);

            SendMessage(MessageHelpers.GetRconConnectMessage());
            byte[] result = ReceiveMessage();

            byte response = MessageHelpers.GetByteFromMessage(ref result);

            if (response == (byte)RconServerResponseEnum.SVRC_BANNED)
                throw new RconDeniedException("Banned by the server");

            if (response == (byte)RconServerResponseEnum.SVRC_OLDPROTOCOL)
            {
                response = MessageHelpers.GetByteFromMessage(ref result);
                string wrongServerProtocolVersion = MessageHelpers.GetStringFromMessage(ref result);
                throw new RconDeniedException("Server uses a newer protocol than version 4: " + response + ", Version string: " + wrongServerProtocolVersion);
            }

            if (response != (byte)RconServerResponseEnum.SVRC_SALT)
                throw new RconDeniedException("Unexpected server response, expected SVRC_SALT, received: " + response);

            string salt = MessageHelpers.GetStringFromMessage(ref result);
            
            return salt;
        }

        public void ConnectToRconServer(string salt, string rconPassword)
        {
            SendMessage(MessageHelpers.GetRconAuthenticateMessage(CreateMD5(salt, rconPassword)));

            byte[] result = ReceiveMessage();
            byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

            if (responseCode == (byte)RconServerResponseEnum.SVRC_INVALIDPASSWORD)
            {
                _udpClient.Close();
                throw new RconDeniedException("Server replied saying invalid Rcon password");
            }

            if (responseCode != (byte)RconServerResponseEnum.SVRC_LOGGEDIN)
                throw new RconDeniedException("Unexpected server response, expected login, got: " + responseCode);

            byte serverProtocolVersion = MessageHelpers.GetByteFromMessage(ref result);
            string hostname = MessageHelpers.GetStringFromMessage(ref result);

            Console.WriteLine("Connected to RCon! Hostname: " + hostname + ", using protocol: " + serverProtocolVersion);

            keepaliveTimer = new System.Timers.Timer(5000);
            keepaliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            keepaliveTimer.AutoReset = true;
            keepaliveTimer.Enabled = true;

            while (!Console.KeyAvailable)
            {
                result = ReceiveMessage();

                responseCode = MessageHelpers.GetByteFromMessage(ref result);

                if (responseCode == (byte)RconServerResponseEnum.SVRC_UPDATE)
                    handleUpdate(result);
                else if (responseCode == (byte)RconServerResponseEnum.SVRC_MESSAGE)
                    handleMessage(result);
                else
                    Console.WriteLine("Recieved server response: " + responseCode);
                
            }
            _udpClient.Close();
        }

        private void KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendMessage(MessageHelpers.GetRconKeepAliveMessage());
        }

        private void handleMessage(byte[] result)
        {
            string message = MessageHelpers.GetStringFromMessage(ref result);
            Console.Write("Server Message: " + message);
        }

        private void handleUpdate(byte[] result)
        {
            byte response = MessageHelpers.GetByteFromMessage(ref result);

            if (response == (byte)RconServerUpdateEnum.SVRCU_ADMINCOUNT)
            {
                byte rconAdmins = MessageHelpers.GetByteFromMessage(ref result);
                Console.WriteLine("New number of rcon admins: " + rconAdmins);
            }
            else
            if (response == (byte)RconServerUpdateEnum.SVRCU_PLAYERDATA)
            {
                byte playerCount = MessageHelpers.GetByteFromMessage(ref result);
                Console.WriteLine("Number of players is now: " + playerCount);
                for (int i=0;i< playerCount;i++)
                {
                    string name = MessageHelpers.GetStringFromMessage(ref result);
                    Console.WriteLine("    " + name);
                }
            }
        }

        public static byte[] CreateMD5(string salt, string rconPassword)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(salt + rconPassword);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string hexString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                byte[] hexStringBytes = System.Text.Encoding.ASCII.GetBytes(hexString);

                return hexStringBytes;
            }
        }
    }
}
