/*
    * File       :   ChatWindow.xaml.cs
    * Assignment :   Win Prog 4/5 
    * Coder      :   Jason Gemanaru & Bobby Vu
    * Date       :   Nov 20, 2016
    * Description:   Contains C# code for the ChatWindow Window. 
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Messaging;
using SupportClass;

namespace MsgClient
{
    /// <summary>
    /// Interaction logic for Lobby.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        bool clientQuit = true;

        /*
        * FUNCTION : Lobby
        * 
        * DESCRIPTION : Constructs a Lobby window object
        *
        * PARAMETERS : NONE
        *
        * RETURNS : NONE
        */
        public Lobby()
        {
            InitializeComponent();

            App.chattingList = new List<string>(); // Chatting list 

            // Start a thread to listen to the messages
            Thread thread = new Thread(ProcessMessages);
            thread.Start();

            // Set label to display server and chat name
            lblServerName.Content = "Server: " + App.ServerName;
            lblChatName.Content = "Chat Name: "+ App.ChatName;

            // Set title
            this.Title = App.ChatName + " - Lobby";

            // Refresh the user online list
            MyMessage m = new MyMessage(Actions.SERVER_ONLINE_LIST, App.ChatName, Environment.MachineName);
            App.myQueue_toServer.Send(m);

        }


        /*
        * FUNCTION : ProcessMessages
        * 
        * DESCRIPTION : This function processes messages decide what to do accordingly
        *
        * PARAMETERS : object sender, MouseButtonEventArgs e
        *
        * RETURNS : NONE
        */
        private void ProcessMessages()
        {
            bool done = false;
            while (!done)
            {
                // Read incoming messages
                MyMessage msg = (MyMessage)(App.myQueue_fromServer.Receive().Body);

                //Execute messages
                switch (msg.Action)
                {
                    case Actions.SERVER_ONLINE_LIST:
                        {
                            // If the server is sending back online list
                            this.Dispatcher.Invoke(() =>
                            {
                                // Update the online list
                                lbOnlineUsers.Items.Clear();

                                foreach (string value in msg.clientList)
                                {
                                    lbOnlineUsers.Items.Add(value);
                                }
                            }
                            );

                            break;
                        }

                    case Actions.CLIENT_CONNECT_SUCCESS:
                        {
                            // If sucessfully connected to another client
                            // spawn a chat window with that client
                            spawnChatWindow(msg.From);
                            break;
                        }

                    case Actions.CLIENT_CONNECT_FAILURE:
                        {
                            // If failed to connect to another client
                            // spawn a message box to inform the user
                            MessageBox.Show("Sorry, " + msg.From + " has disconnected");

                            // Update the online list
                            MyMessage m = new MyMessage(Actions.SERVER_ONLINE_LIST, App.ChatName, Environment.MachineName);
                            App.myQueue_toServer.Send(msg);
                            break;
                        }


                    case Actions.CLIENT_CHAT:
                        {
                            // If receives a message from another client
                            // Find if there's a chat window for the sender yet
                            if (!App.chattingList.Contains(msg.From))
                            {
                                // If no, spawn a chat window
                                spawnChatWindow(msg.From);
                            }

                            // Write message to msg box
                            writeMessageToMsgBox(msg);
                            break;
                        }

                    case Actions.CLIENT_QUIT:
                        {
                            // If a client quitted
                            if (App.chattingList.Contains(msg.From))
                            {
                                // If you're chatting with that client

                                // Inform yourself in the message box
                                App.chattingList.Remove(msg.From);
                                writeMessageToMsgBox(msg);
                                // Disable the sending text box
                                this.Dispatcher.Invoke(() =>
                                {
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.Name == msg.From)
                                        {
                                            ((ChatWindow)window).txtbSendBox.IsEnabled = false;
                                            break;
                                        }
                                    }
                                });
                            }
                            // Request to update online list
                            MyMessage m = new MyMessage(Actions.SERVER_ONLINE_LIST, App.ChatName, Environment.MachineName);
                            App.myQueue_toServer.Send(m);
                            break;
                        }


                    case Actions.SERVER_SHUTDOWN:
                        {
                            // If the server is shutting down
                            clientQuit = false; // update the boolean variable indicating that the server quitted
                            MessageBox.Show("Server has disconnected", "Server Shutdown", MessageBoxButton.OK);
                            // Do clean up
                            cleanUp();
                            break;
                        }

                    case Actions.THIS_QUIT:
                        {
                            // If this client is quitting, exit the loop
                            done = true;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            // Delete message queue
            if (MessageQueue.Exists(@".\Private$\" + App.ChatName))
            {
                MessageQueue.Delete(@".\Private$\" + App.ChatName);
            }

            // Shut down the whole application
            this.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            }
            );

        }




        /*
        * FUNCTION : lbOnlineUsers_MouseDoubleClick
        * 
        * DESCRIPTION : This function is triggered when the user double click in one item in the listbox
        *               It then connects to that according client to start a chatting session
        *
        * PARAMETERS : object sender, MouseButtonEventArgs e
        *
        * RETURNS : NONE
        */
        private void lbOnlineUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //If the user has selected an item
            if (lbOnlineUsers.SelectedItem != null)
            {
                //Requests to chat with a client
                MyMessage msg = new MyMessage(Actions.CLIENT_CONNECT, App.ChatName, Environment.MachineName, lbOnlineUsers.SelectedValue.ToString());
                //Launch chat window with user
                App.myQueue_toServer.Send(msg);                               
                
            }
        }







        /*
        * FUNCTION : writeMessageToMsgBox
        * 
        * DESCRIPTION : This function writes received message to the message box in the chat window
        *               
        * PARAMETERS : MyMessage msg: received message
        *
        * RETURNS : NONE
        */
        private void writeMessageToMsgBox(MyMessage msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Name == msg.From)
                    {
                        string temp = "";
                        if (((ChatWindow)window).txtbMessages.Text != "")
                        {
                            temp = "\n";
                        }


                        if (msg.Action == Actions.CLIENT_CHAT)
                        {
                            // Normal chatting  
                            temp = temp + msg.From + ": " + msg.Content;
                        }
                        else if (msg.Action == Actions.CLIENT_QUIT)
                        {
                            // The other end has disconnected, inform the user
                            temp = temp + msg.From + " has disconnected...";
                        }
                        ((ChatWindow)window).txtbMessages.Text += temp;
                        break;
                    }
                }
            });
            
        }





        /*
        * FUNCTION : spawnChatWindow
        * 
        * DESCRIPTION : This function spawns a chat window with another client
        *               
        * PARAMETERS : NONE
        *
        * RETURNS : NONE
        */
        private void spawnChatWindow(string withWho)
        {
            this.Dispatcher.Invoke(() =>
            {
                ChatWindow wnd = new ChatWindow(withWho);
                App.chattingList.Add(withWho);
                wnd.Show();
            }
            );
            
        }






        /*
        * FUNCTION : btnRefreshOnlineList_Click
        * 
        * DESCRIPTION : This function sends request to update online list
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btnRefreshOnlineList_Click(object sender, RoutedEventArgs e)
        {
            //Sends refresh request
            MyMessage msg = new MyMessage(Actions.SERVER_ONLINE_LIST, App.ChatName, Environment.MachineName);
            App.myQueue_toServer.Send(msg);
        }



        /*
       * FUNCTION : Window_Closing
       * 
       * DESCRIPTION : This function decides how to clean up when the window is closing
       *               
       * PARAMETERS : object sender, System.ComponentModel.CancelEventArgs e
       *
       * RETURNS : NONE
       */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (clientQuit == true)
            {
                // If it's the client quitting, send message to inform the server
                MyMessage m = new MyMessage(Actions.CLIENT_QUIT, App.ChatName);
                App.myQueue_toServer.Send(m);
            }

            // Send message to itself to quit the listening thread and delete the message queue
            cleanUp();  
        }




        /*
       * FUNCTION : cleanUp
       * 
       * DESCRIPTION : This function sends message to this client's message queue informing that it's shutting down
       *               
       * PARAMETERS : NONE
       *
       * RETURNS : NONE
       */
        private void cleanUp()
        {
            MyMessage m = new MyMessage(Actions.THIS_QUIT);
            if (MessageQueue.Exists(@".\Private$\" + App.ChatName))
            {
                App.myQueue_fromServer.Send(m);
            }
        }
    }
}
