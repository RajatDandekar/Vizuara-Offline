using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.HtetAungHlaing
{
    public class Framerate : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 30;
        }
    }
}
