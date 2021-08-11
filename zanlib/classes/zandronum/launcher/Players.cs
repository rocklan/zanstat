﻿namespace Zanlib
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

            byte gameModeByte;
            result = MessageHelpers.GetByteFromMessage(result, out gameModeByte);
            result = MessageHelpers.GetByteFromMessage(result, out _); // instagib setting
            result = MessageHelpers.GetByteFromMessage(result, out _); // buckshot setting

            byte numPlayers;
            result = MessageHelpers.GetByteFromMessage(result, out numPlayers);

            GameModeEnum gameMode = (GameModeEnum)gameModeByte;
            var players = new Player[numPlayers];
            for (var i = 0; i < numPlayers; i++)
            {
                var player = new Player();
                players[i] = player;
                result = MessageHelpers.GetStringFromMessage(result, out player.Name);
                result = MessageHelpers.GetShortFromMessage(result, out player.FragCount);
                result = MessageHelpers.GetShortFromMessage(result, out player.Ping);
                result = MessageHelpers.GetByteFromMessage(result, out player.IsSpectating);
                result = MessageHelpers.GetByteFromMessage(result, out player.IsBot);
                if (gameMode.IsTeamGame())
                    result = MessageHelpers.GetByteFromMessage(result, out player.Team);
                result = MessageHelpers.GetByteFromMessage(result, out player.TimeOnServer);
            }

            return players;
        }
    }
}
