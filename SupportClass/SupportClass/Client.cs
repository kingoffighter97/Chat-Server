/*
* CLASS NAME: Client
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu
* FIRST VERSION : 2016-11-18
* DESCRIPTION:
* This is the Client class that keeps track of all the clients on the server
*/


using System.Collections.Generic;
namespace SupportClass
{
    public class Client
    {
        public string MachineName { get; set; }             // Machine Name
        public string ClientName { get; set; }              // Client Name
        public List<Client> ConnectedTo { get; set; }       // List of clients that the current client is chatting with
        
    }
}
