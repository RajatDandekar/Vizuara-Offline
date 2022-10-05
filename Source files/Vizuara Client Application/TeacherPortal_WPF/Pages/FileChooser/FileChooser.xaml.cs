using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

using Vizuara.TeacherPortal.Shared;

namespace Vizuara.TeacherPortal.Pages.FileChooser
{
    /// <summary>
    /// Interaction logic for FileChooser.xaml
    /// </summary>
    public partial class FileChooser : Page
    {
        static FileChooser_DataContext main;
        Action<string, string> OpenFile;
        public FileChooser(Action<string, string> OpenFile = null)
        {
            this.OpenFile = OpenFile;
            main = new FileChooser_DataContext(); 
            InitializeComponent();

            BackButtonVisibility(Visibility.Hidden);

            this.DataContext = main;
            GetDownloaded();
        }


        #region Custom Functions
        private void GetDownloaded()
        {
            if (!FileChooser_DataContext.ClassChosen)
            {
                BackButtonVisible(false);
                DeleteButtonVisible(true);

                ClearData();

                FileChooser_DataContext.AvailableOptions.Clear();
                foreach (string data in FileRelated.FileManager.GetDownloadedClasses())
                {
                    FileChooser_DataContext.OptionsClass newOption = new FileChooser_DataContext.OptionsClass(data);
                    FileChooser_DataContext.AvailableOptions.Add(newOption);
                }
            }
            else
            {
                BackButtonVisible(true);
                DeleteButtonVisible(false);
            }
        }
        private void ClearData()
        {
            FileChooser_DataContext.ChapterChosen = false;
            FileChooser_DataContext.ClassChosen = false;
            FileChooser_DataContext.ItemTypeChosen = false;
            FileChooser_DataContext.isRunnable = false;

            FileChooser_DataContext.ChosenChapter = "";
            FileChooser_DataContext.ChosenClass = "";
            FileChooser_DataContext.ChosenTypeOfData = "";
        }
        private void CmdDeleteFunction()
        {
            string FilePath = FileRelated.FileManager.GetDataFilePath();

            ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
            char DoubleQuote = '"';
            string Command = "/c " + @"rmdir /s /q " + DoubleQuote + FilePath + DoubleQuote;
            info.Arguments = Command;
            Debug.Log(Command);
            Process cmd = Process.Start(info);
            cmd.WaitForExit(100);

            //Process.Start("cmd.exe", "/c " + @"rmdir /s/q "+FilePath);
        }
        #region Button States
        private void BackButtonVisibility(Visibility newVisibility)
        {
            UIThread.RunOnUIThread(() =>
            {
                BackText.Visibility = newVisibility;
                BackButton.Visibility = newVisibility;
            });
        }
        private void DeleteButtonVisibility(Visibility newVisibility)
        {
            UIThread.RunOnUIThread(() =>
            {
                DeleteButton.Visibility = newVisibility;
                DeleteText.Visibility = newVisibility;
            });
        }
        private void BackButtonVisible(bool Visible)
        {
            if (Visible)
            {
                BackButtonVisibility(Visibility.Visible);
            }
            else
            {
                BackButtonVisibility(Visibility.Hidden);
            }
        }
        private void DeleteButtonVisible(bool Visible)
        {
            if (Visible)
            {
                DeleteButtonVisibility(Visibility.Visible);
            }
            else
            {
                DeleteButtonVisibility(Visibility.Hidden);
            }
        }
        #endregion

        #endregion

        #region Button Event Functions
        public void DeleteAll(object sender, RoutedEventArgs e)
        {
            try
            {
                LongFunctions.LongDirectory.Delete(LongFunctions.LongDirectory.GetWin32LongPath(FileRelated.FileManager.GetDataFilePath()), true);
            }
            catch (IOException except)
            {
                CmdDeleteFunction();
            }
            LongFunctions.LongDirectory.CreateDirectory(LongFunctions.LongDirectory.GetWin32LongPath(FileRelated.FileManager.GetDataFilePath()));

            FileChooser_DataContext.ChapterChosen = false;
            FileChooser_DataContext.ClassChosen = false;
            FileChooser_DataContext.ItemTypeChosen = false;
            FileChooser_DataContext.isRunnable = false;

            GetDownloaded();
            BackButtonVisible(false);
        }
        public void GoBack(object sender, RoutedEventArgs e)
        {
            Button DummyButton = new Button();
            if (FileChooser_DataContext.ItemTypeChosen)
            {
                FileChooser_DataContext.ItemTypeChosen = false;
                FileChooser_DataContext.ChapterChosen = false;
                FileChooser_DataContext.isRunnable = false;

                DummyButton.Content = FileChooser_DataContext.ChosenChapter;
                FileChooser_DataContext.ChosenChapter = "";
                FileChooser_DataContext.ChosenTypeOfData = "";

                OptionChosen(DummyButton, new RoutedEventArgs());

            }
            else if (FileChooser_DataContext.ChapterChosen)
            {
                FileChooser_DataContext.ItemTypeChosen = false;
                FileChooser_DataContext.ChapterChosen = false;
                FileChooser_DataContext.ClassChosen = false;
                FileChooser_DataContext.isRunnable = false;

                DummyButton.Content = FileChooser_DataContext.ChosenClass;

                FileChooser_DataContext.ChosenClass = "";
                FileChooser_DataContext.ChosenChapter = "";

                OptionChosen(DummyButton, new RoutedEventArgs());
            }
            else
            {
                FileChooser_DataContext.ChapterChosen = false;
                FileChooser_DataContext.ClassChosen = false;
                FileChooser_DataContext.ItemTypeChosen = false;
                FileChooser_DataContext.isRunnable = false;

                GetDownloaded();
                DeleteButtonVisible(true);
                BackButtonVisible(false);
            }
        }
        private void OptionChosen(object sender, RoutedEventArgs e)
        {
            string content = (string)((Button)sender).Content;
            FileChooser_DataContext.AvailableOptions.Clear();

            BackButtonVisible(false);
            DeleteButtonVisible(true);

            if (!FileChooser_DataContext.ClassChosen)
            {
                foreach (string data in FileRelated.FileManager.GetChapters(content))
                {
                    FileChooser_DataContext.AvailableOptions.Add(new FileChooser_DataContext.OptionsClass(data));
                }
                FileChooser_DataContext.ChosenClass = content;
                FileChooser_DataContext.ClassChosen = true;

                BackButtonVisible(true);
                DeleteButtonVisible(false);

            } else if (!FileChooser_DataContext.ChapterChosen)
            {
                FileChooser_DataContext.AvailableOptions.Clear();
                foreach (string data in FileRelated.FileManager.GetTypesOfData(FileChooser_DataContext.ChosenClass, content))
                {
                    FileChooser_DataContext.AvailableOptions.Add(new FileChooser_DataContext.OptionsClass(data));
                }
                FileChooser_DataContext.ChosenChapter = content;
                FileChooser_DataContext.ChapterChosen = true;

                BackButtonVisible(true);
                DeleteButtonVisible(false);

            } else if (!FileChooser_DataContext.ItemTypeChosen)
            {
                DeleteButtonVisible(false);
                BackButtonVisible(true);
                bool ContainObj = false;
                foreach (string data in FileRelated.FileManager.GetItems(FileChooser_DataContext.ChosenClass, FileChooser_DataContext.ChosenChapter, content))
                {
                    if (data.Contains(".obj"))
                    {
                        ContainObj = true;
                        break;
                    }
                }

                foreach (string data in FileRelated.FileManager.GetItems(FileChooser_DataContext.ChosenClass, FileChooser_DataContext.ChosenChapter, content))
                {
                    if (!data.Contains(".obj") && ContainObj)
                    {
                        break;
                    }
                    FileChooser_DataContext.AvailableOptions.Add(new FileChooser_DataContext.OptionsClass(data));
                }


                //What we will check is if the current folder any files, if not it is a WebGL folder we go further than this
                if (FileChooser_DataContext.AvailableOptions.Count < 1)
                {
                    FileChooser_DataContext.isRunnable = true;
                    //we will specifically get the folder
                    foreach(string data in FileRelated.FileManager.GetExeFolders(FileChooser_DataContext.ChosenClass, FileChooser_DataContext.ChosenChapter, content))
                    {
                        FileChooser_DataContext.AvailableOptions.Add(new FileChooser_DataContext.OptionsClass(data));
                    }
                }
                FileChooser_DataContext.ItemTypeChosen = true;
                FileChooser_DataContext.ChosenTypeOfData = content;
            }else if (FileChooser_DataContext.isRunnable)
            {
                DeleteButtonVisible(false);
                BackButtonVisible(true);

                //FileChooser_DataContext.isRunnable = false;
                //since it is a webGL folder, we will directly open the index.html file from here
                OpenFile(FileRelated.FileManager.GetRunnableFilePath(FileChooser_DataContext.ChosenClass, FileChooser_DataContext.ChosenChapter, FileChooser_DataContext.ChosenTypeOfData, content),"html");
                
            }
            else
            {
                DeleteButtonVisible(false);
                BackButtonVisible(true);
                string GetFileExtension(string FileName)
                {
                    string[] SplitString = FileName.Split('.');
                    return SplitString[SplitString.Length - 1];
                }
                //Now user choose the item to display
                string FilePath = FileRelated.FileManager.GetDirectoryPath(FileChooser_DataContext.ChosenClass, FileChooser_DataContext.ChosenChapter, FileChooser_DataContext.ChosenTypeOfData) + "\\" + content;
                FilePath = FilePath.Replace("\\", "/");

                //ClearData();
                OpenFile(FilePath, GetFileExtension(content));
            }
        }
        #endregion
    }

    ///<summary>
    ///View Model for File Choose Class
    /// </summary>
    public class FileChooser_DataContext
    {
        public static string ChosenClass;
        public static string ChosenChapter;
        public static string ChosenTypeOfData;
        public static string ChosenWebGLFolder;

        public static bool ClassChosen = false;
        public static bool ChapterChosen = false;
        public static bool ItemTypeChosen = false;
        public static bool isRunnable = false;

        public static ObservableCollection<OptionsClass> AvailableOptions { get; set; }

        public FileChooser_DataContext()
        {
            if (AvailableOptions == null)
            {
                AvailableOptions = new ObservableCollection<OptionsClass> { };
            }
            if (ClassChosen == false)
            {
                ChosenClass = "";
                ChosenChapter = "";
                ChosenTypeOfData = "";
                ClassChosen = false;
                ChapterChosen = false;
                ItemTypeChosen = false;
                isRunnable = false;
            }
        }

        [Serializable]
        public class OptionsClass
        {
            public string optionName
            {
                get;
                set;
            }

            public string displayName
            {
                get;
                set;
            }

            public OptionsClass(string name)
            {
                optionName = name;

                //Changing the display name
                if (name.Contains('.'))
                {
                    string[] Splited = name.Split('.');
                    if (Splited.Length == 3)
                    {
                        displayName = name.Split('.')[1];
                    }
                    else
                    {
                        displayName = name.Split('.')[0];
                    }
                }
                else
                {
                    displayName = name;
                }
            }
        }
    }
}
