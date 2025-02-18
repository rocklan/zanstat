﻿namespace Rocklan.Zanstat
{
    public class MaxPlayers: ZandronumQuery
    {
        private const int SQF_MAXPLAYERS = 0x00000020;

        public MaxPlayers(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the maximum number of players for the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_MAXPLAYERS);
            byte maxPlayers = MessageHelpers.GetByteFromMessage(ref result);
            return maxPlayers;
        }
    }
}
