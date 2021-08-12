using System.Collections.Generic;

namespace Rocklan.Zanstat
{
    public class Pwads: ZandronumQuery
    {
        private const int SQF_PWADS = 0x00000040;

        public Pwads(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the pwads loaded by the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string[] Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_PWADS);
            byte numPwads = MessageHelpers.GetByteFromMessage(ref result);

            var pwads = new List<string>();
            for (var i = 0; i < numPwads; i++)
            {
                string pwad = MessageHelpers.GetStringFromMessage(ref result);
                pwads.Add(pwad);
            }

            return pwads.ToArray();
        }
    }
}
