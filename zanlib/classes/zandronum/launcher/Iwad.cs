using System.Collections.Generic;

namespace Rocklan.Zanstat
{

    public class Iwad: ZandronumQuery
    {
        private const int SQF_IWAD = 0x00000200;

        public Iwad(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the iwad used by the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_IWAD);
            string iwad = MessageHelpers.GetStringFromMessage(ref result);
            return iwad;
        }
    }
}
