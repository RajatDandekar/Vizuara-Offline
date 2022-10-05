using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.ComponentModel;

/// <summary>
/// This script will contains everything other scripts will share to use or just external classes
/// </summary>
namespace Vizuara.TeacherPortal.Shared
{
    /// <summary>
    /// This class contains everything related to downloading stuffs from the server
    /// </summary>
    public class DownloadRelated
    {
        /// <summary>
        /// The individual downloadable data
        /// </summary>
        [Serializable]
        public class Downloadable
        {
            public string FileName;
            public string Hyperlink;
            public Downloadable()
            {

            }

            public Downloadable(string FileName, string Hyperlink)
            {
                this.FileName = FileName;
                this.Hyperlink = Hyperlink;
            }

            public void DisplayData()
            {
                Debug.Log("File Name : " + FileName + ", Need to download from: " + Hyperlink + "");
            }

            public static Downloadable FromJson(string Json)
            {
                return JsonConvert.DeserializeObject<Downloadable>(Json);
            }

            /*
            public static List<Downloadable> GenerateList(string JSONString)
            {
                List<Downloadable> downloadable = new List<Downloadable>();
                foreach (KeyValuePair<string, string[]> data in GenerateDictionary(JSONString))
                {
                    downloadable.Add(GenerateDownloadable(data.Value));
                }
                return downloadable;
            }

            public static Dictionary<string, string[]> GenerateDictionary(string JSONString)
            {
                Debug.Log(JSONString);
                return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(JSONString);
            }

            public static Downloadable GenerateDownloadable(string[] array)
            {
                return new Downloadable(array[1], array[0]);
            }*/
        }

        /// <summary>
        /// The container for the list of data to download
        /// </summary>
        public class ToDownload
        {
            public string Class;
            public string Chapter;
            public string CategoryName; //Name of the Category (3D Models, Videos, etc)
            public Downloadable Downloadable;

            public ToDownload(string Class, string Chapter, string CategoryName, Downloadable downloadable)
            {
                this.Class = Class;
                this.Chapter = Chapter;
                this.CategoryName = CategoryName;
                Downloadable = downloadable;
            }
        }

        /// <summary>
        /// The manager class to handle the way files are downloaded
        /// </summary>
        public static class DownloadManager
        {
            public static bool isDownloading = false;
            public static List<ToDownload> DownloadList = new List<ToDownload>();

            static Shared.ProgressBar ProgressBarWindow = new Shared.ProgressBar();
            static int Progress;
            static int TotalFolder;

            static List<double> DownloadPercentage;
            /// <summary>
            /// Download All Data from the DownloadList
            /// Download All data in the list at the same time
            /// </summary>
            public static void DownloadAllFromList()
            {
                try
                {
                    ProgressBarWindow.Show();
                }catch(InvalidOperationException e)
                {
                    ProgressBarWindow = new Shared.ProgressBar();
                    ProgressBarWindow.Show();
                }
                ChangeProgress(0);
                DownloadPercentage = new List<double>();

                isDownloading = true;
                Progress = 0;
                TotalFolder = 0;

                int index = 0;
                foreach (ToDownload toDownload in DownloadList)
                {
                    TotalFolder++;

                    DownloadPercentage.Add(0);

                    DownloadAndSave(toDownload, index);
                    index++;
                }
            }

            /// <summary>
            /// Download and Save the file (individual)
            /// </summary>
            /// <param name="todownload">ToDownload container class that contains all necessary information </param>
            public static void DownloadAndSave(ToDownload todownload, int index)
            {
                DownloadAndSave(todownload.Class, todownload.Chapter, todownload.CategoryName, todownload.Downloadable.FileName, todownload.Downloadable.Hyperlink, index, 0);
            }

            /// <summary>
            /// Fhe function for changing progress ui
            /// </summary>
            /// <param name="ProgressPercentage"></param>
            static void ChangeProgress(int ProgressPercentage)
            {
                UIThread.RunOnUIThread(() =>
                {
                    ProgressBarWindow.pbStatus.Value = ProgressPercentage;
                    ProgressBarWindow.ProgressBarText.Text = "Downloading (" + Progress + "out of " + TotalFolder + ")";
                });
            }
            /// <summary>
            /// Download data with specific information
            /// </summary>
            public static void DownloadAndSave(string Class, string Chapter, string ValueType, string FileName, string HyperLink, int Index, int FileSize)
            {
                ///The function to download client
                using (WebClient client = new WebClient())
                {
                    string downloadDirectory = LongFunctions.LongDirectory.GetWin32LongPath(GetDownloadDirectory(Class, Chapter, ValueType));
                    string downloadFileName = GetDownloadFileName(downloadDirectory, FileName);
                    LongFunctions.LongDirectory.CreateDirectory(downloadDirectory);

                    ///Download the file without blocking the thread
                    Debug.Log(Class+" "+Chapter+" "+ValueType+" "+HyperLink);
                    client.DownloadFileAsync(new Uri(HyperLink), downloadFileName);
                    client.DownloadFileCompleted += (e, Sender) =>
                    {
                        UIThread.RunOnUIThread(() =>
                        {
                            #region Check If Download Finished
                            Progress++;
                            if(Progress == TotalFolder)
                            {
                                isDownloading = false;
                                ProgressBarWindow.Close();
                            }
                            #endregion

                            FileRelated.ZipExtractor.ExtractZip(downloadFileName, downloadDirectory);
                            //ChangeProgress();
                        });
                    };

                    client.DownloadProgressChanged += (e, Sender) =>
                    {
                        isDownloading = true;

                        double MegabyteReceived = Sender.BytesReceived / 1000000.0;
                        double TotalMBtoDownload = Sender.TotalBytesToReceive / 1000000.0;
                        //if(TotalMBtoDownload)
                        DownloadPercentage[Index] = MegabyteReceived / TotalMBtoDownload;
                        //Debug.Log(DownloadPercentage[Index]+"");

                        double TotalDownloadPercentage = 0;
                        foreach(double value in DownloadPercentage)
                        {
                            //Debug.Log(value+"");
                            TotalDownloadPercentage += value;
                        }
                        TotalDownloadPercentage /= TotalFolder;
                        TotalDownloadPercentage *= 100;
                        //Debug.Log(TotalDownloadPercentage+"");

                        ChangeProgress((int)TotalDownloadPercentage);
                    };

                    
                }
            }

            public static string GetDownloadDirectory(string Class, string Chapter, string ValueType)
            {
                return Directory.GetCurrentDirectory() + "\\Data\\" + Class + "\\" + Chapter + "\\" + ValueType + "\\";
            }

            public static string GetDownloadFileName(string DownloadDirectory, string FileName)
            {
                return DownloadDirectory + " " + FileName + ".";
            }
        }
    }

    /// <summary>
    /// This class will contain everything related to managing files that are alreadt saced on the device
    /// </summary>
    public class FileRelated
    {
        /// <summary>
        /// The class for retrieving the files and folders from the storage
        /// </summary>
        public class FileManager
        {
            #region GetFilePath

            public static string GetWorkingDirectory()
            {
                return Directory.GetCurrentDirectory();
            }

            public static string GetTempFolderPath()
            {
                return GetWorkingDirectory() + "\\Temp";
            }
            public static string GetDataFilePath()
            {
                return GetWorkingDirectory() + "\\Data";
            }
            public static string GetRunnableFilePath(string Class, string Chapter, string type, string folder)
            {
                //We will try to find the exe file inside the folder
                List<string> DownloadFilesExe = GetItems(Class, Chapter, type + "\\" + folder);

                DownloadFilesExe = DownloadFilesExe.Where(x => x.Contains("exe") && !x.Contains("UnityCrash")).ToList();

                string FileName;
                if (DownloadFilesExe.Count > 0)
                {
                    FileName = DownloadFilesExe[0];
                }
                else
                {
                    FileName = "test.exe";
                }

                Debug.Log(FileName);
                return GetDataFilePath() + "\\" + Class + "\\" + Chapter + "\\" + type + "\\" + folder + "\\" + FileName;
            }
            public static string GetDirectoryPath(string Class, string Chapter, string type)
            {
                return GetDataFilePath() + "\\" + Class + "\\" + Chapter + "\\" + type;
            }

            public static List<string> GetExeFolders(string Class, string Chapter, string type)
            {
                return GetDirectories("\\" + Class + "\\" + Chapter + "\\" + type);
            }
            #endregion

            #region GetData
            public static bool isDataAvailable()
            {
                if (!Directory.Exists(GetDataFilePath())){
                    return false;
                }
                return true;
            }
            static List<string> GetDirectories(string path)
            {
                string[] directories;
                try
                {
                    directories= Directory.GetDirectories(GetDataFilePath() + path);
                }catch(DirectoryNotFoundException exception)
                {
                    Directory.CreateDirectory(GetDataFilePath() + path);
                    directories = Directory.GetDirectories(GetDataFilePath() + path);
                }
                List<string> returnValues = new List<string>();
                foreach(string directory in directories)
                {
                    returnValues.Add(Path.GetFileName(directory));
                }
                return returnValues;
            }
            public static List<string> GetDownloadedClasses()
            {
                return GetDirectories("");
            }
            public static List<string> GetChapters(string Class)
            {
                return GetDirectories("\\" + Class);
            }
            public static List<string> GetTypesOfData(string Class, string Chapter)
            {
                return GetDirectories("\\"+Class+"\\"+Chapter);
            }
            public static List<string> GetItems(string Class, string Chapter, string type)
            {
                List<string> Files = new List<string>();
                foreach(string FilePath in Directory.GetFiles(GetDirectoryPath(Class, Chapter, type)))
                {
                    Files.Add(Path.GetFileName(FilePath));
                }
                return Files;
                //return GetDirectories("\\" + Class + "\\" + Chapter + "\\" + type);
            }
            #endregion

        }

        /// <summary>
        /// This class will extract the downloaded files
        /// </summary>
        public static class ZipExtractor
        {
            public static void ExtractZip(string ZipFilePath, string directory)
            {
                Shared.ProgressBar pg = new Shared.ProgressBar();
                pg.Show();

                #region Local_Functions
                ///Change the percentage of the progress bar
                void ChangePercentage(int Percentage, string Text)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        pg.pbStatus.Value = Percentage;
                        pg.ProgressBarText.Text = Text;
                    });
                }
                string GetEntryPath (string director, string name)
                {
                    string value = director + name;
                    return LongFunctions.LongDirectory.GetWin32LongPath(value.Replace('/', '\\'));
                }
                string GetEntryDirectory(string director, string name)
                {
                    string value = director + name;
                    return LongFunctions.LongDirectory.GetWin32LongPath(Path.GetDirectoryName(value.Replace('/','\\')));
                }
                bool CheckIfIsDirectory(string name)
                {
                    //As the entry.FullName returns with a '/' at the end if it's a folder we check if we should treat the entry as a folder this way
                    if (name[name.Length-1]!='/')
                    {
                        //Treat it as a file
                        return false;
                    }
                    else
                    {
                        //Treat it as a folder
                        return true;
                    }
                }

                #endregion

                Task.Run(() =>
                {
                    #region Deletion Of Existing Directory
                    foreach(string ExistingDirectory in Directory.GetDirectories(directory))
                    {
                        Directory.Delete(ExistingDirectory, true);
                    }

                    #endregion
                    //extract zip file
                    using (ZipArchive archive = ZipFile.OpenRead(ZipFilePath))
                    {
                        int TotalNumberOfFiles = archive.Entries.Count;
                        int index = 1;

                        #region Extraction Of Files
                        try
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if(LongFunctions.LongDirectory.Exists(GetEntryPath(directory, entry.FullName)))
                                {
                                    LongFunctions.LongDirectory.Delete(GetEntryPath(directory, entry.FullName), true);
                                }

                                //What this try catch block does is that it tries to Extract the entry as a file
                                //If it's a folder it will throw IOException
                                //So we create a folder instead with that entry
                                #region InnerTryCatch_Block
                                Debug.Log(entry.FullName);
                                if (CheckIfIsDirectory(entry.FullName))
                                {
                                    //Debug.Log("Folder");
                                    //if its a folder we create a directory using the path
                                    LongFunctions.LongDirectory.CreateDirectory(GetEntryPath(directory, entry.FullName));
                                }
                                else
                                {
                                    //Debug.Log("File: "+entry.FullName);
                                    // if it's a file, we just extract into it
                                    try
                                    {
                                        Debug.Log(GetEntryPath(directory, entry.FullName));
                                        entry.ExtractToFile(GetEntryPath(directory, entry.FullName), true);
                                    }
                                    catch (DirectoryNotFoundException)
                                    {
                                        LongFunctions.LongDirectory.CreateDirectory(GetEntryDirectory(directory, entry.FullName));
                                        entry.ExtractToFile(GetEntryPath(directory, entry.FullName), true);
                                        Debug.Log("Not Found");
                                    }
                                    if (entry.FullName.Contains(".webm"))
                                    {
                                        //Debug.Log(entry.FullName);
                                        Security.EncryptFile(GetEntryPath(directory, entry.FullName), GetEntryPath(directory, index + "." + entry.FullName));
                                        LongFunctions.LongFile.Delete(GetEntryPath(directory, entry.FullName));
                                    }
                                    else if (entry.FullName.Contains(".vizdata"))
                                    {
                                        //Debug.Log(entry.FullName);
                                        Security.EncryptString(GetEntryPath(directory, entry.FullName));
                                    }
                                }
                                #endregion

                                #region Reporting Progress
                                int percent = (int)(100 * index / TotalNumberOfFiles);
                                ChangePercentage(percent, "Extracting files (" + index + "/" + TotalNumberOfFiles + ")");
                                index++;
                                #endregion
                            }
                        }catch(DirectoryNotFoundException e)
                        {
                            //Debug.Log("break");
                            ChangePercentage(50, "Extracting files ( Total Number of Files " + TotalNumberOfFiles + ")");
                            foreach (string ExistingDirectory in Directory.GetDirectories(directory))
                            {
                                LongFunctions.LongDirectory.Delete(ExistingDirectory, true);
                            }
                            LongFunctions.LongDirectory.CreateDirectory(directory);
                            ZipFile.ExtractToDirectory(ZipFilePath, directory);
                        }
                        #endregion
                        archive.Dispose();

                        //Finally we delete the zip file from the storage
                        #region Deletion of the zip file
                        if (LongFunctions.LongFile.Exists(ZipFilePath))
                        {
                            LongFunctions.LongFile.Delete(ZipFilePath);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                pg.Close();
                            });
                        }
                        #endregion
                    }
                });
            }
        }

        ///<summary>
        ///This class contains all the security features of this application
        /// </summary>
        public class Security
        {
            private static string SKey = "bbdih9xMZ7HVP6Tip7RY";
            private static string SaltKey = "wNnYuPxihG4t89VWQv4L";
            private static int Iterations = 2096; // Recommendation is >= 1000

            private static string publickey = "41931032";
            private static string secretkey = "58192345";
            public static void EncryptFile(string srcFilename, string destFilename)
            {
                var aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                var salt = Encoding.ASCII.GetBytes(SaltKey);
                var key = new Rfc2898DeriveBytes(SKey, salt, Iterations);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

                if (File.Exists(destFilename))
                {
                    File.Delete(destFilename);
                }
                using (var dest = new FileStream(destFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (var cryptoStream = new CryptoStream(dest, transform, CryptoStreamMode.Write))
                    {
                        using (var source = new FileStream(srcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            source.CopyTo(cryptoStream);
                        }
                    }
                }
            }

            public static void DecryptFile(string srcFilename, string destFilename)
            {
                var aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                var salt = Encoding.ASCII.GetBytes(SaltKey);
                var key = new Rfc2898DeriveBytes(SKey, salt, Iterations);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var dest = new FileStream(destFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (var cryptoStream = new CryptoStream(dest, transform, CryptoStreamMode.Write))
                    {
                        try
                        {
                            using (var source = new FileStream(srcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                source.CopyTo(cryptoStream);
                            }
                        }
                        catch (CryptographicException exception)
                        {
                            throw new ApplicationException("Decryption failed.", exception);
                        }
                    }
                }
            }

            public static void EncryptString(string FileName)
            {
                    string OriginalData, EncryptedData;
                using (StreamReader reader = new StreamReader(FileName))
                {
                    OriginalData = reader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(FileName,false))
                    {

                    byte[] secretkeyByte = { };
                    secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                    byte[] publickeybyte = { };
                    publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                    MemoryStream ms = null;
                    CryptoStream cs = null;
                    byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(OriginalData);
                    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                    {
                        ms = new MemoryStream();
                        cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                        cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                        cs.FlushFinalBlock();
                        EncryptedData = Convert.ToBase64String(ms.ToArray());
                    }

                    writer.Write(EncryptedData);
                    }
            }

            public static string DecryptString(string FileName)
            {
                string EncryptedData, DecryptedData;

                using(StreamReader reader = new StreamReader(FileName))
                {
                    EncryptedData = reader.ReadToEnd();
                }

                Debug.Log("Before Encryption : "+EncryptedData);

                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[EncryptedData.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(EncryptedData.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    DecryptedData = encoding.GetString(ms.ToArray());
                }
                return DecryptedData;
            }
        }

    }
    
    /// <summary>
    /// This summary will contain a static function to check for internet connection
    /// </summary>
    public class InternetConnection
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }

    /// <summary>
    /// The class containter for RunOnUIThread function
    /// many class use this
    /// </summary>
    public class UIThread
    {
        public static void RunOnUIThread(Action task)
        {
            Application.Current.Dispatcher.Invoke(()=>
            {
                task.Invoke();
            });
        }

        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    //Long Paths
    //Added to fix the issues with DirectoryNotFoundException with some files while Extracting
    /// <summary>
    /// Sources from https://stackoverflow.com/questions/5188527/how-to-deal-with-files-with-a-name-longer-than-259-characters
    /// </summary>
    public class LongFunctions
    {
        internal static class NativeMethods
        {
            internal const int FILE_ATTRIBUTE_ARCHIVE = 0x20;
            internal const int INVALID_FILE_ATTRIBUTES = -1;

            internal const int FILE_READ_DATA = 0x0001;
            internal const int FILE_WRITE_DATA = 0x0002;
            internal const int FILE_APPEND_DATA = 0x0004;
            internal const int FILE_READ_EA = 0x0008;
            internal const int FILE_WRITE_EA = 0x0010;

            internal const int FILE_READ_ATTRIBUTES = 0x0080;
            internal const int FILE_WRITE_ATTRIBUTES = 0x0100;

            internal const int FILE_SHARE_NONE = 0x00000000;
            internal const int FILE_SHARE_READ = 0x00000001;

            internal const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

            internal const long FILE_GENERIC_WRITE = STANDARD_RIGHTS_WRITE |
                                                        FILE_WRITE_DATA |
                                                        FILE_WRITE_ATTRIBUTES |
                                                        FILE_WRITE_EA |
                                                        FILE_APPEND_DATA |
                                                        SYNCHRONIZE;

            internal const long FILE_GENERIC_READ = STANDARD_RIGHTS_READ |
                                                    FILE_READ_DATA |
                                                    FILE_READ_ATTRIBUTES |
                                                    FILE_READ_EA |
                                                    SYNCHRONIZE;



            internal const long READ_CONTROL = 0x00020000L;
            internal const long STANDARD_RIGHTS_READ = READ_CONTROL;
            internal const long STANDARD_RIGHTS_WRITE = READ_CONTROL;

            internal const long SYNCHRONIZE = 0x00100000L;

            internal const int CREATE_NEW = 1;
            internal const int CREATE_ALWAYS = 2;
            internal const int OPEN_EXISTING = 3;

            internal const int MAX_PATH = 260;
            internal const int MAX_ALTERNATE = 14;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct WIN32_FIND_DATA
            {
                public System.IO.FileAttributes dwFileAttributes;
                public FILETIME ftCreationTime;
                public FILETIME ftLastAccessTime;
                public FILETIME ftLastWriteTime;
                public uint nFileSizeHigh; //changed all to uint, otherwise you run into unexpected overflow
                public uint nFileSizeLow;  //|
                public uint dwReserved0;   //|
                public uint dwReserved1;   //v
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
                public string cAlternate;
            }


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool CopyFileW(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int GetFileAttributesW(string lpFileName);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool DeleteFileW(string lpFileName);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool MoveFileW(string lpExistingFileName, string lpNewFileName);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool GetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);


            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);


            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool FindClose(IntPtr hFindFile);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool RemoveDirectory(string path);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int SetFileAttributesW(string lpFileName, int fileAttributes);
        }

        public static class LongFile
        {
            private const int MAX_PATH = 260;

            public static bool Exists(string path)
            {
                if (path.Length < MAX_PATH) return System.IO.File.Exists(path);
                var attr = NativeMethods.GetFileAttributesW(GetWin32LongPath(path));
                return (attr != NativeMethods.INVALID_FILE_ATTRIBUTES && ((attr & NativeMethods.FILE_ATTRIBUTE_ARCHIVE) == NativeMethods.FILE_ATTRIBUTE_ARCHIVE));
            }

            public static void Delete(string path)
            {
                if (path.Length < MAX_PATH) System.IO.File.Delete(path);
                else
                {
                    bool ok = NativeMethods.DeleteFileW(GetWin32LongPath(path));
                    if (!ok) ThrowWin32Exception();
                }
            }

            public static void AppendAllText(string path, string contents)
            {
                AppendAllText(path, contents, Encoding.Default);
            }

            public static void AppendAllText(string path, string contents, Encoding encoding)
            {
                if (path.Length < MAX_PATH)
                {
                    System.IO.File.AppendAllText(path, contents, encoding);
                }
                else
                {
                    var fileHandle = CreateFileForAppend(GetWin32LongPath(path));
                    using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                    {
                        var bytes = encoding.GetBytes(contents);
                        fs.Position = fs.Length;
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            public static void WriteAllText(string path, string contents)
            {
                WriteAllText(path, contents, Encoding.Default);
            }

            public static void WriteAllText(string path, string contents, Encoding encoding)
            {
                if (path.Length < MAX_PATH)
                {
                    System.IO.File.WriteAllText(path, contents, encoding);
                }
                else
                {
                    var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

                    using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                    {
                        var bytes = encoding.GetBytes(contents);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            public static void WriteAllBytes(string path, byte[] bytes)
            {
                if (path.Length < MAX_PATH)
                {
                    System.IO.File.WriteAllBytes(path, bytes);
                }
                else
                {
                    var fileHandle = CreateFileForWrite(GetWin32LongPath(path));

                    using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            public static void Copy(string sourceFileName, string destFileName)
            {
                Copy(sourceFileName, destFileName, false);
            }

            public static void Copy(string sourceFileName, string destFileName, bool overwrite)
            {
                if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Copy(sourceFileName, destFileName, overwrite);
                else
                {
                    var ok = NativeMethods.CopyFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName), !overwrite);
                    if (!ok) ThrowWin32Exception();
                }
            }

            public static void Move(string sourceFileName, string destFileName)
            {
                if (sourceFileName.Length < MAX_PATH && (destFileName.Length < MAX_PATH)) System.IO.File.Move(sourceFileName, destFileName);
                else
                {
                    var ok = NativeMethods.MoveFileW(GetWin32LongPath(sourceFileName), GetWin32LongPath(destFileName));
                    if (!ok) ThrowWin32Exception();
                }
            }

            public static string ReadAllText(string path)
            {
                return ReadAllText(path, Encoding.Default);
            }

            public static string ReadAllText(string path, Encoding encoding)
            {
                if (path.Length < MAX_PATH) { return System.IO.File.ReadAllText(path, encoding); }
                var fileHandle = GetFileHandle(GetWin32LongPath(path));

                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
                {
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    return encoding.GetString(data);
                }
            }

            public static string[] ReadAllLines(string path)
            {
                return ReadAllLines(path, Encoding.Default);
            }

            public static string[] ReadAllLines(string path, Encoding encoding)
            {
                if (path.Length < MAX_PATH) { return System.IO.File.ReadAllLines(path, encoding); }
                var fileHandle = GetFileHandle(GetWin32LongPath(path));

                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
                {
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    var str = encoding.GetString(data);
                    if (str.Contains("\r")) return str.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    return str.Split('\n');
                }
            }
            public static byte[] ReadAllBytes(string path)
            {
                if (path.Length < MAX_PATH) return System.IO.File.ReadAllBytes(path);
                var fileHandle = GetFileHandle(GetWin32LongPath(path));

                using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read))
                {
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    return data;
                }
            }


            public static void SetAttributes(string path, FileAttributes attributes)
            {
                if (path.Length < MAX_PATH)
                {
                    System.IO.File.SetAttributes(path, attributes);
                }
                else
                {
                    var longFilename = GetWin32LongPath(path);
                    NativeMethods.SetFileAttributesW(longFilename, (int)attributes);
                }
            }

            #region Helper methods

            private static SafeFileHandle CreateFileForWrite(string filename)
            {
                if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
                SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.CREATE_ALWAYS, 0, IntPtr.Zero);
                if (hfile.IsInvalid) ThrowWin32Exception();
                return hfile;
            }

            private static SafeFileHandle CreateFileForAppend(string filename)
            {
                if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
                SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.CREATE_NEW, 0, IntPtr.Zero);
                if (hfile.IsInvalid)
                {
                    hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_WRITE, NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                    if (hfile.IsInvalid) ThrowWin32Exception();
                }
                return hfile;
            }

            internal static SafeFileHandle GetFileHandle(string filename)
            {
                if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
                SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)NativeMethods.FILE_GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                if (hfile.IsInvalid) ThrowWin32Exception();
                return hfile;
            }

            internal static SafeFileHandle GetFileHandleWithWrite(string filename)
            {
                if (filename.Length >= MAX_PATH) filename = GetWin32LongPath(filename);
                SafeFileHandle hfile = NativeMethods.CreateFile(filename, (int)(NativeMethods.FILE_GENERIC_READ | NativeMethods.FILE_GENERIC_WRITE | NativeMethods.FILE_WRITE_ATTRIBUTES), NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                if (hfile.IsInvalid) ThrowWin32Exception();
                return hfile;
            }

            public static System.IO.FileStream GetFileStream(string filename, FileAccess access = FileAccess.Read)
            {
                var longFilename = GetWin32LongPath(filename);
                SafeFileHandle hfile;
                if (access == FileAccess.Write)
                {
                    hfile = NativeMethods.CreateFile(longFilename, (int)(NativeMethods.FILE_GENERIC_READ | NativeMethods.FILE_GENERIC_WRITE | NativeMethods.FILE_WRITE_ATTRIBUTES), NativeMethods.FILE_SHARE_NONE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                }
                else
                {
                    hfile = NativeMethods.CreateFile(longFilename, (int)NativeMethods.FILE_GENERIC_READ, NativeMethods.FILE_SHARE_READ, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                }

                if (hfile.IsInvalid) ThrowWin32Exception();

                return new System.IO.FileStream(hfile, access);
            }


            [DebuggerStepThrough]
            public static void ThrowWin32Exception()
            {
                int code = Marshal.GetLastWin32Error();
                if (code != 0)
                {
                    throw new System.ComponentModel.Win32Exception(code);
                }
            }

            public static string GetWin32LongPath(string path)
            {
                if (path.StartsWith(@"\\?\")) return path;

                if (path.StartsWith("\\"))
                {
                    path = @"\\?\UNC\" + path.Substring(2);
                }
                else if (path.Contains(":"))
                {
                    path = @"\\?\" + path;
                }
                else
                {
                    var currdir = Environment.CurrentDirectory;
                    path = Combine(currdir, path);
                    while (path.Contains("\\.\\")) path = path.Replace("\\.\\", "\\");
                    path = @"\\?\" + path;
                }
                return path.TrimEnd('.'); ;
            }

            private static string Combine(string path1, string path2)
            {
                return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.'); ;
            }


            #endregion

            public static void SetCreationTime(string path, DateTime creationTime)
            {
                long cTime = 0;
                long aTime = 0;
                long wTime = 0;

                using (var handle = GetFileHandleWithWrite(path))
                {
                    NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);
                    var fileTime = creationTime.ToFileTimeUtc();
                    if (!NativeMethods.SetFileTime(handle, ref fileTime, ref aTime, ref wTime))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            public static void SetLastAccessTime(string path, DateTime lastAccessTime)
            {
                long cTime = 0;
                long aTime = 0;
                long wTime = 0;

                using (var handle = GetFileHandleWithWrite(path))
                {
                    NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                    var fileTime = lastAccessTime.ToFileTimeUtc();
                    if (!NativeMethods.SetFileTime(handle, ref cTime, ref fileTime, ref wTime))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            public static void SetLastWriteTime(string path, DateTime lastWriteTime)
            {
                long cTime = 0;
                long aTime = 0;
                long wTime = 0;

                using (var handle = GetFileHandleWithWrite(path))
                {
                    NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                    var fileTime = lastWriteTime.ToFileTimeUtc();
                    if (!NativeMethods.SetFileTime(handle, ref cTime, ref aTime, ref fileTime))
                    {
                        throw new Win32Exception();
                    }
                }
            }

            public static DateTime GetLastWriteTime(string path)
            {
                long cTime = 0;
                long aTime = 0;
                long wTime = 0;

                using (var handle = GetFileHandleWithWrite(path))
                {
                    NativeMethods.GetFileTime(handle, ref cTime, ref aTime, ref wTime);

                    return DateTime.FromFileTimeUtc(wTime);
                }
            }

        }

        public class LongDirectory
        {
            private const int MAX_PATH = 260;

            public static void CreateDirectory(string path)
            {
                if (string.IsNullOrWhiteSpace(path)) return;
                if (path.Length < MAX_PATH)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                else
                {
                    var paths = GetAllPathsFromPath(GetWin32LongPath(path));
                    foreach (var item in paths)
                    {
                        if (!LongExists(item))
                        {
                            var ok = NativeMethods.CreateDirectory(item, IntPtr.Zero);
                            if (!ok)
                            {
                                ThrowWin32Exception();
                            }
                        }
                    }
                }
            }

            public static void Delete(string path)
            {
                Delete(path, false);
            }

            public static void Delete(string path, bool recursive)
            {
                if (path.Length < MAX_PATH && !recursive)
                {
                    System.IO.Directory.Delete(path, false);
                }
                else
                {
                    if (!recursive)
                    {
                        bool ok = NativeMethods.RemoveDirectory(GetWin32LongPath(path));
                        if (!ok) ThrowWin32Exception();
                    }
                    else
                    {
                        DeleteDirectories(new string[] { GetWin32LongPath(path) });
                    }
                }
            }


            private static void DeleteDirectories(string[] directories)
            {
                foreach (string directory in directories)
                {
                    string[] files = LongDirectory.GetFiles(directory, null, System.IO.SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        LongFile.Delete(file);
                    }
                    directories = LongDirectory.GetDirectories(directory, null, System.IO.SearchOption.TopDirectoryOnly);
                    DeleteDirectories(directories);
                    bool ok = NativeMethods.RemoveDirectory(GetWin32LongPath(directory));
                    if (!ok) ThrowWin32Exception();
                }
            }

            public static bool Exists(string path)
            {
                if (path.Length < MAX_PATH) return System.IO.Directory.Exists(path);
                return LongExists(GetWin32LongPath(path));
            }

            private static bool LongExists(string path)
            {
                var attr = NativeMethods.GetFileAttributesW(path);
                return (attr != NativeMethods.INVALID_FILE_ATTRIBUTES && ((attr & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) == NativeMethods.FILE_ATTRIBUTE_DIRECTORY));
            }


            public static string[] GetDirectories(string path)
            {
                return GetDirectories(path, null, SearchOption.TopDirectoryOnly);
            }

            public static string[] GetDirectories(string path, string searchPattern)
            {
                return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
            }

            public static string[] GetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption)
            {
                searchPattern = searchPattern ?? "*";
                var dirs = new List<string>();
                InternalGetDirectories(path, searchPattern, searchOption, ref dirs);
                return dirs.ToArray();
            }

            private static void InternalGetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption, ref List<string> dirs)
            {
                NativeMethods.WIN32_FIND_DATA findData;
                IntPtr findHandle = NativeMethods.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(path), searchPattern), out findData);

                try
                {
                    if (findHandle != new IntPtr(-1))
                    {

                        do
                        {
                            if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) != 0)
                            {
                                if (findData.cFileName != "." && findData.cFileName != "..")
                                {
                                    string subdirectory = System.IO.Path.Combine(path, findData.cFileName);
                                    dirs.Add(GetCleanPath(subdirectory));
                                    if (searchOption == SearchOption.AllDirectories)
                                    {
                                        InternalGetDirectories(subdirectory, searchPattern, searchOption, ref dirs);
                                    }
                                }
                            }
                        } while (NativeMethods.FindNextFile(findHandle, out findData));
                        NativeMethods.FindClose(findHandle);
                    }
                    else
                    {
                        ThrowWin32Exception();
                    }
                }
                catch (Exception)
                {
                    NativeMethods.FindClose(findHandle);
                    throw;
                }
            }

            public static string[] GetFiles(string path)
            {
                return GetFiles(path, null, SearchOption.TopDirectoryOnly);
            }

            public static string[] GetFiles(string path, string searchPattern)
            {
                return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
            }


            public static string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)
            {
                searchPattern = searchPattern ?? "*";

                var files = new List<string>();
                var dirs = new List<string> { path };

                if (searchOption == SearchOption.AllDirectories)
                {
                    //Add all the subpaths
                    dirs.AddRange(LongDirectory.GetDirectories(path, null, SearchOption.AllDirectories));
                }

                foreach (var dir in dirs)
                {
                    NativeMethods.WIN32_FIND_DATA findData;
                    IntPtr findHandle = NativeMethods.FindFirstFile(System.IO.Path.Combine(GetWin32LongPath(dir), searchPattern), out findData);

                    try
                    {
                        if (findHandle != new IntPtr(-1))
                        {

                            do
                            {
                                if ((findData.dwFileAttributes & System.IO.FileAttributes.Directory) == 0)
                                {
                                    string filename = System.IO.Path.Combine(dir, findData.cFileName);
                                    files.Add(GetCleanPath(filename));
                                }
                            } while (NativeMethods.FindNextFile(findHandle, out findData));
                            NativeMethods.FindClose(findHandle);
                        }
                    }
                    catch (Exception)
                    {
                        NativeMethods.FindClose(findHandle);
                        throw;
                    }
                }

                return files.ToArray();
            }



            public static void Move(string sourceDirName, string destDirName)
            {
                if (sourceDirName.Length < MAX_PATH || destDirName.Length < MAX_PATH)
                {
                    System.IO.Directory.Move(sourceDirName, destDirName);
                }
                else
                {
                    var ok = NativeMethods.MoveFileW(GetWin32LongPath(sourceDirName), GetWin32LongPath(destDirName));
                    if (!ok) ThrowWin32Exception();
                }
            }

            #region Helper methods



            [DebuggerStepThrough]
            public static void ThrowWin32Exception()
            {
                int code = Marshal.GetLastWin32Error();
                if (code != 0)
                {
                    throw new System.ComponentModel.Win32Exception(code);
                }
            }

            public static string GetWin32LongPath(string path)
            {

                if (path.StartsWith(@"\\?\")) return path;

                var newpath = path;
                if (newpath.StartsWith("\\"))
                {
                    newpath = @"\\?\UNC\" + newpath.Substring(2);
                }
                else if (newpath.Contains(":"))
                {
                    newpath = @"\\?\" + newpath;
                }
                else
                {
                    var currdir = Environment.CurrentDirectory;
                    newpath = Combine(currdir, newpath);
                    while (newpath.Contains("\\.\\")) newpath = newpath.Replace("\\.\\", "\\");
                    newpath = @"\\?\" + newpath;
                }
                return newpath.TrimEnd('.');
            }

            private static string GetCleanPath(string path)
            {
                if (path.StartsWith(@"\\?\UNC\")) return @"\\" + path.Substring(8);
                if (path.StartsWith(@"\\?\")) return path.Substring(4);
                return path;
            }

            private static List<string> GetAllPathsFromPath(string path)
            {
                bool unc = false;
                var prefix = @"\\?\";
                if (path.StartsWith(prefix + @"UNC\"))
                {
                    prefix += @"UNC\";
                    unc = true;
                }
                var split = path.Split('\\');
                int i = unc ? 6 : 4;
                var list = new List<string>();
                var txt = "";

                for (int a = 0; a < i; a++)
                {
                    if (a > 0) txt += "\\";
                    txt += split[a];
                }
                for (; i < split.Length; i++)
                {
                    txt = Combine(txt, split[i]);
                    list.Add(txt);
                }

                return list;
            }

            private static string Combine(string path1, string path2)
            {
                return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.');
            }


            #endregion
        }
    }
}
