using System;
using System.Collections.Generic;

namespace Rocklan.Zanstat
{
    public class ZandronumPlayerChangeEventArgs : EventArgs
    {
        public ZandronumPlayerChangeEventArgs(IEnumerable<string> players)
        {
            Players = players;
        }

        public IEnumerable<string> Players { get; set; }
    }
}
