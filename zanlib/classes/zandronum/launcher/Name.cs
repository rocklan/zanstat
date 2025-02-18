﻿namespace Rocklan.Zanstat
{
    public class Name: ZandronumQuery
    {
        private const int SQF_NAME = 0x00000001;
        
        public Name(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the name of the specified server (set via sv_hostname).
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_NAME);
            string serverName = MessageHelpers.GetStringFromMessage(ref result);
            return serverName;
        }
    }
}
