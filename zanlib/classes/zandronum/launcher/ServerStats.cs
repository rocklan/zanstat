using System;

namespace Rocklan.Zanstat
{
    public class ServerStats : ZandronumQuery
    {
        private const int SQF_NAME = 0x00000001;
        private const int SQF_URL = 0x00000002;
        
        private const int SQF_MAPNAME = 0x00000008;
        private const int SQF_IWAD = 0x00000200;

        public ServerStats(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the current map name running on the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Display()
        {
            int message = SQF_NAME | SQF_URL | SQF_MAPNAME | SQF_IWAD;
            var result = _networkHelper.GetLauncherMessageFromServer(message);

            string serverName = MessageHelpers.GetStringFromMessage(ref result);
            string serverUrl = MessageHelpers.GetStringFromMessage(ref result);
            string mapName = MessageHelpers.GetStringFromMessage(ref result);
            string iwadName = MessageHelpers.GetStringFromMessage(ref result);

            Console.WriteLine("Server name: " + serverName + ", Url: "  + serverUrl);
            Console.WriteLine("IWad name: " + iwadName);
            Console.WriteLine("Map name: " + mapName);

            return mapName;
        }
    }
}
