namespace Rocklan.Zanstat
{
    public enum RconServerResponseEnum
    {
        SVRC_OLDPROTOCOL = 32, //This set of enumerations starts at 32, increments by one
        SVRC_BANNED,
        SVRC_SALT,
        SVRC_LOGGEDIN,
        SVRC_INVALIDPASSWORD,
        SVRC_MESSAGE,
        SVRC_UPDATE,
        SVRC_TABCOMPLETE,
        SVRC_TOOMANYTABCOMPLETES,
    };
}
