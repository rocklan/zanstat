using System;

namespace Rocklan.Zanstat
{
    public class ZandronumMessageEventArgs: EventArgs
    {
        public ZandronumMessageEventArgs(string message) 
        { 
            Message = message; 
        }

        public string Message { get; set; }
    }
}
