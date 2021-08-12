namespace Rocklan.Zanstat
{
    public enum RconClientRequestEnum
    {
        CLRC_BEGINCONNECTION = 52, // Also increments by one
        CLRC_PASSWORD,
        CLRC_COMMAND,
        CLRC_PONG,
        CLRC_DISCONNECT,
        CLRC_TABCOMPLETE,
    };
}
