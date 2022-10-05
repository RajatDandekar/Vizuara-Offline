using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vizuara.TeacherPortal.Shared;

namespace Vizuara.TeacherPortal.Pages.FileChooser
{
    /// <summary>
    /// Interaction logic for ExeDisplayer.xaml
    /// </summary>
    public partial class ExeDisplayer : Page
    {
        public static UnityHwndHost app;
        public ExeDisplayer(string FilePath, string Argument = "")
        {
            InitializeComponent();
            app = new UnityHwndHost(LongFunctions.LongFile.GetWin32LongPath(FilePath), Argument);
            ContentGrid.Children.Add(app);

            #region Trying to fix the issue where the screen is not centered
            Task.Run(async () =>
            {
                await Task.Delay(500);
                UIThread.RunOnUIThread(() =>
                {
                    ParentGrid.RowDefinitions[0].Height = new GridLength(0.1f, GridUnitType.Star);
                });
            });
            #endregion
        }
    }

    /// <summary>
    /// The class to handle unity window
    /// </summary>
    public class UnityHwndHost : HwndHost
    {
        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")] // TODO: 32/64?
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);
        internal const Int32 GWLP_USERDATA = -21;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        internal const UInt32 WM_CLOSE = 0x0010;

        private string programName;
        private string arguments;
        private Process process = null;
        private IntPtr unityHWND = IntPtr.Zero;

        public UnityHwndHost(string programName, string arguments = "")
        {
            this.programName = programName;
            this.arguments = arguments;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Debug.Log("Going to launch Unity at: " + this.programName + " " + this.arguments);
            process = new Process();
            process.StartInfo.FileName = programName;
            process.StartInfo.Arguments = "-parentHWND " + hwndParent.Handle + " " + arguments + (arguments.Length == 0 ? "" : " ");
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            process.WaitForInputIdle();

            int repeat = 50;
            while (unityHWND == IntPtr.Zero && repeat-- > 0)
            {
                Thread.Sleep(100);
                EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);
            }
            if (unityHWND == IntPtr.Zero)
                throw new Exception("Unable to find Unity window");
            Debug.Log("Found Unity window: " + unityHWND);

            repeat += 150;
            while ((GetWindowLongPtr(unityHWND, GWLP_USERDATA).ToInt32() & 1) == 0 && --repeat > 0)
            {
                Thread.Sleep(100);
                Debug.Log("Waiting for Unity to initialize... " + repeat);
            }

            if (repeat == 0)
            {
                Debug.Log("Timed out while waiting for Unity to initialize");
            }
            else
            {
                Debug.Log("Unity initialized!");
            }
            return new HandleRef(this, unityHWND);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            if (unityHWND != IntPtr.Zero)
                throw new Exception("Found multiple Unity windows");
            unityHWND = hwnd;
            return 0;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Debug.Log("Asking Unity to exit...");
            PostMessage(unityHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            int i = 30;
            while (!process.HasExited)
            {
                if (--i < 0)
                {
                    Debug.Log("Process not dead yet, killing...");
                    process.Kill();
                }
                Thread.Sleep(100);
            }
        }

        public void CloseApplication()
        {
            if (process != null)
            {
                //try catch block in case the application exits by itself after running in the background
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException exception)
                {
                    Debug.Log(exception.Message);
                    //Do Nothing
                }
            }
        }
    }
}
