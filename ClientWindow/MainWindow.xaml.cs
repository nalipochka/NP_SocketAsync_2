using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Threading;

namespace ClientWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            disconnectToServer_btn.IsEnabled = false;
        }

        delegate void TextDelegate(string str);
        delegate void BoolDelegate(bool b);

        IPAddress ip;
        IPEndPoint endP;
        Socket socket;

        CancellationTokenSource cts;
        CancellationToken token;
   
        private void connectToServer_Click(object sender, RoutedEventArgs e)
        {
            connectToServer_btn.IsEnabled = false;
            disconnectToServer_btn.IsEnabled = true;
            Task.Run(ConnectToServer);
            cts = new CancellationTokenSource();
            token = cts.Token;
        }
        private void disconnectToServer_Click(object sender, RoutedEventArgs e)
        {
            connectToServer_btn.IsEnabled = true;
            disconnectToServer_btn.IsEnabled = false;
            cts.Cancel();
        }
        private async void ConnectToServer()
        {
            ip = IPAddress.Parse("127.0.0.1");
            endP = new IPEndPoint(ip, 1024);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectAsync(endP);
                byte[] buffer = new byte[1024];
                int length = 0;
                //while(socket.Available <= 0)
                do
                {
                    if (token.IsCancellationRequested)
                        break;
                    else
                    {
                        length = await socket.ReceiveAsync(buffer, SocketFlags.None);
                        string message = Encoding.Default.GetString(buffer, 0, length);
                        await connectToServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), message);
                    }
                }while (length > 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await connectToServer_btn.Dispatcher.BeginInvoke(new BoolDelegate(UpdateButton1), true);
                await disconnectToServer_btn.Dispatcher.BeginInvoke(new BoolDelegate(UpdateButton2), false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        private void UpdateTextBox1(string str)
        {
            StringBuilder sb = new StringBuilder(connectToServer_txtBox.Text);
            sb.AppendLine(str);
            connectToServer_txtBox.Text = sb.ToString();
        }
        private void UpdateButton1(bool b)
        {
            connectToServer_btn.IsEnabled = b;
        }
        private void UpdateButton2(bool b)
        {
            disconnectToServer_btn.IsEnabled = b;
        }


    }
}
