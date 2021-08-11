using System;

namespace Zanlib
{
    public class Rcon : ZandronumQuery
    {
        public Rcon(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Connects to RCon
        /// </summary>
        /// <returns></returns>
        public void DisplayData(string rconPassword)
        {
            string salt = _networkHelper.GetSaltFromServer();
            _networkHelper.connectToServer(salt, rconPassword);

        }
    }
}
