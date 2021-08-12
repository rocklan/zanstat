using System;

namespace Rocklan.Zanstat
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
