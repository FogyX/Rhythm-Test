using UnityEngine;

namespace LDG.Core.Prototype
{
    public class ApplicationSettings : MonoBehaviour
    {
        [Tooltip("Choose a key the user can press that will force the application to quit without prompting.")]
        public KeyCode forceQuiteKey = KeyCode.Escape;

        [Tooltip("user defined > 0, never sleep = -1, system defined = -2")]
        public float sleepTimeout = -1.0f;
        public int targetFramerate = 60;

        // Use this for initialization
        void Start()
        {
            Application.targetFrameRate = targetFramerate;


            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}