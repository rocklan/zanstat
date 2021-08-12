using System;

namespace Zanlib
{
    public class ZandronumMapChangeEventArgs : EventArgs
    {
        public ZandronumMapChangeEventArgs(string mapName)
        {
            MapName = mapName;
        }

        public string MapName { get; set; }
    }
}
