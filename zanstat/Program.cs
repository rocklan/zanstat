using System;
using Rocklan.Zanstat;

namespace Rocklan.Zanstat.Commandline
{
    class Program
    {
        static string hostname = "localhost";
        static int port = 10666;
        static string rconpassword = "";

        static int Main(string[] args)
        {
            try
            {
                return new Program().QueryZandronumServer(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private Zandronum _zandronum;


        public int QueryZandronumServer(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-runtests")
                    return RunTests();

                if (args[i] == "-server")
                {
                    if (!GetServerArg(i, args))
                        return 1;
                }

                if (args[i] == "-rcon")
                {
                    if (!GetRconArg(i, args))
                        return 1;
                }
            }

            Console.WriteLine($"Connecting to {hostname}:{port} ... ");

            _zandronum = new Zandronum(hostname, port);

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-getname")
                {
                    var serverName = _zandronum.Name.Get();
                    Console.WriteLine($"Server name: {serverName}");
                }
                else if (args[i] == "-getmapname")
                {
                    var mapName = _zandronum.MapName.Get();
                    Console.WriteLine($"Current map: {mapName}");
                }
                else if (args[i] == "-getmaxplayers")
                {
                    var maxPlayers = _zandronum.MaxPlayers.Get();
                    Console.WriteLine($"Max players: {maxPlayers}");
                }
                else if (args[i] == "-getpwads")
                {
                    var pwads = _zandronum.PWads.Get();
                    Console.WriteLine("PWADS:");
                    foreach (var pwad in pwads)
                        Console.WriteLine($"  {pwad}");
                }
                else if (args[i] == "-getiwad")
                {
                    var iwad = _zandronum.Iwad.Get();
                    Console.WriteLine($"IWAD: {iwad}");
                }
                else if (args[i] == "-getskill")
                {
                    var skill = _zandronum.Skill.Get();
                    Console.WriteLine($"Skill: {skill}");
                }
                else if (args[i] == "-getlimits")
                {
                    var limits = _zandronum.Limits.Get();
                    Console.WriteLine($"Limits: {limits}");
                }
                else if (args[i] == "-getplayers")
                {
                    var players = _zandronum.Players.Get();
                    Console.WriteLine("Players:");
                    foreach (var player in players)
                        Console.WriteLine($"  {player}");
                }
                else if (args[i] == "-getteams")
                {
                    var teams = _zandronum.Teams.Get();
                    Console.WriteLine("Teams:");
                    foreach (var team in teams)
                        Console.WriteLine($"  {team}");
                }
                else if (args[i] == "-rcon")
                {
                    StartRcon();
                }
            }

            return 0;
        }

        private void StartRcon()
        {
            _zandronum.Rcon.ServerMessage += Rcon_ServerMessage;
            _zandronum.Rcon.MapChange += Rcon_MapChange;
            _zandronum.Rcon.PlayerChange += Rcon_PlayerChange;

            _zandronum.Rcon.ConnectToRcon(rconpassword);

            
            Console.WriteLine("Please enter commands to pass to server:");
            string line = Console.ReadLine();

            while (line != "quit")
            {
                _zandronum.Rcon.SendCommand(line);
                line = Console.ReadLine();
            }

            _zandronum.Rcon.DisconnectFromRcon();

            Console.WriteLine("We should have disconnected. Press enter to quit...");
            Console.ReadLine();
        }

        private void Rcon_PlayerChange(object sender, EventArgs e)
        {
            var ea = e as ZandronumPlayerChangeEventArgs;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DateTime.Now + ": " + "Players have changed: ");
            foreach (var player in ea.Players)
            {
                Console.WriteLine("  " + player);
            }
        }

        private void Rcon_MapChange(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DateTime.Now + ": " + (e as ZandronumMapChangeEventArgs).MapName);
        }

        private void Rcon_ServerMessage(object sender, EventArgs e)
        {
            string message = (e as ZandronumMessageEventArgs).Message;

            int colorTag = message.IndexOf("\\c-");
            if (colorTag != -1)
            {
                string firstBit = message.Substring(0, colorTag);
                string endBit = message.Substring(colorTag + 3, message.Length - colorTag - 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(DateTime.Now + ": " + firstBit);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(endBit);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(DateTime.Now + ": " + message);
            }
            
        }

        private static int RunTests()
        {
            Tests.TestUncompressed();
            Tests.TestCompressed();
            Console.ReadKey();
            return 0;
        }

        private static bool GetServerArg(int i, string[] args)
        {
            if (i < args.Length)
            {
                var server = args[i + 1];
                var colon = server.IndexOf(':');
                if (colon >= 0)
                {
                    hostname = server.Substring(0, colon);
                    port = int.Parse(server.Substring(colon + 1));
                }
                else
                {
                    hostname = server;
                }
                return true;
            }
            else
            {
                Console.WriteLine("Must specify a server!");
                return false;
            }
        }


        private static bool GetRconArg(int i, string[] args)
        {
            if (i < args.Length)
            {
                rconpassword = args[i + 1];
                return true;
            }
            else
            {
                Console.WriteLine("Must specify an rcon password!");
                return false;
            }
        }
    }
}
