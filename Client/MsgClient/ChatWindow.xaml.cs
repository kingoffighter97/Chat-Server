/*
    * File       :   ChatWindow.xaml.cs
    * Assignment :   Win Prog 4/5 
    * Coder      :   Jason Gemanaru & Bobby Vu
    * Date       :   Nov 20, 2016
    * Description:   Contains C# code for the ChatWindow Window.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SupportClass;
namespace MsgClient
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        string chatWith = "";
        public ChatWindow(string chattingWith)
        {
            //Sets chat window info
            InitializeComponent();
            chatWith = chattingWith;
            lblChatters.Content = chattingWith;
            this.Name = chattingWith;
            this.Title = App.ChatName + " - Chat Window - with " + chattingWith;
        }

        /*
        * FUNCTION : btnSend_Click
        * 
        * DESCRIPTION : Click event for send button
        *
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Send message
            MyMessage m = new MyMessage(Actions.CLIENT_CHAT, App.ChatName, Environment.MachineName, lblChatters.Content.ToString(), txtbSendBox.Text.ToString());
            App.myQueue_toServer.Send(m);

            string temp = "";
            // Put that message in the message box
            if (txtbMessages.Text != "")
            {
                temp = "\n";
            }

            temp = temp + m.From + ": " + m.Content;
            txtbMessages.Text += temp;

            // Clear that text in the sending box
            txtbSendBox.Text = "";
        }

        /*
        * FUNCTION : Window_Closing
        * 
        * DESCRIPTION : Window close event
        *
        * PARAMETERS : object sender, System.ComponentModel.CancelEventArgs e
        *
        * RETURNS : NONE
        */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.chattingList.Remove(chatWith);         
        }

        /*
        * FUNCTION : txtbSendBox_TextChanged
        * 
        * DESCRIPTION : Text change event for send box
        *
        * PARAMETERS : object sender, TextChangedEventArgs e
        *
        * RETURNS : NONE
        */
        private void txtbSendBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Enables send button if the send box is not empty
            if (txtbSendBox.Text == "")
            {
                btnSend.IsEnabled = false;
            }
            else
            {
                btnSend.IsEnabled = true;
            }
        }

        /*
        * FUNCTION : txtbSendBox_KeyDown
        * 
        * DESCRIPTION : Key press event for send text box
        *
        * PARAMETERS : object sender, KeyEventArgs e
        *
        * RETURNS : NONE
        */
        private void txtbSendBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Allows users to send message by hitting enter
            if (e.Key == Key.Enter && btnSend.IsEnabled == true)
            {
                btnSend_Click(sender, e);
            }
        }

        /*
        * FUNCTION : txtbMessages_TextChanged
        * 
        * DESCRIPTION : Text change event for chat message box
        *
        * PARAMETERS : object sender, TextChangedEventArgs e
        *
        * RETURNS : NONE
        */
        private void txtbMessages_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Scrolls to the end of the chat box automatically
            txtbMessages.ScrollToEnd();
        }
    }
}
