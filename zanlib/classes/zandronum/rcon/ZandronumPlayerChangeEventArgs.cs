using System;
using System.Collections.Generic;

namespace Zanlib
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
