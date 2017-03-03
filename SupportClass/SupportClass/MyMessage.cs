/*
* CLASS NAME: MyMessage
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu
* FIRST VERSION : 2016-11-18
* DESCRIPTION:
* This is the Message class that is used through out the application to send and receive data
*/

using System;
using System.Collections.Generic;
using System.Messaging;
namespace SupportClass
{
    [Serializable]
    public class MyMessage
    {
        public string From { get; set; }                    // Client name
        public string MachineName { get; set; }             // Machine name
        public string To { get; set; }                      // Receiver's name
        public string Content { get; set; }                 // Content of the message

        public List<string> clientList { get; set; }        // Online client list that the server sends back to the client
        public SupportClass.Actions Action;                 // Action/Signal between server and client



        // Constructors
        public MyMessage() { }
        public MyMessage(SupportClass.Actions Action)
        {
            this.Action = Action;
        }

        public MyMessage(string From)
        {
            this.From = From;
        }

        public MyMessage(string From, string To)
        {
            this.From = From;
            this.To = To;
        }

        public MyMessage(SupportClass.Actions Action, string From)
        {
            this.Action = Action;
            this.From = From;
        }

        public MyMessage(SupportClass.Actions Action, string From, string MachineName)
        {
            this.Action = Action;
            this.From = From;
            this.MachineName = MachineName;
        }

        public MyMessage(SupportClass.Actions Action, string From, string MachineName, string To)
        {
            this.Action = Action;
            this.From = From;
            this.MachineName = MachineName;
            this.To = To;
        }

        public MyMessage(SupportClass.Actions Action, string From, string MachineName, string To, string Content)
        {
            this.Action = Action;
            this.From = From;
            this.MachineName = MachineName;
            this.To = To;
            this.Content = Content;
        }




        /*
        * FUNCTION : createMessageQueue
        * 
        * DESCRIPTION : This function creates a local message queue if it doesn't exist yet
        *
        * PARAMETERS : string name: name of the message queue
        *
        * RETURNS : MessageQueue: the created message queue
        */
        public static MessageQueue createMessageQueue(string name)
        {
            // Create message queue between Listener and Processor
            MessageQueue msgQueue = null;

            string pathName = @".\Private$\" + name;
            if (MessageQueue.Exists(pathName))
            {
                // If there is an existing message queue already
                msgQueue = new MessageQueue(pathName);
            }
            else
            {
                // If there is no message queue yet
                msgQueue = MessageQueue.Create(pathName);
            }
            msgQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(MyMessage) });
            return msgQueue;
        }
    }

    
}
