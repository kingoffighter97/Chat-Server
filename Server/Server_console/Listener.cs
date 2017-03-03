/*
* CLASS NAME: Listener
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu
* FIRST VERSION : 2016-11-18
* DESCRIPTION:
* This is the Listener of the server
* It reads the message from the clients and send it to the processors' message queue
*/

using System.Threading;
using System.Messaging;
using SupportClass;
namespace Server_console
{
    class Listener
    {

        /*
        * FUNCTION : startListener
        * 
        * DESCRIPTION : This function spawns a thread for a Listener
        *
        * PARAMETERS : List<Client> clientList: list of client
        *
        * RETURNS : NONE
        */
        public void startListener()
        {
            Thread listenerThread = new Thread(() => Listen());
            listenerThread.Start();
        }


        /*
        * FUNCTION : startListener
        * 
        * DESCRIPTION : This method receives messages from the client and sends them to the processor's message queue
        *
        * PARAMETERS : NONE
        *
        * RETURNS : NONE
        */
        private void Listen()
        {
            MessageQueue msgQueue_Listener = MyMessage.createMessageQueue("Listener");
            MessageQueue msgQueue_Processor = MyMessage.createMessageQueue("Processor");
            bool done = false;
            while (!done)
            {
                // Receive message from client
                MyMessage message = (MyMessage)(msgQueue_Listener.Receive().Body);
                msgQueue_Processor.Send(message);

                // Delete the message queue and exit the loop if the server is shutting down
                if (message.Action == Actions.SERVER_SHUTDOWN)
                {                    
                    MessageQueue.Delete(@".\Private$\Listener");
                    done = true;                  
                }         
                
            }
        }
    }
}
