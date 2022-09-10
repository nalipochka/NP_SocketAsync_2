using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace NP_SocketAsync_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            quotes.Add("Одно я поняла об англичанах: небольшая победа делает из высокомерными, а малейшая неудача ввергает в уныние.");
            quotes.Add("Городские легенды, уличные сплетни, вторичная информация. Если это рассказывать, то больше похоже на болтовню.");
            quotes.Add("Мы с тобой одной крови, мы с тобой одной породы, Нам не привыкать к боли, если имя ей «свобода».");
            quotes.Add("«Отложим» — самая ужасная форма отказа.");
            quotes.Add("Жизнь может сбить нас с ног, но только мы выбираем подниматься нам или нет.");
            quotes.Add("Все мы люди, все человеки. По себе знаю.");
            quotes.Add("Мне кажется, мир нуждается в кинематографе так же, как тяжелораненый нуждается в обезболивающем.");
            quotes.Add("... я люблю тебя и хочу, чтобы ты был достоин самого себя.");
            quotes.Add("Я буду жить своей жизнью.");
            quotes.Add("Денни, в конце концов, все мы люди... И я, и ты.");
            quotes.Add("Дурак — человек, который живет нерефлексивно и реактивно.");
            quotes.Add("Все животные — стайные. У одиночек — стая внутри.");
            quotes.Add("... Чем больше истины вы раскроете, тем больше умений откроете.");
        }

        List<string> quotes = new List<string>();

        delegate void TextDelegate(string str);

        IPAddress ip;
        IPEndPoint endP;
        Socket socket;

        private void UpdateTextBox1(string str)
        {
            StringBuilder sb = new StringBuilder(startServer_txtBox.Text);
            sb.AppendLine(str);
            startServer_txtBox.Text = sb.ToString();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            startServer_Btn.IsEnabled = false;
            Task.Run(StartServer_Main);
        }
        
        private async void StartServer_Main()
        {
            ip = IPAddress.Parse("127.0.0.1");
            endP = new IPEndPoint(ip, 1024);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
             
            try
            {
                socket.Bind(endP);
                socket.Listen();

                await startServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), "Server started!");
                while (true)
                {
                    Socket newS = await socket.AcceptAsync();

                    await Task.Run(() => StartServer_Child(newS));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        private async void StartServer_Child(Socket newS)
        {
            LogClient logClient = new LogClient() { ClientIp = newS.RemoteEndPoint.ToString(), ConnectingTime = DateTime.Now };
            List<string> sentQuotes = new List<string>();
            try
            {
                Random rnd = new Random();
                await startServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), logClient.ClientIpToString());
                await startServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), logClient.ConnectingTimeToString());
                byte[] buffer = new byte[1024];

                for (int i = 0; i < 5; i++)
                {
                    string quote = quotes[rnd.Next(0, quotes.Count())];
                    buffer = Encoding.Default.GetBytes(quote);
                    await newS.SendAsync(buffer, SocketFlags.None);
                    sentQuotes.Add(quote);
                    Thread.Sleep(500);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "SocketError!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                logClient.Quotes = sentQuotes;
                logClient.DisconnectingTime = DateTime.Now;
                await startServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), logClient.QuotesToString());
                await startServer_txtBox.Dispatcher.BeginInvoke(new TextDelegate(UpdateTextBox1), logClient.DisconnectingTimeToString());
                newS.Shutdown(SocketShutdown.Both);
                newS.Close();
            }
        }
    }
}
