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

        public NetworkHelper(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;

            _udpClient = new UdpClient();
            _huffman = new Huffman();
        }


        public byte[] GetLauncherMessageFromServer(int serverQueryFlag)
        {
            byte[] message = MessageHelpers.GetLauncherMessage(serverQueryFlag);
            var compressedMessage = _huffman.Encode(message);

            _udpClient.Connect(_hostname, _port);
            _udpClient.Send(compressedMessage, compressedMessage.Length);

            var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            var compressedResult = _udpClient.Receive(ref remoteEndpoint);
            var result = _huffman.Decode(compressedResult);

            int response;
            result = MessageHelpers.GetIntFromMessage(result, out response);
            if (response != 5660023)
            {
                _udpClient.Close();
                throw ServerDeniedException.Make(response);
            }

            result = MessageHelpers.GetIntFromMessage(result, out int time);
            result = MessageHelpers.GetStringFromMessage(result, out string serverVersion);
            result = MessageHelpers.GetIntFromMessage(result, out int flags);

            _udpClient.Close();

            return result;
           
        }

        IPEndPoint _endpoint;

        public string GetSaltFromServer()
        {
            byte[] message = MessageHelpers.GetRconConnectMessage();
            var compressedMessage = _huffman.Encode(message);

            _udpClient.Connect(_hostname, _port);
            _udpClient.Send(compressedMessage, compressedMessage.Length);

            _endpoint = new IPEndPoint(IPAddress.Any, 0);

            var compressedResult = _udpClient.Receive(ref _endpoint);
            var result = _huffman.Decode(compressedResult);

            result = MessageHelpers.GetByteFromMessage(result, out byte response);

            if (response == (byte)RconServerResponseEnum.SVRC_BANNED)
                throw new RconDeniedException("Banned by the server");

            if (response == (byte)RconServerResponseEnum.SVRC_OLDPROTOCOL)
            {
                result = MessageHelpers.GetByteFromMessage(result, out byte wrongServerProtocolVersion);
                result = MessageHelpers.GetStringFromMessage(result, out string serverVersionString);
                throw new RconDeniedException("Server uses a newer protocol than version 4: " + wrongServerProtocolVersion + ", Version string: " + serverVersionString);
            }

            if (response != (byte)RconServerResponseEnum.SVRC_SALT)
                throw new RconDeniedException("Unexpected server response, expected SVRC_SALT, received: " + response);

            MessageHelpers.GetStringFromMessage(result, out string salt);
            
            return salt;
        }

        public void connectToServer(string salt, string rconPassword)
        {
            byte[] saltHashed = CreateMD5(salt, rconPassword);
            byte[] connectMessage = MessageHelpers.GetRconAuthenticateMessage(saltHashed);
            var compressedMessage = _huffman.Encode(connectMessage);
            
            _udpClient.Send(compressedMessage, compressedMessage.Length);

            var compressedResult = _udpClient.Receive(ref _endpoint);
            var result = _huffman.Decode(compressedResult);

            result = MessageHelpers.GetByteFromMessage(result, out byte response);
            if (response == (byte)RconServerResponseEnum.SVRC_INVALIDPASSWORD)
            {
                _udpClient.Close();
                throw new RconDeniedException("Server replied saying invalid Rcon password");
            }

            if (response != (byte)RconServerResponseEnum.SVRC_LOGGEDIN)
                throw new RconDeniedException("Unexpected server response, expected login, got: " + response);

            result = MessageHelpers.GetByteFromMessage(result, out byte serverProtocolVersion);
            result = MessageHelpers.GetStringFromMessage(result, out string hostname);

            Console.WriteLine("Connected to RCon! Hostname: " + hostname + ", using protocol: " + serverProtocolVersion);


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
