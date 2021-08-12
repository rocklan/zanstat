Zandronum Server Query App
------------------------------------------------------------

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

`-server hostname:port` is required!

Notes:
   * If `-rcon` is supplied then no command is needed. It will go into interactive mode where you can pass through any command you wish.
   * `-getteams` doesn't currently work! My server returns no data for this command.
   * For commands, you may only query the server every `sv_queryignoretime` seconds. If you query too frequently, the server will return an error code and the library will throw an exception.
