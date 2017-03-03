/*
    * File       :   App.xaml.cs
    * Coder      :   Jason Gemanaru
    * Date       :   Nov 20, 2016
    * Description:   Contains main App C#, which starts the program.
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Messaging;

namespace MsgClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MessageQueue myQueue_toServer;
        public static MessageQueue myQueue_fromServer;

        public static string ChatName;
        public static string ServerName;

        public static List<string> chattingList;        

        

    }
}
