namespace Zanlib
{
    public class Limits: ZandronumQuery
    {
        private const int SQF_LIMITS = 0x00010000;

        public Limits(NetworkHelper networkHelper) : base(networkHelper) { }

        public struct Limit
        {
            public short FragLimit;
            public short TimeLimit;
            public short TimeLeft;
            public short DuelLimit;
            public short PointLimit;
            public short WinLimit;

            public override string ToString()
            {
                return $"FragLimit={FragLimit}, TimeLimit={TimeLimit}, TimeLeft={TimeLeft}, DuelLimit={DuelLimit}, PointLimit={PointLimit}, WinLimit={WinLimit}";
            }
        };

        /// <summary>
        /// Returns the limits on the server.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Limit Get()
        {
            var result = _networkHelper.GetLauncherMessageFromServer(SQF_LIMITS);
            var data = new Limit();
            data.FragLimit = MessageHelpers.GetShortFromMessage(ref result);
            data.TimeLimit = MessageHelpers.GetShortFromMessage(ref result);
            if (data.TimeLimit > 0)
                data.TimeLeft = MessageHelpers.GetShortFromMessage(ref result);
            data.DuelLimit = MessageHelpers.GetShortFromMessage(ref result);
            data.PointLimit = MessageHelpers.GetShortFromMessage(ref result);
            data.WinLimit = MessageHelpers.GetShortFromMessage(ref result);
            return data;
        }
    }
}
