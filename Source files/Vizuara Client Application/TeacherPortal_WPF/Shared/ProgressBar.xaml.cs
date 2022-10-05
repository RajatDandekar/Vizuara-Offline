using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vizuara.TeacherPortal.Shared
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        public void ChangeProgressBarValue(int newValue, string StringValue)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgressBarText.Text = StringValue;
                pbStatus.Value = newValue;
            });
        }
    }
}
