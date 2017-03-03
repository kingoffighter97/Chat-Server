using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ChatInfo;

namespace MsgClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SendMessage : Window
    {
        public SendMessage()
        {
            InitializeComponent();
        }

        private void txtbMsg_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Disables send button if no message
            if (txtbMsg.Text == "")
            {
                btnSend.IsEnabled = false;
            }
            else
            {
                btnSend.IsEnabled = true;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuiChatRoomJoin_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomJoin wnd = new ChatRoomJoin();
            wnd.ShowDialog();
        }

        private void menuiServerConnect_Click(object sender, RoutedEventArgs e)
        {
            ServerConnect wnd = new ServerConnect();
            wnd.ShowDialog();
        }

        private void menuiChatRoomCreate_Click(object sender, RoutedEventArgs e)
        {
            if (Client.CreateLobby() == false)
            {
                MessageBox.Show("Error creating room", "Create Room Error");
            }
        }
    }
}
