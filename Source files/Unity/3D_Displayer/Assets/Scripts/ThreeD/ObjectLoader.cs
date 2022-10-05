using UnityEngine.UI;
using UnityEngine;
using Dummiesman;
using System.IO;
using System;
using System.Linq;

namespace com.HtetAungHlaing
{
    public class ObjectLoader : MonoBehaviour
    {
        private GameObject LoadedGameObject;
        [SerializeField]
        private GameObject GameObjectToReplaced;
        [SerializeField]
        private Text DisplayText;

        private void Awake()
        {
            //The problem with Command line arguments is that they cannot read properly
            //if there are spaces between them
            string Params = string.Join(" ", Environment.GetCommandLineArgs());
            string[] CommandLineArgs = Environment.GetCommandLineArgs();//Params.Split('\"');

            string ModelArgument = "";
            string TextureArgument = "";
            string ModelFilePath = "";
            string TextureFilePath = "";

            //DisplayText.text = CommandLineArgs[0];

            try
            {
                ModelArgument = CommandLineArgs.Where(x => x.Contains("ModelPath=")).ToList()[0];
            }
            catch(Exception e)
            {

            }
            try
            {
                TextureArgument = CommandLineArgs.Where(x => x.Contains("TexturePath=")).ToList()[0];
            }
            catch (Exception e)
            {

            }
            try
            {
                ModelFilePath = ModelArgument.Split('=')[1];
            }
            catch (Exception e)
            {

            }
            try
            {
                TextureFilePath = ModelArgument.Split('=')[1];
            }
            catch (Exception e)
            {

            }
            DisplayText.text = ModelFilePath;

            /*
            #if UNITY_EDITOR
            ModelFilePath = "C:\\EndGame FIles\\Assets\\MFPS\\Custom Models\\Weapons\\LODWeapons\\AK.obj";
            #endif
            */
            string DirectoryPath = Path.GetDirectoryName(ModelFilePath);
            string ModelName = Path.GetFileName(ModelFilePath);
            ModelName = ModelName.Split('.')[0];
            TextureFilePath = DirectoryPath + "\\" + ModelName + ".png";
            //DisplayText.text = TextureFilePath;

            LoadModel(ModelFilePath, TextureFilePath);
        }

        public void LoadModel(string ModelFilePath, string TextureFilePath)
        {

            if (LoadedGameObject != null)
            {
                Destroy(LoadedGameObject);
            }
            LoadedGameObject = new OBJLoader().Load(ModelFilePath);
            Texture2D texture = ImageLoader.LoadTexture(TextureFilePath);
            GameObjectToReplaced.transform.GetComponent<MeshFilter>().sharedMesh = LoadedGameObject.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
            GameObjectToReplaced.transform.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            Destroy(LoadedGameObject);

        }

        public void RunIndexOutOfExceptionTryCatch(Action MainTask, Action ExceptionTask)
        {
            try
            {
                MainTask.Invoke();
            }catch(IndexOutOfRangeException exception)
            {
                ExceptionTask.Invoke();
            }
        }
    }
}

