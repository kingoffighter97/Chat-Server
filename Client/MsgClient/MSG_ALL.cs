using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Text.RegularExpressions;

//----------------------- CLIENT -----------------------------

namespace MSG_ALL
{

    class MSG_ALL_INFO
    {
        //Const --------------------------------------------------------------
        public const string SERVER_INC_NAME = "MSG_REQUEST_PIPE_";

        const string MESSAGE_DELIMITER = "|";

        //Regex
        public const string MESSAGE_COMPNAME_FORMAT = "^[^\\/:*?\" <>|.][^\\/:*?\"<>|]*$";      //Regex for Windows Computer Name format
        public const string MESSAGE_PIPENAME_FORMAT = "^[a-zA-Z0-9]+$";                         //Regex for my pipe/user name format
        public const string MESSAGE_COMMAND_FORMAT = "^[a-zA-Z]*$";                             //Message command format 

        //Commands
        public const string MESSAGE_COMMAND_MSG = "MSG";

        //Static --------------------------------------------------------------


        
        public static bool CreateMessage(out string msg, string userName, string computerName, string command, string param)
        {
            Regex regComp = new Regex(MSG_ALL_INFO.MESSAGE_COMPNAME_FORMAT);
            Regex regPipe = new Regex(MSG_ALL_INFO.MESSAGE_PIPENAME_FORMAT);

            //If none of the parameters are blank
            if ((userName.Length != 0 && computerName.Length != 0 && command.Length != 0 && param.Length != 0) &&
                (regComp.IsMatch(computerName) && regPipe.IsMatch(userName)))
            {
                msg = userName + MESSAGE_DELIMITER
                    + computerName + MESSAGE_DELIMITER
                    + command + MESSAGE_DELIMITER
                    + param;

                return true;
            }

            //If there is an error
            msg = "";
            return false;
        }
    }

    class MSG_ALL_INFO_C
    {
        //CONST ------------------------------------------------


        //Static -----------------------------------------------

        //Data members
        public static string ServerComp = "";

        public static NamedPipeServerStream pServerW = null;        //This writes to this stream
        public static NamedPipeServerStream pServerR = null;        //This reads from this stream

        //Methods

        //Takes the username, and formats it into a string that can be parsed by the server
        public static bool FormatConnectRequest(string userName, out string formattedMsg)
        {
            Regex regComp = new Regex(MSG_ALL_INFO.MESSAGE_COMPNAME_FORMAT);
            Regex regPipe = new Regex(MSG_ALL_INFO.MESSAGE_PIPENAME_FORMAT);

            //If both strings don't contain illegal characters
            if (regComp.IsMatch(Environment.MachineName) && regPipe.IsMatch(userName))
            {
                //Combines computer name with pipe name with a colon delimiter
                formattedMsg = Environment.MachineName + ":" + userName;
                return true;
            }

            formattedMsg = "";
            return false;
        }

        public static bool CreateUniqueServerPipes(string userName)
        {
            try
            {
                //Creates the pipes that will be used for reading and writing
                pServerW = new NamedPipeServerStream(userName + "_W", PipeDirection.Out);
                pServerR = new NamedPipeServerStream(userName + "_R", PipeDirection.In);

                return true;
            }
            catch (Exception ex)
            {
                //If there is an error creating the pipes
                Console.WriteLine(ex);
                ClearPipes();
            }
            
            return false;
        }

        public static void ClearPipes()
        {
            pServerR.Close();
            pServerW.Close();
            pServerR = null;
            pServerW = null;
        } 

    }
}
