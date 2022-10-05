using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.IO;
using Vizuara.TeacherPortal.Shared;
using Vlc.DotNet.Wpf;
using System.ComponentModel;

namespace Vizuara.TeacherPortal.Pages.FileChooser
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : Page
    {
        public FileStream fs;
        public VlcControl VideoControl;

        private bool DoneInitialization = false;
        private bool isDragging = false;
        public VideoPlayer(string Source)
        {
            InitializeComponent();
            ChangeSource(Source);
        }
        
        /// <summary>
        /// Only initialize when VideoControl is not empty
        /// </summary>
        public void InitializeVideoPlayer()
        {
            if (DoneInitialization)
            {
                    VLCParentControl.Children.RemoveAt(VLCParentControl.Children.Count-1);
            }
            DoneInitialization = true;
            Debug.Log("New Video Control being created");
            VideoControl = new VlcControl();
            VideoControl.BeginInit();
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            var options = new string[]
            {
            // VLC options can be given here. Please refer to the VLC command line documentation.
            };
                // Default installation path of VideoLAN.LibVLC.Windows
                var libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

                VideoControl.SourceProvider.CreatePlayer(libDirectory, options);
                VideoControl.EndInit();
                VLCParentControl.Children.Add(VideoControl);
                VideoControl.SourceProvider.MediaPlayer.PositionChanged += VideoPosition_Changed;
                
        }

        /// <summary>
        /// Main Change Source Method don't use it
        /// </summary>
        private void ChangeSource()
        {
            PlayActionImage.Source = GetImage("\\Sprites\\UI\\PauseButton.png");
            VideoControl.SourceProvider.MediaPlayer.Play(fs);
            UIThread.RunOnUIThread(async ()=>
            {
                await Task.Delay(100);
                TotalTime.Text = GetTimeStringFromMillisecond(VideoControl.SourceProvider.MediaPlayer.Length);
            });
        }

        /// <summary>
        /// Method for other classes to change the source within this class
        /// </summary>
        /// <param name="NewSource"></param>
        public void ChangeSource(string NewSource)
        {
            Debug.Log("Changing Video Source");

            DisposeVideoPlayer();

            string TempFolder = System.IO.Path.GetTempPath();
            string NewTemp = TempFolder + "TEMPFILE" + RandomStringGenerator(8) + ".webm";
            Debug.Log(NewTemp);
            FileRelated.Security.DecryptFile(NewSource, NewTemp);
            //Thread.Sleep(100);
            //Thread.Sleep(100);
            InitializeVideoPlayer();

            fs = new FileStream(NewTemp, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            ChangeSource();
        }

        public void DisposeVideoPlayer()
        {
            if (VideoControl != null)
            {
                Application.Current.Dispatcher.InvokeAsync(() => {
                    VideoControl.Dispose();
                    VideoControl = null;
                });
            }
        }

        public string RandomStringGenerator(int NumberOfCharacters)
        {
            var rand = new Random();
            string Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string FinalRandomString = "";
            for(int i = 0; i < NumberOfCharacters; i++)
            {
                FinalRandomString += Characters[rand.Next(0, Characters.Length)];
            }
            return FinalRandomString;
        }

        private void VideoPosition_Changed(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (VideoControl != null)
            {
                UIThread.RunOnUIThread(() =>
                {
                    SliderControl.Value = VideoControl.SourceProvider.MediaPlayer.Position * 100;
                    CurrentTime.Text = GetTimeStringFromMillisecond(VideoControl.SourceProvider.MediaPlayer.Time);
                });
            }
        }

        #region UI Events
        private void MoveToPosition(object sender, RoutedEventArgs e)
        {
            if (isDragging)
            {
                VideoControl.SourceProvider.MediaPlayer.Position = (float)SliderControl.Value / 100;
            }
        }

        private void DragStarted(object sender, RoutedEventArgs e)
        {
            PauseVideo();
            isDragging = true;
        }

        private void DragCompleted(object sender, RoutedEventArgs e)
        {
            ResumeVideo();
            isDragging = false;
        }
        private void PlayActionButton_Click(object sender, RoutedEventArgs e)
        {
            if(VideoControl != null)
            {
                //VideoControl.SourceProvider.MediaPlayer.Position = 0.5f;

                if (VideoControl.SourceProvider.MediaPlayer.IsPlaying())
                {
                    PauseVideo();
                }
                else
                {
                    ResumeVideo();
                }

                //Length is the total duration of the video
                //Debug.Log(VideoControl.SourceProvider.MediaPlayer.Length+"");
            }
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region Custom Functions
        private void PauseVideo()
        {
            VideoControl.SourceProvider.MediaPlayer.Pause();
            PlayActionImage.Source = GetImage("\\Sprites\\UI\\PlayButton.png");
        }

        private void ResumeVideo()
        {
            VideoControl.SourceProvider.MediaPlayer.Play();
            PlayActionImage.Source = GetImage("\\Sprites\\UI\\PauseButton.png");
        }

        Uri GetUri(string Path)
        {
            return new Uri(Path, UriKind.Relative);
        }

        BitmapImage GetImage(string Source)
        {
            return new BitmapImage(GetUri(Source));
        }

        private string GetTimeStringFromMillisecond(long Time)
        {
            //Time will be coming in millisecond so we will round it up to nearest int after dividing it by 1000
            int TotalSeconds = (int)Math.Round((double)(Time / 1000));
            int Minutes = TotalSeconds / 60;
            int Seconds = TotalSeconds % 60;

            string MinuteString = (Minutes >= 10) ? (Minutes + "") : ("0" + Minutes);
            string SecondString = (Seconds >= 10) ? (Seconds + "") : ("0" + Seconds);

            return MinuteString + ":" + SecondString;
        }

        #endregion
    }
}
