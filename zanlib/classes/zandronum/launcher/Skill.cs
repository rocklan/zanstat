﻿namespace Rocklan.Zanstat
{
    public class Skill: ZandronumQuery
    {
        private const int SQF_GAMESKILL = 0x00001000;

        public Skill(NetworkHelper networkHelper) : base(networkHelper) { }

        /// <summary>
        /// Returns the skill level on the server
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_GAMESKILL);
            byte skill = MessageHelpers.GetByteFromMessage(ref result);
            return skill;
        }
    }
}
