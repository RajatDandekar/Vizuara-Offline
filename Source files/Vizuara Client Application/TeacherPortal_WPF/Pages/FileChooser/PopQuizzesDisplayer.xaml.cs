using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using CefSharp;
using Microsoft.Web;
using Vizuara.TeacherPortal.Shared;

namespace Vizuara.TeacherPortal.Pages.FileChooser
{
    /// <summary>
    /// Interaction logic for PopQuizzesDisplayer.xaml
    /// </summary>
    public partial class PopQuizzesDisplayer : Page
    {
        public PopQuizzesDisplayer(string FilePath)
        {
            InitializeComponent();
            Debug.Log("Should be loading");
            Browser.MenuHandler = new CustomMenuHandler();
            string URLData = FileRelated.Security.DecryptString(FilePath);
            Debug.Log(URLData);
            Browser.Load(URLData);
            //HideScriptErrors(Browser, true);
        }

        public void DisposeBrowser()
        {
            Browser.Dispose();
        }
    }
    public class CustomMenuHandler : CefSharp.IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}
