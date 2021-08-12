namespace Rocklan.Zanstat
{
    public class Players: ZandronumQuery
    {
        private const int SQF_GAMETYPE = 0x00000080;
        private const int SQF_NUMPLAYERS = 0x00080000;
        private const int SQF_PLAYERDATA = 0x00100000;

        public Players(NetworkHelper networkHelper) : base(networkHelper) { }

        public class Player
        {
            public string Name;
            public short FragCount;
            public short Ping;
            public byte IsSpectating;
            public byte IsBot;
            public byte Team; // 255 is no team
            public byte TimeOnServer; // in minutes

            public override string ToString()
            {
                return $"Name={Name}, FragCount={FragCount}, IsSpectating={IsSpectating}, IsBot={IsBot}, Team={Team}, TimeOnServer={TimeOnServer}";
            }
        };

        /// <summary>
        /// Returns the players on the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Player[] Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_GAMETYPE | SQF_NUMPLAYERS | SQF_PLAYERDATA);

            byte gameModeByte = MessageHelpers.GetByteFromMessage(ref result);
            MessageHelpers.GetByteFromMessage(ref result); // instagib setting
            MessageHelpers.GetByteFromMessage(ref result); // buckshot setting

            byte numPlayers = MessageHelpers.GetByteFromMessage(ref result);

            GameModeEnum gameMode = (GameModeEnum)gameModeByte;
            var players = new Player[numPlayers];
            for (var i = 0; i < numPlayers; i++)
            {
                var player = new Player();
                players[i] = player;
                player.Name = MessageHelpers.GetStringFromMessage(ref result);
                player.FragCount = MessageHelpers.GetShortFromMessage(ref result);
                player.Ping = MessageHelpers.GetShortFromMessage(ref result);
                player.IsSpectating = MessageHelpers.GetByteFromMessage(ref result);
                player.IsBot = MessageHelpers.GetByteFromMessage(ref result);
                if (gameMode.IsTeamGame())
                    player.Team = MessageHelpers.GetByteFromMessage(ref result);
                player.TimeOnServer = MessageHelpers.GetByteFromMessage(ref result);
            }

            return players;
        }
    }
}
