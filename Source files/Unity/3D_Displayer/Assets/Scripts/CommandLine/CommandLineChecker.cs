using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace com.HtetAungHlaing
{
    public class CommandLineChecker : MonoBehaviour
    {
        //We will check if the command line includes the value we want it to include
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void CheckForCommandLine()
        {
            string[] CommandLineArgs = Environment.GetCommandLineArgs();

            string Code = "";
            try
            {
                Code = CommandLineArgs.Where(x => x.Contains("code")).ToList()[0];
            }catch(Exception e)
            {
                Application.Quit();
            }

            if(Code.Length == 0)
            {
                Application.Quit();
            }

            Code = Code.Split('=')[1];
        }
    }
}
