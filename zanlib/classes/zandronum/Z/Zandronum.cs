using System;
using System.Collections.Generic;
using System.Text;

namespace Rocklan.Zanstat
{
    public partial class Zandronum
    {
        
        private readonly NetworkHelper networkHelper;

        public Zandronum(string ServerName = "127.0.0.1", int Port = 10666)
        {
            networkHelper = new NetworkHelper(ServerName, Port);

            Iwad = new Iwad(networkHelper);
            Limits = new Limits(networkHelper);
            MapName = new MapName(networkHelper);
            MaxPlayers = new MaxPlayers(networkHelper);
            Name = new Name(networkHelper);
            Players = new Players(networkHelper);
            PWads = new Pwads(networkHelper);
            Skill = new Skill(networkHelper);
            Teams = new Teams(networkHelper);
            ServerStats = new ServerStats(networkHelper);
            Rcon = new Rcon(networkHelper);
        }


        public ServerStats ServerStats { get; private set; }
        public Iwad Iwad { get; private set; }
        public Limits Limits { get; private set; }
        public MapName MapName { get; private set; }
        public MaxPlayers MaxPlayers { get; private set; }
        public Name Name { get; private set; }
        public Players Players { get; private set; }
        public Pwads PWads { get; private set; }
        public Skill Skill { get; private set; }
        public Teams Teams { get; private set; }

        public Rcon Rcon { get; private set; }
    }

}
