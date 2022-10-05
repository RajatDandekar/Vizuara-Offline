using UnityEngine;

namespace com.HtetAungHlaing
{
    public class DevLog : MonoBehaviour
    {
        public static void Log(object value)
        {
            #if UNITY_EDITOR
            Debug.Log((string)value);
            #endif
        }
    }
}
