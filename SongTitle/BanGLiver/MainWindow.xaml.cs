using System.Windows;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using BanGLiver.Extension;
using BanGLiver.Modal.BangDream;
using Newtonsoft.Json;

namespace BanGLiver
{
    public partial class MainWindow : Window
    {
        private FileSystemWatcher RequestWatcher { get; }= new FileSystemWatcher();

        private string _songInfo = "";
        private readonly string _textIdle = "BanG-Liver Now Playing info";
        private readonly string _textWait = "Waiting for Live data...";
        private bool _isLaunched;
        private bool _isPlaying;

        public MainWindow()
        {
            InitializeComponent();
            RequestWatcher.Path = @"C:\Temp";
            RequestWatcher.NotifyFilter = NotifyFilters.LastWrite;
            RequestWatcher.Filter = "requestBody.txt";
            RequestWatcher.Changed += RequestBody_OnChanged;
            SetText(_textIdle);
            _isLaunched = false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _isLaunched = !_isLaunched;
            RequestWatcher.EnableRaisingEvents = _isLaunched;
            StartButton.Content = _isLaunched ? "ストップ" : "スタート";
            SetText(_isLaunched ? _textWait : _textIdle);
        }

        /// <summary>
        /// Handles file watcher request body on changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RequestBody_OnChanged(object source, FileSystemEventArgs e)
        {
            try {
                using (var sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    var text = sr.ReadToEnd();
                    var jsonData = text.JsonDeserialize<TapjoyRequest>();

                    if (jsonData.Events.Any())
                    {
                        // Has event
                        var tapjoyEvent = jsonData.Events.First();
                        _songInfo = $"{tapjoyEvent.P1} [{tapjoyEvent.P2.ToUpper()}]";
                        SetText("再生中：" + _songInfo);
                        _isPlaying = true;
                    }
                    else if(_isPlaying){
                        SetText("済む：" + _songInfo);
                        _isPlaying = false;
                    }
                
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Procress text and save text for display
        /// </summary>
        /// <param name="text"></param>
        private void SetText(string text)
        {
            File.WriteAllText(@"C:\Temp\bdg_now_playing.txt", $@"{text}          ");
            Dispatcher.Invoke(() =>
            {
                NowPlaying.Text = text;
            });
        }
    }
}
