using System;

namespace Zanlib
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
