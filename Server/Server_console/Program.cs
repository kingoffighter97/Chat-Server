/*
* FILE : Program.cs
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu
* FIRST VERSION : 2016-11-18
* DESCRIPTION :
* The chat server
* It has a thread for each of its Listener and Processors
*/


using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Messaging;
using SupportClass;
namespace Server_console
{
    class Program
    {
        private static bool isclosing = false; // boolean variable to keep the main thread from shutting down
        public static void Main()
        {

            // Turn on event handler to check if he program is off
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            List<Client> clientList = new List<Client>(); // List of clients

            Console.WriteLine("SERVER STARTED AT: " + Environment.MachineName);

            // Spawn a thread for the Listener
            Listener listener = new Listener();
            listener.startListener();
            Console.WriteLine("LISTENER STARTED");

            // Create 4 instances of Processor
            List<Processor> processor = new List<Processor>();
            for (int i = 0; i < 4; i++)
            {
                Processor p = new Processor();
                processor.Add(p);
            }

            int j = 1;
            // Spawn a thread to run each of them and pass the client list as well
            foreach (Processor p in processor)
            {
                p.startProcessor(clientList);
                Console.WriteLine("PROCESSOR NO. " + j.ToString() + " STARTED");
                j++;
                // Delay so there won't be error in creating message queue
                Thread.Sleep(500);
            }

            while (!isclosing) ; // Avoid the main thread from shutting down
        }




        /*
        * FUNCTION : ConsoleCtrlCheck
        * 
        * DESCRIPTION : This function do the cleaning when there's a signal informing the server is shutting down
        *
        * PARAMETERS : CtrlTypes ctrlType: type of signal
        *
        * RETURNS : bool true
        */
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            
            if (ctrlType == CtrlTypes.CTRL_C_EVENT || ctrlType == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                // If the user hit "Ctrl + C" or the 'x' button
                // Send message to itself to clean up message queue
                MyMessage m = new MyMessage();
                m.Action = Actions.SERVER_SHUTDOWN;
                MessageQueue toListener = new MessageQueue(@".\Private$\Listener");          
                toListener.Send(m);

                // Sleep 0.5s so the program won't close too fast (gives the Processor time to delete the message queue)
                Thread.Sleep(500);
                isclosing = true;
            }

            return true;
        }



        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}
