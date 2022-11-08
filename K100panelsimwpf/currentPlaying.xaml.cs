using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace K100panelsimwpf
{
    /// <summary>
    /// currentPlaying.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class currentPlaying : Window
    {
        static JArray songdb = new JArray();
        public string serverip = "127.0.0.1";
        public int serverPort = 9000;
        public static uint currentSong = 0;
        static HttpListener listener = new HttpListener(); //HTTPサーバ
        static Thread thread = new Thread(new ParameterizedThreadStart(WorkerThread));
        public currentPlaying()
        {
            InitializeComponent();
        }
        private static JObject getSongInfo(uint songnum)
        {

            for (int i = 0; i < songdb.Count; i++)
            {
                JObject o = new JObject((JObject)songdb[i]);
                if (uint.Parse(o["snum"].ToString()) == songnum)
                {
                    return o;
                }
            }
            return null;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            listener.Prefixes.Add(string.Format("http://{0}:{1}/", serverip, serverPort));
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            songdb = JArray.Parse(File.ReadAllText("./db.json"));
            listener.Start();  //新しいスレッドでサーバ開始
            if (!thread.IsAlive)
                thread.Start(listener);
        }
        private static void ProcessRequest(HttpListenerContext ctx) //サーバに要求があったとき実行される関数
        {
            string url = ctx.Request.RawUrl;
            if (ctx.Request.Url.LocalPath.StartsWith("/currently-playing"))
            {
                //MessageBox.Show(ctx.Request.HttpMethod);
                JObject o = getSongInfo(currentSong);
                JObject ret = new JObject();
                if(o != null)
                {
                    ret.Add("songName", o["stitile"].ToString());
                    ret.Add("songArtist", o["sartist"].ToString());
                    ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes(ret.ToString()));
                    //ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("abc"));
                }
                else
                {
                    ret.Add("songName", "-");
                    ret.Add("songArtist", "-");
                    ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes(ret.ToString()));
                }
            }
            else
            {
                ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("Error"));
            }
            #region Todo
            /*
           if (ctx.Request.Url.LocalPath.StartsWith("/songReq"))
           {
               string a = System.IO.Path.GetFileNameWithoutExtension(ctx.Request.Url.LocalPath);
               int key = 0;
               int songN = 0;
               if (a.Contains("&"))
               {
                   try
                   {
                       key = int.Parse(a.Split('&')[1]);
                       a = a.Split('&')[0];
                   }
                   catch
                   {
                       ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("NG;1"));
                       return;
                   }
               }

               if (int.TryParse(a, out songN) && Math.Abs(key) <= 6)
               {
                   if (songN <= 99999 && songN > 100)
                   {

                       if (mainwin.backgroundWorker1.IsBusy)
                       {
                           ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("NG;2"));
                       }
                       else
                       {
                           ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("OK"));
                           mainwin.songQueue.Add(string.Format("{0},{1}", songN, key));
                       }

                   }
                   else
                   {
                       ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("NG;3"));
                       // frm.addLog(string.Format("클라이언트: {0}, 곡 예약 시도 {1} 번", clientip, songN));
                   }
               }
               else
               {
                   ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("NG;4"));
                   //frm.addLog(string.Format("클라이언트: {0}, 잘못된 형식으로 예약 시도 {1}", clientip, a));
               }

           }
           else
           {
               ResponceProcessBinary(ctx, Encoding.UTF8.GetBytes("NG;4"));
           }*/
            #endregion

        }
        private static void ResponceProcessBinary(HttpListenerContext ctx, byte[] data)
        {
            //HttpListenerRequest request = ctx.Request;
            HttpListenerResponse response = ctx.Response;
            //헤더 설정        
            response.Headers.Add("Accept-Encoding", "none"); //gzip 처리하기 귀찮으므로 비압축
            response.Headers.Add("Content-Type", "application/json; charset=UTF-8");
            response.Headers.Add("Server", "KaraokeReqAPI");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS;");

            //스트림 쓰기
            response.ContentLength64 = data.Length;
            Stream output = response.OutputStream;
            output.Write(data, 0, data.Length);
        }
        private static void WorkerThread(object arg)
        {
            try
            {
                while (listener.IsListening)
                {
                    HttpListenerContext ctx = listener.GetContext();
                    ProcessRequest(ctx);
                }
            }
            catch (ThreadAbortException)
            {
                //frm.textBox1.AppendText("Normal Stopping Service");
            }
        }
        public void songNum_changed(uint snum)
        {          
            Thread th = new Thread(task);
            currentSong = snum;
            th.Start();
            void task()
            {
                JObject info = getSongInfo(snum);
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {

                        songNum.Content = snum.ToString().PadLeft(5, '0');
                        if (info != null)
                        {
                            songName.Content = string.Format("{0} - {1}", info["stitile"].ToString(), info["sartist"].ToString());
                        }
                        else
                        {
                            songName.Content = "정보없음";
                        }
                    }));
            }
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (thread.IsAlive)
            {
                thread.Abort(listener);
                listener.Stop();
            }
        }
    }
}
