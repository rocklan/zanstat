using System.Net;
using System.Net.Sockets;

namespace Rocklan.Zanstat
{
    public class NetworkHelper
    {
        private readonly string _hostname;
        private readonly int _port;
        private readonly Huffman _huffman;

        public NetworkHelper(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;

            _huffman = new Huffman();
        }


        public byte[] GetLauncherMessageFromServer(int serverQueryFlag)
        {
            var udpClient = new UdpClient();
            udpClient.Connect(_hostname, _port);

            var msg = MessageHelpers.GetLauncherMessage(serverQueryFlag);
            var compressedMessage = _huffman.Encode(msg);
            udpClient.Send(compressedMessage, compressedMessage.Length);

            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            var compressedResult = udpClient.Receive(ref endpoint);

            var result = _huffman.Decode(compressedResult);

            int response = MessageHelpers.GetIntFromMessage(ref result);
            if (response != 5660023)
            {
                udpClient.Close();
                throw ServerDeniedException.Make(response);
            }

            int time = MessageHelpers.GetIntFromMessage(ref result);
            string serverVersion = MessageHelpers.GetStringFromMessage(ref result);
            int flags = MessageHelpers.GetIntFromMessage(ref result);

            udpClient.Close();

            return result;

        }
    }
}
