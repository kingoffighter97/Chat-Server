/*
* CLASS NAME: Processor
* PROJECT : Windows and Mobile Programming Assignment #4
* PROGRAMMER : Bobby Vu
* FIRST VERSION : 2016-11-18
* DESCRIPTION:
* This is the Processor of the server
* It reads the message from the Listener, process it and send back appropriate signals to the client(s)
*/

using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using SupportClass;

namespace Server_console
{
    class Processor
    {

        /*
        * FUNCTION : startProcessor
        * 
        * DESCRIPTION : This function spawns a thread for a Processor
        *
        * PARAMETERS : List<Client> clientList: list of client
        *
        * RETURNS : NONE
        */
        public void startProcessor(List<Client> clientList)
        {
            Thread processorThread = new Thread(() => Process(clientList));
            processorThread.Start();
        }




        /*
        * FUNCTION : Find
        * 
        * DESCRIPTION : This function finds a client by its name
        *
        * PARAMETERS : List<Client> clientList: list of client
        *              string Name : name of the client that needs finding
        *
        * RETURNS : Client: the found client (every properties of this client will be null the client was not found)
        */
        private static Client Find(List<Client> clientList, string Name)
        {
            Client target = new Client();
            foreach (Client c in clientList)
            {
                if (c.ClientName == Name)
                {
                    target = c;
                    break;                    
                }
            }
            return target;
        }




        /*
        * FUNCTION : ProcessMessage
        * 
        * DESCRIPTION : This function processes messages and decide what to do after that (update client list, send back message to client)
        *
        * PARAMETERS : List<Client> clientList: list of client
        *              MyMessage message: received message
        *
        * RETURNS : NONE
        */
        private static void ProcessMessage(MyMessage message, List<Client> clientList)
        {                    
            switch (message.Action)
            {
                
                case Actions.CLIENT_REG:
                    {
                        // A client connects to the server
                        // Check if the client exists already                        
                        Client from = Find(clientList, message.From);

                        // Determines the destination of the message
                        MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + message.MachineName + "\\Private$\\" + message.From);
                        if (from.ClientName != null)
                        {
                            // If the name already exists, notify the client                            
                            MyMessage m = new MyMessage(Actions.SERVER_REG_FAIL);
                            toClient.Send(m);
                        }
                        else
                        {
                            // If not, add client to the list 
                            Client c = new Client();
                            c.ClientName = message.From;
                            c.MachineName = message.MachineName;
                            c.ConnectedTo = new List<Client>();
                            clientList.Add(c);

                            // Notify the client
                            MyMessage m = new MyMessage(Actions.SERVER_REG_SUCCESS);                                  
                            toClient.Send(m);

                            foreach (Client l in clientList)
                            {
                                // Renew all clients' online list
                                toClient = new MessageQueue(@".\Private$\Processor");
                                m = new MyMessage(Actions.SERVER_ONLINE_LIST, l.ClientName, l.MachineName);
                                toClient.Send(m);
                            }
                        }

                        break;

                    }

                case Actions.SERVER_ONLINE_LIST:
                    {
                        // If the client requests the list of currently online clients

                        // Determine the destination of the message
                        MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + message.MachineName + "\\Private$\\" + message.From);
                        // Load the client list into the message
                        MyMessage m = new MyMessage(Actions.SERVER_ONLINE_LIST);
                        m.clientList = new List<string>();
                        foreach (Client c in clientList)
                        {
                            if (c.ClientName != message.From)
                            {
                                m.clientList.Add(c.ClientName);
                            }
                        }
                        toClient.Send(m); // Send the message
                        break;
                    }




                case Actions.CLIENT_CONNECT:
                    {   
                        // If a client requests to chat to another client           
                        Client from = Find(clientList, message.From);
                        Client to = Find(clientList, message.To);

                        // Determine the destination of the message
                        MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + message.MachineName + "\\Private$\\" + message.From);
                        MyMessage m = new MyMessage(message.To, message.From);
                        // Check if the receiving client exists, send back according signal
                        if (to.ClientName == null)
                        {
                            m.Action = Actions.CLIENT_CONNECT_FAILURE;  
                        }
                        else
                        {
                            // Add the receiving client to the sending client's chatting list
                            m.Action = Actions.CLIENT_CONNECT_SUCCESS;
                            from.ConnectedTo.Add(to);
                        }
                        toClient.Send(m);

                        
                        break;
                    }
                    

                case Actions.CLIENT_CHAT:
                    {
                        // If the client sends a message to another client

                        // Find them
                        Client from = Find(clientList, message.From);
                        Client to = Find(clientList, message.To);
                        // Determine the destination of the message and send it
                        MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + to.MachineName + "\\Private$\\" + to.ClientName);
                        toClient.Send(message);

                        // Add the sending client to the receiving client's chatting list if he's not there yet
                        if (!to.ConnectedTo.Contains(from))
                        {
                            to.ConnectedTo.Add(from);
                        }
                        
                        break;
                    }

                case Actions.CLIENT_QUIT:
                    {
                        // If a client disconnects
                        // Find the client
                        Client target = Find(clientList, message.From);
                        
                        // Delete that client from everyone's chatting list
                        foreach(Client c in clientList)
                        {
                            foreach(Client connected in c.ConnectedTo)
                            {
                                if (connected.ClientName == target.ClientName)
                                {                 
                                    // Inform the clients it's chatting with that it quitted
                                    MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + c.MachineName + "\\Private$\\" + c.ClientName);
                                    toClient.Send(message);

                                    // Delete the client if it's connected to any other client
                                    c.ConnectedTo.Remove(connected);
                                    break;
                                }
                            }
                        }
                        
                        // Delete that client from the list
                        clientList.Remove(target);
                        break;
                    }

                case Actions.SERVER_SHUTDOWN:
                    {
                        // If the server shuts down
                        MyMessage m = new MyMessage(Actions.SERVER_SHUTDOWN);
                        foreach (Client c in clientList)
                        {
                            // Send message to all clients informing them that the server is down
                            MessageQueue toClient = new MessageQueue("FormatName:DIRECT=OS:" + c.MachineName + "\\Private$\\" + c.ClientName);
                            toClient.Send(m);
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        /*
        * FUNCTION : Process
        * 
        * DESCRIPTION : This function receives message from the listener and call the ProcessMessage function to process the message
        *
        * PARAMETERS : List<Client> clientList: list of client
        *              MyMessage message: received message
        *
        * RETURNS : NONE
        */
        private static void Process(List<Client> clientList)
        {
            
            bool done = false;
            // Create message queue between Listener and Processor
            MessageQueue msgQueue_Processor = MyMessage.createMessageQueue("Processor");

            while (!done)
            {                
                // Receive message from listener
                MyMessage message = (MyMessage)(msgQueue_Processor.Receive().Body);
                Console.WriteLine("PROCESSOR - Received message: " + message.From + ", " + message.Action + ", " + message.To); // logging             

                // Process message
                ProcessMessage(message, clientList);

                // Delete the message queue and exit the loop if the server is shutting down
                if (message.Action == Actions.SERVER_SHUTDOWN)
                {
                    MessageQueue.Delete(@".\Private$\Processor");                    
                    done = true;
                }           
            }
        }

    }
}
