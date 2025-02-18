﻿namespace Rocklan.Zanstat
{
    public class Teams: ZandronumQuery
    {
        private const int SQF_TEAMINFO_NUMBER = 0x00200000;
        private const int SQF_TEAMINFO_NAME = 0x00400000;
        private const int SQF_TEAMINFO_COLOR = 0x00800000;
        private const int SQF_TEAMINFO_SCORE = 0x01000000;

        public Teams(NetworkHelper networkHelper) : base(networkHelper) { }

        public class Team
        {
            public string Name;
            public int Colour;
            public short Score;

            public override string ToString()
            {
                return $"Name={Name}, Colour={Colour}, Score={Score}";
            }
        };

        /// <summary>
        /// Returns the teams on the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Team[] Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_TEAMINFO_NUMBER | SQF_TEAMINFO_NAME | SQF_TEAMINFO_COLOR | SQF_TEAMINFO_SCORE);

            byte numTeams = MessageHelpers.GetByteFromMessage(ref result );

            var teams = new Team[numTeams];

            // Get team names.
            for (var i = 0; i < numTeams; i++)
            {
                var team = new Team();
                teams[i] = team;
                team.Name = MessageHelpers.GetStringFromMessage(ref result);
            }

            // Get team colours.
            for (var i = 0; i < numTeams; i++)
                teams[i].Colour = MessageHelpers.GetIntFromMessage(ref result);

            // Get team scores.
            for (var i = 0; i < numTeams; i++)
                teams[i].Score = MessageHelpers.GetShortFromMessage(ref result);

            return teams;
        }
    }
}
