/*
    * File       :   ServerConnect.xaml
    * Assignment :   Win Prog 4/5
    * Coder      :   Jason Gemanaru & Bobby Vu
    * Date       :   Nov 20, 2016
    * Description:   Contains C# code for the ServerConnect window
*/
using System;
using System.Windows;
using System.Windows.Input;
using System.Messaging;
using SupportClass;

namespace MsgClient
{
    /// <summary>
    /// Interaction logic for ServerConnect.xaml
    /// </summary>
    public partial class ServerConnect : Window
    {
        /*
        * FUNCTION : ServerConnect
        * 
        * DESCRIPTION : Constructs a Lobby window object
        *
        * PARAMETERS : NONE
        *
        * RETURNS : NONE
        */
        public ServerConnect()
        {
            InitializeComponent();

            //Sets default server name to this machine
            txtbServerName.Text = Environment.MachineName;
        }

        /*
        * FUNCTION : btnConnect_Click
        * 
        * DESCRIPTION : Connect button click event
        *
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //Connects to server
            App.myQueue_toServer = new MessageQueue("FormatName:DIRECT=OS:" + txtbServerName.Text + @"\Private$\Listener");
            if (MessageQueue.Exists(@".\Private$\" + txtbUserName.Text))
            {
                lblErrorMessage.Content = "Username already exists";
                return;
            }
            App.myQueue_fromServer = MyMessage.createMessageQueue(txtbUserName.Text);
            //Creates message
            MyMessage msg = new MyMessage(Actions.CLIENT_REG, txtbUserName.Text, Environment.MachineName);

            //Sends message to server
            try
            {
                App.myQueue_toServer.Send(msg);
            }
            catch (MessageQueueException ex)
            {
                lblErrorMessage.Content = "Server does not exist";
                return;
            }

            
            try
            {
                //Reads my message queue for response
                msg = (MyMessage)(App.myQueue_fromServer.Receive(new TimeSpan(0, 0, 2)).Body);

                if (msg.Action == Actions.SERVER_REG_SUCCESS)
                {
                    //Launch lobby window

                    App.ChatName = txtbUserName.Text;
                    App.ServerName = txtbServerName.Text;

                    Lobby lobby = new Lobby();
                    lobby.Show();

                    //Close this lobby
                    this.Close();
                }
                else if (msg.Action == Actions.SERVER_REG_FAIL)
                {
                    //Sets error message
                    lblErrorMessage.Content = "Username already exists";
                }
            }
            catch (MessageQueueException ex)
            {
                Console.WriteLine(ex);
                lblErrorMessage.Content = "Server is offline";
                MessageQueue.Delete(@".\Private$\" + txtbUserName.Text);
                return;
            }
        }

        /*
        * FUNCTION : txtbUserName_PreviewKeyDown
        * 
        * DESCRIPTION : Username text box key down event
        *
        * PARAMETERS : object sender, KeyEventArgs e
        *
        * RETURNS : NONE
        */
        private void txtbUserName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }
    }

}
