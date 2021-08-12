using System;
using System.Text;

namespace Zanlib
{
    internal static class MessageHelpers
    {
        private const int LAUNCHER_CHALLENGE = 199;

        /// <summary>
        /// Parses a null terminated ASCII string from the input byte data and returns the remaining byte data after the string, as well as the string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringFromMessage(ref byte[] input)
        {
            var nullTerminatorIndex = input.Length;
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == 0)
                {
                    nullTerminatorIndex = i;
                    break;
                }
            }

            string str = Encoding.ASCII.GetString(input[0..nullTerminatorIndex]);

            input = input[(nullTerminatorIndex + 1)..];
            return str;
        }

        /// <summary>
        /// Converts the four bytes at the start of `input` to an int value and returns the remaining byte data after the int value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int GetIntFromMessage(ref byte[] input)
        {
            int val = BitConverter.ToInt32(input[0..4]);
            input = input[4..];
            return val;
        }

        /// <summary>
        /// Converts the two bytes at the start of `input` to a short value and returns the remaining byte data after the short value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static short GetShortFromMessage(ref byte[] input)
        {
            short val = BitConverter.ToInt16(input[0..2]);
            input = input[2..];
            return val;
        }

        /// <summary>
        /// Converts the two bytes at the start of `input` to a byte value and returns the remaining byte data after the byte value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte GetByteFromMessage(ref byte[] input)
        {
            byte val = input[0];
            input = input[1..];
            return val;
        }

        /// <summary>
        /// Returns a message of type `messageType` for sending to the server.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static byte[] GetLauncherMessage(int messageType)
        {
            var message = new int[3];
            message[0] = LAUNCHER_CHALLENGE;
            message[1] = messageType;
            message[2] = 0; // Time field - unused.

            var byteMessage = new byte[message.Length * sizeof(int)];
            Buffer.BlockCopy(message, 0, byteMessage, 0, byteMessage.Length);
            return byteMessage;
        }

        /// <summary>
        /// Returns an rcon "can we please connect" message
        /// </summary>
        public static byte[] GetRconConnectMessage()
        {
            var message = new byte[2];
            message[0] = (byte)RconClientRequestEnum.CLRC_BEGINCONNECTION;
            message[1] = 4;
            return message;
        }

        public static byte[] GetRconAuthenticateMessage(byte[] auth)
        {
            var message = new byte[auth.Length + 2];
            message[0] = (byte)RconClientRequestEnum.CLRC_PASSWORD;
            Buffer.BlockCopy(auth, 0, message, 1, auth.Length);
            message[33] = 0;
            return message;
        }

        public static byte[] GetRconKeepAliveMessage()
        {
            var message = new byte[1];
            message[0] = (byte)RconClientRequestEnum.CLRC_PONG;
            return message;
        }

        public static byte[] GetRconDisconnectMessage()
        {
            var message = new byte[1];
            message[0] = (byte)RconClientRequestEnum.CLRC_DISCONNECT;
            return message;
        }

        internal static byte[] GetCommandMessage(string line)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(line);

            var message = new byte[asciiBytes.Length + 1];
            message[0] = (byte)RconClientRequestEnum.CLRC_COMMAND;
            Buffer.BlockCopy(asciiBytes, 0, message, 1, asciiBytes.Length);
            return message;
        }
    }


}
