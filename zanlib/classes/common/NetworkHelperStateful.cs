using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rocklan.Zanstat
{
    public class NetworkHelperStateful
    {
        private readonly Huffman _huffman;
        private IPEndPoint _endpoint;
        private UdpClient _udpClient;

        public NetworkHelperStateful()
        {
            _huffman = new Huffman();
        }

        public void OpenConnection(string hostname, int port)
        {
            _udpClient = new UdpClient();
            _udpClient.Connect(hostname, port);
        }

        public void CloseConnection()
        {
            if (_udpClient != null)
                _udpClient.Close();
        }


        public string GetSaltFromServer()
        {
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

            return MessageHelpers.GetStringFromMessage(ref result);
        }

        public int AuthenticateToRconServer(string salt, string rconPassword)
        {
            byte[] md5 = CreateMD5(salt, rconPassword);
            SendMessage(MessageHelpers.GetRconAuthenticateMessage(md5));

            byte[] result = ReceiveMessage();
            byte responseCode = MessageHelpers.GetByteFromMessage(ref result);

            if (responseCode == (byte)RconServerResponseEnum.SVRC_INVALIDPASSWORD)
            {
                _udpClient.Close();
                throw new RconDeniedException("Server replied saying invalid Rcon password");
            }

            if (responseCode != (byte)RconServerResponseEnum.SVRC_LOGGEDIN)
            {
                _udpClient.Close();
                throw new RconDeniedException("Unexpected server response, expected login, got: " + responseCode);
            }

            MessageHelpers.GetByteFromMessage(ref result); // serverProtocolVersion
            MessageHelpers.GetStringFromMessage(ref result); // hostname

            return ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;
        }

        internal void SendCommand(string line)
        {
            SendMessage(MessageHelpers.GetCommandMessage(line));
        }


        private static byte[] CreateMD5(string salt, string rconPassword)
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




        public void SendMessage(byte[] message)
        {
            var compressedMessage = _huffman.Encode(message);
            _udpClient.Send(compressedMessage, compressedMessage.Length);
        }



        public byte[] ReceiveMessage()
        {
            if (_endpoint == null)
                _endpoint = new IPEndPoint(IPAddress.Any, 0);

            var compressedResult = _udpClient.Receive(ref _endpoint);

            var result = _huffman.Decode(compressedResult);
            return result;
        }
    }
}
