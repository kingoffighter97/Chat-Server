/*
* CLASS NAME: Constants
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu & Jason Gemanaru
* FIRST VERSION : 2016-11-18
* DESCRIPTION:
* This is the class that contains necessary contants
*/

namespace SupportClass
{
    public enum Actions
    {
        CLIENT_REG = 100,
        CLIENT_CONNECT = 200,
        CLIENT_CONNECT_SUCCESS = 201,
        CLIENT_CONNECT_FAILURE = 202,
        CLIENT_DISCONNECT = 300,
        CLIENT_QUIT = 400,        
        CLIENT_CHAT = 500,

        SERVER_REG_FAIL = 1000,
        SERVER_REG_SUCCESS = 1001,
        SERVER_ONLINE_LIST = 5000,
        SERVER_SHUTDOWN = 1,

        THIS_QUIT = 600
    }
}
