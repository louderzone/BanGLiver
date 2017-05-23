using System.Windows;
using System.IO;
using System;
using Newtonsoft.Json;

namespace BanGLiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileSystemWatcher requestWatcher = new FileSystemWatcher();

        private String SongInfo = "";
        private String displayText_IDLE = "BanG-Liver Now Playing info";
        private String displayText_WAIT = "Waiting for Live...";
        private bool isLaunched;

        bool IsPlaying = false;
        public MainWindow()
        {
            InitializeComponent();
            requestWatcher.Path = "C:\\Temp";
            requestWatcher.NotifyFilter = NotifyFilters.LastWrite;
            requestWatcher.Filter = "requestBody.txt";
            requestWatcher.Changed += new FileSystemEventHandler(OnChanged);
            SetText(displayText_IDLE);
            isLaunched = false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isLaunched = !isLaunched;
            requestWatcher.EnableRaisingEvents = isLaunched;
            StartButton.Content = isLaunched ? "ストップ" : "スタート";
            SetText(isLaunched ? displayText_WAIT : displayText_IDLE);
            /*if(StartButton.Content.ToString() == "Start")
            {
                requestWatcher.EnableRaisingEvents = true;
                StartButton.Content = "Stop";
                SetText(displayText_WAIT);
            }
            else
            {
                requestWatcher.EnableRaisingEvents = false;
                StartButton.Content = "Start";
                SetText(displayText_IDLE);
            }*/
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try {
                using (StreamReader sr = new StreamReader(e.FullPath.ToString()))
                {
                    // Read the stream to a string, and write the string to the console.
                    String data = sr.ReadToEnd();
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(data);

                    if (jsonData.events[0].p1 != null)
                    {
                        SongInfo = jsonData.events[0].p1 + " [" + ((string)jsonData.events[0].p2).ToUpper() + "]";
                        SetText("再生中：" + SongInfo);
                        IsPlaying = true;
                    }
                    else if(IsPlaying){
                        SetText("済む：" + SongInfo);
                        IsPlaying = false;
                    }
                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SetText(String text)
        {
            System.IO.File.WriteAllText(@"C:\Temp\bdg_now_playing.txt", text + "          ");
            this.Dispatcher.Invoke(() =>
            {
                NowPlaying.Text = text;
            });
        }
    }
}
