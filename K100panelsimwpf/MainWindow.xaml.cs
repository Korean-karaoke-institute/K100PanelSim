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
using System.IO.Ports;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;

namespace K100panelsimwpf
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        
        SerialPort serialPort1 = new SerialPort();
        currentPlaying cpw = new currentPlaying();
       
        public MainWindow()
        {
            InitializeComponent();
            
        }
        public void send_data(byte data, bool isBtn)
        {
            if (serialPort1.IsOpen)
            {
                byte[] send = new byte[3];
                if (isBtn)
                    send[0] = 0x7f;
                else
                    send[0] = 0x7e;
                send[1] = data;
                send[2] = getChecksum(new byte[] { send[0], send[1] });
                serialPort1.Write(send, 0, send.Length);
            }
        }
        public static byte getChecksum(byte[] data)
        {
            uint sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum = sum + data[i];
            }
            return Convert.ToByte(sum % 256);
        }
        private void btn_1_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            System.IO.Stream str = Properties.Resources.beep;
            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
            snd.Play();
            if (serialPort1.IsOpen)
            {
                Rectangle btn = (Rectangle)sender;
                switch (btn.Name.Split('_')[1])
                {
                    case "1":
                        send_data(btnSdata.btn_num1, true);
                        break;
                    case "2":
                        send_data(btnSdata.btn_num2, true);
                        break;
                    case "3":
                        send_data(btnSdata.btn_num3, true);
                        break;
                    case "4":
                        send_data(btnSdata.btn_num4, true);
                        break;
                    case "5":
                        send_data(btnSdata.btn_num5, true);
                        break;
                    case "6":
                        send_data(btnSdata.btn_num6, true);
                        break;
                    case "7":
                        send_data(btnSdata.btn_num7, true);
                        break;
                    case "8":
                        send_data(btnSdata.btn_num8, true);
                        break;
                    case "9":
                        send_data(btnSdata.btn_num9, true);
                        break;
                    case "0":
                        send_data(btnSdata.btn_num0, true);
                        break;
                    case "resCancel":
                        send_data(btnSdata.btn_songreservecancel, true);
                        break;
                    case "songstop":
                        send_data(btnSdata.btn_cancel, true);
                        break;
                    case "reserve":
                        send_data(btnSdata.btn_songreserve, true);
                        break;
                    case "start":
                        send_data(btnSdata.btn_start, true);
                        break;
                    case "tempoup":
                        send_data(btnSdata.btn_tempo_up, true);
                        break;
                    case "tempodown":
                        send_data(btnSdata.btn_tempo_down, true);
                        break;
                    case "sharp":
                        send_data(btnSdata.btn_sharp, true);
                        break;
                    case "flat":
                        send_data(btnSdata.btn_flat, true);
                        break;
                    case "female":
                        send_data(btnSdata.btn_female, true);
                        break;
                    case "male":
                        send_data(btnSdata.btn_male, true);
                        break;
                    case "disco":
                        send_data(btnSdata.btn_disco, true);
                        break;
                    case "medley":
                        send_data(btnSdata.btn_artistMedley, true);
                        break;
                    case "melody":
                        send_data(btnSdata.btn_audition, true);
                        break;

                }
            }
            
         
        }
        
        
        private void Window_Initialized(object sender, EventArgs e)
        {
           
            //cpw.Show();
            foreach(string p in SerialPort.GetPortNames())
            {
                portcombobox.Items.Add(p);
            }          
            serialPort1.BaudRate = 38400;           
            serialPort1.DataReceived += SerialPort1_DataReceived;
           
            // serialPort1.Open();
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string raw = serialPort1.ReadLine();
            if (raw.StartsWith("DOT"))
            {
                try
                {
                    string s = raw.Split(':')[1];
                    string[] n = s.Split(',');
                    int[] fnddata = Array.ConvertAll(n, int.Parse);
                    List<byte> dotdata = new List<byte>();

                    for (int i = 1; i < fnddata.Length; i++)
                    {
                        string temp = Convert.ToString(fnddata[i], 2).PadLeft(8, '0');
                        for (int k = 0; k < temp.Length; k++)
                        {
                            dotdata.Add(byte.Parse(temp[k].ToString()));
                        }
                    }


                    Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          label1.Content = getDots(dotdata.ToArray(), 30, 32 * 0);
                          label2.Content = getDots(dotdata.ToArray(), 30, 32 * 1);
                          label3.Content = getDots(dotdata.ToArray(), 30, 32 * 2);
                          label4.Content = getDots(dotdata.ToArray(), 30, 32 * 3);
                          label5.Content = getDots(dotdata.ToArray(), 30, 32 * 4);
                          label6.Content = getDots(dotdata.ToArray(), 30, 32 * 5);
                          label7.Content = getDots(dotdata.ToArray(), 30, 32 * 6);
                          label8.Content = getDots(dotdata.ToArray(), 30, 32 * 7);
                          label9.Content = getDots(dotdata.ToArray(), 30, 32 * 8);
                          label10.Content = getDots(dotdata.ToArray(), 30, 32 * 9);
                          label11.Content = getDots(dotdata.ToArray(), 30, 32 * 10);
                          label12.Content = getDots(dotdata.ToArray(), 30, 32 * 11);
                          label13.Content = getDots(dotdata.ToArray(), 30, 32 * 12);
                      }));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }             
            }
            else if (raw.StartsWith("FNDS"))
            {
                string s = raw.Split(':')[1];
                string[] n = s.Split(',');
                int[] fnddata = Array.ConvertAll(n, int.Parse);
                int songno = fnddata[6] + (fnddata[5] * 10) + (fnddata[4] * 100) + (fnddata[3] * 1000) + (fnddata[2] * 10000) + (fnddata[1] * 100000);
                cpw.songNum_changed((uint)songno);
                Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() =>
                 {
                     fnd_0.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[1]), UriKind.RelativeOrAbsolute));
                     fnd_1.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[2]), UriKind.RelativeOrAbsolute));
                     fnd_2.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[3]), UriKind.RelativeOrAbsolute));
                     fnd_3.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[4]), UriKind.RelativeOrAbsolute));
                     fnd_4.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[5]), UriKind.RelativeOrAbsolute));
                     fnd_5.Source = new BitmapImage(new Uri(string.Format(@"Resources\fnd_{0}.png", fnddata[6]), UriKind.RelativeOrAbsolute));
                 }));

            }
            else if (raw.StartsWith("FNDC"))
            {
                string s = raw.Split(':')[1];
                string[] n = s.Split(',');
                int[] fnddata = Array.ConvertAll(n, int.Parse);
                if (fnddata[1] == 1)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    img_time.Visibility = Visibility.Hidden;
                    img_coin.Visibility = Visibility.Visible;
                    fndcoin_dot.Visibility = Visibility.Hidden;
                    int coin = (fnddata[2] * 256) + fnddata[3];
                    fndcoin_2.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", coin%10), UriKind.RelativeOrAbsolute));
                    coin = coin / 10;
                    fndcoin_1.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", coin % 10), UriKind.RelativeOrAbsolute));
                    coin = coin / 10;
                    fndcoin_0.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", coin % 10), UriKind.RelativeOrAbsolute));
                }));
                }

                else if (fnddata[1] == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    img_time.Visibility = Visibility.Visible;
                    img_coin.Visibility = Visibility.Hidden;
                    fndcoin_dot.Visibility = Visibility.Visible;
                    //label16.Text = string.Format("남은시간 {0}:{1}", fnddata[2], fnddata[3].ToString().PadLeft(2, '0'));
                    int min = fnddata[3];
                    fndcoin_0.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", fnddata[2]), UriKind.RelativeOrAbsolute));
                    fndcoin_2.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", min % 10), UriKind.RelativeOrAbsolute));
                    min = min / 10;
                    fndcoin_1.Source = new BitmapImage(new Uri(string.Format(@"Resources\coin_{0}.png", min % 10), UriKind.RelativeOrAbsolute));
                }));
                }
            }
        }

        public string getDots(byte[] dot, int length, int start)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                for (int i = start; i < (length + start); i++)
                {
                    if (dot[i] == 0)
                    {
                        sb.Append("‎　 ");
                    }
                    else if (dot[i] == 1)
                    {
                        sb.Append("● ");
                    }

                }
                return sb.ToString();
            }
            catch
            {
                return "　 ";
            }
        }

        private void serialopen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort1.PortName = portcombobox.Text;
                serialPort1.Open();
                serialopen.IsEnabled = false;
                portcombobox.IsEnabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }

        private void portcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serialopen.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serialPort1.Close();
        }
    }
}
