# Zandronum Server Querying

This repository provides a .NET nuget package and a command line console application for querying [Zandronum](https://zandronum.com) using the [Launcher Protocol](https://wiki.zandronum.com/Launcher_protocol) and also using the [RCon protocol](https://wiki.zandronum.com/RCon_protocol). 

## Console Application

Usage:

`zanstat <-server hostname:port> <-rcon RConServerPassword> [command]`

where `[command]` is one of:

```
   -getname       Print server name
   -getmapname    Print current map name
   -getmaxplayers Print max players  
   -getpwads      Print pwads in use  
   -getiwad       Print iwad in use  
   -getskill      Print skill setting  
   -getlimits     Print various server limits  
   -getplayers    Print players currently on the server  
   -getteams      Print teams  
```

Notes:
   * If `-server` is not supplied then it will default to `localhost` and port `10666`
   * If `-rcon` is supplied then no command is needed. It will go into interactive mode where you can pass through any command you wish.
   * `-getteams` doesn't currently work! My server returns no data for this command.
   * For commands, you may only query the server every `sv_queryignoretime` seconds. If you query too frequently, the server will return an error code and the library will throw an exception.

## Nuget Package

To use, instantiate the `Zandronum` class and pass through the server URL and port:

```csharp
Zanlib.Zandronum zandronum = new Zanlib.Zandronum("example.com", 10666);
```

Once instantiated you can then query the server for information like Server Name, Current Map, current Players, etc:

```csharp

var mapName = _zandronum.MapName.Get();
var players = zandronum.Players.Get();

Console.WriteLine($"Players current on map: {mapName}");

foreach (var player in players)
{
    Console.WriteLine($"  {player}");
}
```

You can also connect to the server using RCon and it will raise events whenever the server raises events:

```csharp
class MyCode
{
   function ConnectToZandronumServer()
   {
       zandronum.Rcon.ServerMessage += Rcon_ServerMessage;
       zandronum.Rcon.ConnectToRcon("ThisIsMyRconPassword");

       Console.WriteLine("Listening to server messages, press enter to quit...");
       Console.ReadLine();
   }

   private void Rcon_ServerMessage(object sender, EventArgs e)
   {
       string message = (e as ZandronumMessageEventArgs).Message;
       
       // Messages from the server have a newline appended, and they also 
       // have color coding embedded. You might want to filter out \\c- from the message
       
       Console.Write(message);
   }
}
```

If you'd like to send message to the server you can do so using the `SendCommand()` function:

```csharp
// return the list of maps
_zandronum.Rcon.SendCommand("ListMaps");
   
// change to map03:
_zandronum.Rcon.SendCommand("MAP MAP03");
```

## Dev Notes

Many, many thanks go to legend [Sam Izzo](https://github.com/samizzo/) for doing 99% of the work - writing the Huffman encoder, the UDP launcher code, and helping me get the RCon stuff working. 

Beware, this library is using UDP, not TCP! UDP is a strange, weird world, brace yourself.
