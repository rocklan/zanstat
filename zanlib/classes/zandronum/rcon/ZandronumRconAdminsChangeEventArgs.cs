using System;

namespace Zanlib
{
    public class ZandronumRconAdminsChangeEventArgs : EventArgs
    {
        public ZandronumRconAdminsChangeEventArgs(int admins)
        {
            Admins = admins;
        }

        public int Admins { get; set; }
    }
}
