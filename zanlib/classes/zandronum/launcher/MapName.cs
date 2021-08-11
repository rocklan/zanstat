namespace Zanlib
{
    public class MapName: ZandronumQuery
    {
        private const int SQF_MAPNAME = 0x00000008;

        public MapName(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the current map name running on the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_MAPNAME);
            string mapName = MessageHelpers.GetStringFromMessage(ref result);
            return mapName;
        }
    }
}
