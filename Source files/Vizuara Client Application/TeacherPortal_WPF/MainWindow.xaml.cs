using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media.Animation;
using Vizuara.TeacherPortal.Pages.Downloads;
using Vizuara.TeacherPortal.Pages.Home;
using Vizuara.TeacherPortal.Pages.Profile;
using Vizuara.TeacherPortal.Pages.FileChooser;
using Vizuara.TeacherPortal.Shared;
using System.ComponentModel;
using System.Threading;

namespace TeacherPortal_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This class is the parent/host class to host all other 4 classes
    /// This class will help switching between the 4
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations
        private double _aspectRatio;
        private bool? _adjustingHeight = null;
        private bool isTabOpened = false;
        
        private enum ContentType
        {
            Videos,
            Runable,
            Pictures,
            Models,
            Html
        }
        //List of possible extension
        //Make sure it corresponds to ContentType Enum
        private List<string[]> ContentExtensions = new List<string[]>
        {
            new string[2]{ "mp4","webm"},
            new string[1]{ "exe"},
            new string[3]{ "png", "jpg", "jpeg"},
            new string[1]{ "obj"},
            new string[1]{ "vizdata"}
        };

        #region ImagePath
        public class ImagePath {
            public string Enabled;
            public string Disabled;

            public ImagePath(string enabled, string disabled)
            {
                Enabled = enabled;
                Disabled = disabled;
            }
        }

        public ImagePath HomeImagePath;
        public ImagePath ClassesImagePath;
        public ImagePath DownloadsImagePath;
        public ImagePath ProfileImagePath;
        #endregion

        #endregion

        private VideoPlayer videoPlayer;
        private PopQuizzesDisplayer popQuizzesDisplayer;

        //private string Code;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            #region Setting up Ui image paths
            string GetMenuBarUIPath()
            {
                return "/Sprites/MenuBarUI/";
            }

            ImagePath Path(string Title)
            {
                return new ImagePath(GetMenuBarUIPath() + Title + "_Selected.png", GetMenuBarUIPath() + Title + "_Disabled.png");
            }

            HomeImagePath = Path("Home");
            ClassesImagePath = Path("Classes");
            DownloadsImagePath = Path("Downloads");
            ProfileImagePath = Path("Profile");

            #endregion

            InitializeComponent();
            this.SourceInitialized += Window_SourceInitialized;
            this.Closing += CloseExe;
        }
        
        private void CloseExe(object sender, CancelEventArgs e)
        {
            if(ExeDisplayer.app != null)
            {
                ExeDisplayer.app.CloseApplication();
            }
        }

        async void Quit(object sender, RoutedEventArgs e)
        {
            TeacherPortalSpecific.QuitTheApplication();
        }

        #region Setting Selected
        Uri GetUri(string Path)
        {
            return new Uri(Path, UriKind.Relative);
        }
        BitmapImage GetImage(string Path)
        {
            return new BitmapImage(GetUri(Path));
        }
        SolidColorBrush DisabledColor()
        {
            return new SolidColorBrush(Color.FromRgb(204, 204, 204));
        }
        SolidColorBrush EnabledColor()
        {
            return new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        ImageSource GetSource(ImagePath path, bool selected)
        {
            return selected ? GetImage(path.Enabled) : GetImage(path.Disabled);
        }
        SolidColorBrush GetColor(bool selected)
        {
            return selected ? EnabledColor() : DisabledColor(); ;
        }

        public void SetState(ImagePath path, Image image, TextBlock text, bool selected)
        {
            UIThread.RunOnUIThread(() => {
                image.Source = GetSource(path, selected);
                text.Foreground = GetColor(selected);
            });
        }
        public void HomeState(bool selected)
        {
            SetState(HomeImagePath, HomeImage, HomeText, selected);
        }
        public void ClassesState(bool selected)
        {
            SetState(ClassesImagePath, ClassesImage, ClassesText, selected);
        }
        public void DownloadsState(bool selected)
        {
            SetState(DownloadsImagePath, DownloadsImage, DownloadsText, selected);
        }
        public void ProfileState(bool selected)
        {
            SetState(ProfileImagePath, ProfileImage, ProfileText, selected);
        }
        public void SetUIStates(bool homeState, bool classesState, bool downloadsState, bool profileState)
        {
            HomeState(homeState);
            ClassesState(classesState);
            DownloadsState (downloadsState);
            ProfileState(profileState);
        }
        #endregion

        #region MenuButton_ClickedEvent
        /// <summary>
        /// When the home button is clicked
        /// </summary>
        private void HomeMenuButtonClicked_Event(object sender, RoutedEventArgs e)
        {
            ChangeTabEvent();

            MainDisplayScreen.Navigate(new Home());
            SetUIStates(true, false, false, false);
        }
        /// <summary>
        /// This function occurs when the user presses the Classes Button
        /// This function is the most complicated out of all 4 menu button functions
        /// </summary>
        private void ClassesMenuButtonClicked_Event(object sender, RoutedEventArgs e)
        {
            ChangeTabEvent();
            //videoPlayer = null;

            MainDisplayScreen.Navigate(new FileChooser((FilePath, FileExtension)=>
            {
                //There are several condition regarding stuff when
                #region Functions for checking file extensions
                ///<summary>
                ///to get the predefined list of file extensions
                /// </summary>
                string[] GetFileExtension(ContentType type)
                {
                    return ContentExtensions[(int)type];
                }

                ///<summary>
                ///Instead of checking the condition in a hard-coded way,
                ///this way we can check it in a much more readable way
                /// </summary>
                bool CheckFileExtension(string StringExtension, ContentType type)
                {
                    //Linq function to check for all possible extensions
                    return GetFileExtension(type).Where(x => x == StringExtension).Count() > 0;
                }
                #endregion

                #region Conditional Area
                //This action will be returned if the user click on something to display
                if (CheckFileExtension(FileExtension, ContentType.Videos))
                {
                    //Displaying the video with the video player xaml file
                    if (videoPlayer != null)
                    {
                        //videoPlayer.ChangeSource();
                        //videoPlayer = null;
                        videoPlayer.ChangeSource(FilePath);
                    }
                    else
                    {
                        videoPlayer = new VideoPlayer(FilePath);
                    }
                    MainDisplayScreen.Navigate(videoPlayer);

                }else if (CheckFileExtension(FileExtension, ContentType.Pictures))
                {
                    //Displaying the image file with image batch displayer with a scroll view
                    MainDisplayScreen.Navigate(new ImageDisplayer(FilePath));
                }else if(CheckFileExtension(FileExtension, ContentType.Models))
                {
                    //We try to run the 3D_Displayer Application with arguments

                    //This one is special, we will have to put the file path in the second parameter (that accepts arguments)
                    //with the first one pointing to the 3D_Displayer Application
                    string FileDirectory = Path.GetDirectoryName(FilePath);
                    string TextureName = Path.GetFileName(FilePath).Split('.')[0];

                    //Place holder
                    MainDisplayScreen.Navigate(new ExeDisplayer(FileRelated.FileManager.GetWorkingDirectory() + "\\3D_Displayer.exe", "\"ModelPath="+FilePath+"\" \"code=Hello World\""));
                }else if(CheckFileExtension(FileExtension, ContentType.Html))
                {
                    if (popQuizzesDisplayer != null)
                    {
                        popQuizzesDisplayer.DisposeBrowser();
                    }
                        popQuizzesDisplayer = new PopQuizzesDisplayer(FilePath);
                        MainDisplayScreen.Navigate(popQuizzesDisplayer);
                }
                else
                {
                    //Playable files
                    MainDisplayScreen.Navigate(new ExeDisplayer(FilePath));
                }
                #endregion
            }));
            SetUIStates(false, true, false, false);
        }
        /// <summary>
        /// When the Download tab button is clicked
        /// </summary>
        private void DownloadMenuButtonClicked_Event(object sender, RoutedEventArgs e)
        {
            ChangeTabEvent();
            MainDisplayScreen.Navigate(new Downloads());
            SetUIStates(false, false, true, false);
        }
        /// <summary>
        /// When the Profile tab button is clicked
        /// </summary>
        private void ProfileMenuButtonClicked_Event(object sender, RoutedEventArgs e)
        {
            ChangeTabEvent();
            MainDisplayScreen.Navigate(new Profile());
            SetUIStates(false, false, false, true);
        }

        public void ChangeTabEvent()
        {
            if (videoPlayer != null)
            {
                videoPlayer.DisposeVideoPlayer();
                videoPlayer.fs.Close();
            }

            if (popQuizzesDisplayer != null)
            {
                popQuizzesDisplayer.DisposeBrowser();
            }

            if (ExeDisplayer.app != null)
            {
                ExeDisplayer.app.CloseApplication();
            }
        }
        #endregion
        /// <summary>
        /// Whem the tab is opened
        /// </summary>
        private void TabEvent(object sender, RoutedEventArgs e)
        {
            if (isTabOpened)
            {
                isTabOpened = false;

                new Thread(() =>
                {
                        for (int i = 7; i >= 0; i--)
                        {
                            this.Dispatcher.Invoke((Action)delegate { LeftTab.ColumnDefinitions[0].Width = new GridLength(0.02 * i, GridUnitType.Star); });
                            //Task.Delay(1).Wait();
                            Thread.Sleep(1);
                        }
                }).Start();
            }
            else
            {
                isTabOpened = true;
                Task.Run(() => {
                    for (int i = 0; i <= 7; i++)
                    {
                        this.Dispatcher.Invoke((Action)delegate { LeftTab.ColumnDefinitions[0].Width = new GridLength(0.02 * i, GridUnitType.Star); });
                        Task.Delay(1).Wait();
                    }
                });
            }
        }
        #region ThirdPartyCode
        //From https://itecnote.com/tecnote/resize-a-wpf-window-but-maintain-proportions/
        internal enum SWP
        {
            NOMOVE = 0x0002
        }
        internal enum WM
        {
            WINDOWPOSCHANGING = 0x0046,
            EXITSIZEMOVE = 0x0232,
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition() // mouse position relative to screen
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        private void Window_SourceInitialized(object sender, EventArgs ea)
        {

            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual((Window)sender);
            hwndSource.AddHook(DragHook);

            _aspectRatio = this.Width / this.Height;
        }
        private IntPtr DragHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM)msg)
            {
                case WM.WINDOWPOSCHANGING:
                    {
                        WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

                        if ((pos.flags & (int)SWP.NOMOVE) != 0)
                            return IntPtr.Zero;

                        Window wnd = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                        if (wnd == null)
                            return IntPtr.Zero;

                        // determine what dimension is changed by detecting the mouse position relative to the 
                        // window bounds. if gripped in the corner, either will work.
                        if (!_adjustingHeight.HasValue)
                        {
                            Point p = GetMousePosition();

                            double diffWidth = Math.Min(Math.Abs(p.X - pos.x), Math.Abs(p.X - pos.x - pos.cx));
                            double diffHeight = Math.Min(Math.Abs(p.Y - pos.y), Math.Abs(p.Y - pos.y - pos.cy));

                            _adjustingHeight = diffHeight > diffWidth;
                        }

                        if (_adjustingHeight.Value)
                            pos.cy = (int)(pos.cx / _aspectRatio); // adjusting height to width change
                        else
                            pos.cx = (int)(pos.cy * _aspectRatio); // adjusting width to heigth change

                        Marshal.StructureToPtr(pos, lParam, true);
                        handled = true;
                    }
                    break;
                case WM.EXITSIZEMOVE:
                    _adjustingHeight = null; // reset adjustment dimension and detect again next time window is resized
                    break;
            }

            return IntPtr.Zero;
        }
        #endregion
    }

    public class TeacherPortalSpecific
    {
        public static void QuitTheApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
