using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LDG.Demo
{
    public class FPS : MonoBehaviour
    {
        //public Text fpsText;
        public Text fpsText;

        private int frameCount = 0;
        private float timeSnapshot = 0.0f;

        // Use this for initialization
        void Start()
        {
            timeSnapshot = Time.time;
            frameCount = 0;
        }

        void UpdateFPS()
        {
            if ((Time.time - timeSnapshot) >= 1.0f)
            {
                if (fpsText) fpsText.text = frameCount.ToString() + " FPS";

                frameCount = 0;
                timeSnapshot = Time.time;
            }

            frameCount += 1;
        }

        private void Update()
        {
            UpdateFPS();
        }
    }
}