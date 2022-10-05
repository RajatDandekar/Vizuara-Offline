using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using Vizuara.TeacherPortal.Shared;

namespace Vizuara.TeacherPortal.Pages.Downloads
{
    /// <summary>
    /// Interaction logic for Downloads.xaml
    /// </summary>
    public partial class Downloads : Page
    {

        DownloadClass_ViewModel main;
        /// <summary>
        /// Initialization
        /// </summary>
        public Downloads()
        {
            main = new DownloadClass_ViewModel();
            InitializeComponent();
            DataContext = main;

            if (InternetConnection.IsInternetAvailable())
            {
                //If internet is available, we request the classes dropdown
                SetClassesDropdown();
            }
            else
            {
                //If not, we show an error dialog
                
            }
            //First we will try to get the value
        }

        #region DropdownSetting
        /// <summary>
        /// Set the dropdown data
        /// </summary>
        public void SetDropdown(Code DropdownCode, Action<string> LoopAction, Action<int> EndAction, string Class = "", string Chapter = "")
        {
            //async task to get dropdown data from the server
            Task.Run(async () =>
            {
                string JSON;
                //Setting JSON based on type of data
                switch (DropdownCode)
                {
                    case Code.GetClasses:
                        {
                            JSON = await ConnectionManager.GetClassesData();
                            break;
                        }
                    case Code.GetChapters:
                        {
                            JSON = await ConnectionManager.GetChaptersData(Class);
                            break;
                        }
                    case Code.GetValuesInChapters:
                        {
                            JSON = await ConnectionManager.GetChapterDownloadableList(Class, Chapter);
                            break;
                        }
                    default:
                        {
                            JSON = await ConnectionManager.GetClassesData();
                            break;
                        }
                }

                while(!(JSON.Length > 0))
                {
                    await Task.Delay(100);
                }
                try
                {
                    ResponseClass[] responses = JsonConvert.DeserializeObject<ResponseClass[]>(JSON);

                    //Debug.Log(JSON);
                    int length = responses.Length;
                    int index = 0;

                    foreach (ResponseClass data in responses)
                    {
                        Debug.Log("Action Invoked");
                        LoopAction(data.value);
                        //invoke()
                    }

                    EndAction(length);

                }
                catch (JsonReaderException e)
                {
                    Debug.Log(e.Message);
                }
            });
        }

        /// <summary>
        /// Set the classes dropdown
        /// </summary>
        public void SetClassesDropdown()
        {
            List<string> data = new List<string>();
            SetDropdown(Code.GetClasses, (ReturnData) =>
            {
                data.Add(ReturnData);
                //Debug.Log(ReturnData);
            }, (length) =>
             {
             RunUIThread(() =>
             {
                 main.ClassesList.Clear();
                 main.ClassesList.Add("None");
                 ClassesDropdown.SelectedIndex = 0;
                 foreach (string value in data)
                 {
                     main.ClassesList.Add(value);
                 }
             });
            });
        }

        /// <summary>
        /// Setting the chapter dropdown
        /// </summary>
        /// <param name="Class"></param>
        public void SetChaptersDropdown(string Class)
        {
            List<string> data = new List<string>();
            SetDropdown(Code.GetChapters, (ReturnData) =>
            {
                data.Add(ReturnData);
                //Debug.Log(ReturnData);
            }, (length) =>
            {
                RunUIThread(() =>
                {
                    main.ChaptersList.Clear();
                    main.ChaptersList.Add("None");
                    ChaptersDropdown.SelectedIndex = 0;
                    foreach (string value in data)
                    {
                        main.ChaptersList.Add(value);
                        //Debug.Log(value);
                    }
                });
            }, Class);
        }
        #endregion
        public void GetChapterDownloadableList(string Class, string Chapter)
        {
            int TotalDownloaded = 0;
            int TotalNeeded = 0;

            SetInformationText("");
            SetChooseButtonVisibility(false);
            DownloadRelated.DownloadManager.DownloadList.Clear();

            SetDropdown(Code.GetValuesInChapters, async (TypeOfValue) =>
            {
                //Debug.Log(ReturnData);
                string JSONData = RemoveInitialAndLast(await ConnectionManager.GetDownloadableData(Class, Chapter, TypeOfValue));

                DownloadRelated.Downloadable ZipFile = DownloadRelated.Downloadable.FromJson(JSONData);
                DownloadRelated.DownloadManager.DownloadList.Add(new DownloadRelated.ToDownload(Class, Chapter, TypeOfValue, ZipFile));
                Debug.Log(DownloadRelated.DownloadManager.DownloadList.Count+"");


                string InformationData = "Files to download: ";
                foreach (DownloadRelated.ToDownload toDownload in DownloadRelated.DownloadManager.DownloadList)
                {
                    InformationData += "(" + toDownload.CategoryName + ") ";
                    //Debug.Log(InformationData);
                    //toDownload.Downloadable.DisplayData();
                }

                SetInformationText(InformationData);

                TotalDownloaded++;
                if(TotalDownloaded == TotalNeeded)
                {
                    SetChooseButtonVisibility(true);
                }
                else
                {
                    SetChooseButtonVisibility(false);
                }
                /*DownloadRelated.ToDownload todownload = new DownloadRelated.ToDownload(TypeOfValue, JSONData);
                //We will process only if we have data inside
                int index = 1;
                foreach(DownloadRelated.Downloadable downloadable in todownload.downloadables)
                {
                    if (todownload.CategoryName == "Videos")
                    {
                        DownloadRelated.DownloadManager.DownloadAndSave(Class, Chapter, TypeOfValue, downloadable.DownloadableName, downloadable.Hyperlink, index, ". ");
                    }
                    index++;
                }*/
            }, async (length) =>
            {
                TotalNeeded = length;
                Debug.Log("Done Downloading List");
                //await Task.Delay(2000);
                //We will request the data for all downloadable data and stored it in a dictionary possibly
                //Loop won't be done in here

            }, Class, Chapter);
        }

        #region Dropdown Value Changed
        private void ClassesDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //We clear the Chapter dropdown as well here
            ClearChaptersDropdown();
            if (ClassesDropdown.SelectedIndex != 0)
            {
                //This is not None
                //So we refresh
                SetChaptersDropdown(ClassesDropdown.SelectedValue.ToString());
            }
            else
            {
                SetInformationText("");
                SetChooseButtonVisibility(false);
            }
        }
        private void ChaptersDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ChaptersDropdown.SelectedIndex != 0)
            {
                if (ChaptersDropdown.SelectedValue != null)
                {
                    GetChapterDownloadableList(ClassesDropdown.SelectedValue.ToString(), ChaptersDropdown.SelectedValue.ToString());
                }
            }
            else
            {
                SetInformationText("");
                SetChooseButtonVisibility(false);
            }
        }
        #endregion

        /// <summary>
        /// The thread to run UI actions on
        /// typically adding and removing items from the combo box and such
        /// </summary>
        /// <param name="task">The task to do inside the UI thread</param>
        public void RunUIThread(Action task)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                task.Invoke();
            });
        }

        public void ClearChaptersDropdown()
        {
            RunUIThread(()=>
            {
                main.ChaptersList.Clear();
            });
        }

        public string RemoveInitialAndLast(string value)
        {
            string newValue = value.Remove(0, 1);
            newValue = newValue.Remove(newValue.Length - 1, 1);
            return newValue;
        }

        public void SetInformationText(string newText)
        {
            RunUIThread(()=>
            {
                InformationText.Text = newText;
                UpdateLayout();
            });
        }

        public void SetChooseButtonVisibility(bool Visible)
        {
            RunUIThread(() => {
                if (Visible)
                {
                    ChooseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ChooseButton.Visibility = Visibility.Hidden;
                }
            });
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DownloadRelated.DownloadManager.isDownloading)
            {
                DownloadRelated.DownloadManager.DownloadAllFromList();
                /*
                Shared.ProgressBar progress = new Shared.ProgressBar();
                progress.Show();
                */
            }
            else
            {
                SetInformationText("Please wait for the download to finish");
            }
        }

        [Serializable]
        public class ResponseClass
        {
            public string value;
        }

    }

    /// <summary>
    /// View Model for this class specifically
    /// For all observable components
    /// </summary>
    public class DownloadClass_ViewModel
    {
        public ObservableCollection<string> ClassesList { get; set; }
        public ObservableCollection<string> ChaptersList { get; set; }
        public DownloadClass_ViewModel()
        {
            ClassesList = new ObservableCollection<string> { };
            ChaptersList = new ObservableCollection<string> { };
        }
    }    
}
