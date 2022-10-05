using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Vizuara.TeacherPortal.Pages
{
    /// <summary>
    /// The enum class for all types of access
    /// </summary>
    public enum Code
    {
        None,                   //None
        GetClasses,             //How many classes are available to be downloaded?
        GetChapters,            //How many chapters do we have in that specific class
        GetValuesInChapters,    //What types of values do we have in that chapter
        GetDownloadableData,    //Get the links to the downloadable data
    }
    /// <summary>
    /// This class is the container class for various data
    /// all the connection data will be accessed from here
    /// </summary>
    public class ConnectionData
    {
        public static readonly string MainDomain = "https://asia-south1-vizuara-offline.cloudfunctions.net/vizuara";
        public static readonly string Default = "";
    }

    /// <summary>
    /// Connection Manager will contain all the connection related data
    /// other classes can use connection manager to retrieve data
    /// </summary>
    public class ConnectionManager
    {
        public static readonly HttpClient client = new HttpClient();
        
        public async static Task<string> PostRequest(string MainDomain, string Path, FormUrlEncodedContent content)
        {
            try
            {
                //Debug.Log(await content.ReadAsStringAsync());
                HttpResponseMessage responseBody = await client.PostAsync(CombineLink(MainDomain, Path), content);
                string res = await responseBody.Content.ReadAsStringAsync();
                return RemoveExtraComma(res);

            }catch(HttpRequestException e)
            {
                Debug.Log("\n Exception Caught");
                Debug.Log("Message :"+ e.Message);
                return "Exception";
            }
        }

        public async static Task<string> GetData(FormUrlEncodedContent content)
        {
            return await PostRequest(ConnectionData.MainDomain, ConnectionData.Default, content);
        }

        public async static Task<string> GetClassesData()
        {
            BasePostData BPD = new BasePostData(Code.GetClasses);
            FormUrlEncodedContent content = new FormUrlEncodedContent(BPD.GetDictionary());
            return await GetData(content);
        }
        public async static Task<string> GetChaptersData(string Chapter)
        {
            ChapterPostData CPD = new ChapterPostData(Code.GetChapters, Chapter);
            //Debug.Log(BPD.ChapterValue + "Value");
            FormUrlEncodedContent content = new FormUrlEncodedContent(CPD.GetDictionary());
            return await GetData(content);
        }
        public async static Task<string> GetChapterDownloadableList(string Class, string Chapter)
        {
            ChapterDownloadableData CDD = new ChapterDownloadableData(Code.GetValuesInChapters, Class, Chapter);
            FormUrlEncodedContent content = new FormUrlEncodedContent(CDD.GetDictionary());
            return await GetData(content);
        }

        public async static Task<string> GetDownloadableData(string Class, string Chapter, string TypeOfData)
        {
            IndividualDownloadableData downloadableData = new IndividualDownloadableData(Code.GetDownloadableData, Class, Chapter,TypeOfData);
            FormUrlEncodedContent content = new FormUrlEncodedContent(downloadableData.GetDictionary());
            return await GetData(content);
        }
        public static string CombineLink(string Main, string path)
        {
            return Main + "/" + path;
        }

        public static string RemoveExtraComma(string CommaText)
        {
            return CommaText.Remove(CommaText.Length - 2, 1);
        }
        public class BasePostData
        {
            public Code type;
            public BasePostData(Code c)
            {
                type = c;
            }

            public virtual Dictionary<string, string> GetDictionary()
            {
                Dictionary<string,string> newValue = new Dictionary<string, string>(1);
                newValue.Add("type", ((int)type).ToString());
                return newValue;
            }
        }

        public class ChapterPostData : BasePostData
        {
            public string ClassValue;
            public ChapterPostData(Code c, string chapValue):base(c)
            {
                ClassValue = chapValue;
            }

            public override Dictionary<string, string> GetDictionary()
            {
                Dictionary<string, string> newValue = new Dictionary<string, string>(1);
                newValue.Add("type", ((int)type).ToString());
                newValue.Add("class", ClassValue);
                return newValue;
            }
        }

        public class ChapterDownloadableData : ChapterPostData
        {
            public string ChapterValue;

            public ChapterDownloadableData(Code c, string Class, string Chapter):base(c, Class)
            {
                ChapterValue = Chapter;
            }

            public override Dictionary<string, string> GetDictionary()
            {
                Dictionary<string, string> newValue = new Dictionary<string, string>(1);
                newValue.Add("type", ((int)type).ToString());
                newValue.Add("class", ClassValue);
                newValue.Add("chapter", ChapterValue);
                return newValue;
            }
        }

        public class IndividualDownloadableData : ChapterDownloadableData
        {
            public string typeOfData;
            public IndividualDownloadableData(Code c, string Class, string Chapter, string TypeOfData):base(c, Class, Chapter)
            {
                typeOfData = TypeOfData;
            }

            public override Dictionary<string, string> GetDictionary()
            {
                Dictionary<string, string> newValue = new Dictionary<string, string>(1);
                newValue.Add("type", ((int)type).ToString());
                newValue.Add("class", ClassValue);
                newValue.Add("chapter", ChapterValue);
                newValue.Add("typeofdata", typeOfData);
                return newValue;
            }
        }
    }
}
