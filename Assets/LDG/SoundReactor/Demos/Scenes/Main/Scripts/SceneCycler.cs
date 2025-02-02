/*
IMPORTANT:
  It is tempting to use this script directly, but don't! It has a habit of changing sometimes. The best thing to do
  if you want to use this script in your project is to duplicate it.
  
INSTRUCTIONS:
  Follow these steps to make a unique version of any script:
    1. Select the script and use Unity's duplicate command.
    2. Open the duplicate script and change its namespace to something other than what it is.
    3. The script is ready to be used!

  (Simply renaming the script will not work. When a script is created it is assigned an ID called a GUID, and that is
   what a scene references, not the name. When Unity duplicates the script it creates a new unique ID, making it
   impervious to SoundReactor updates.)
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace LDG.Demo
{
    public class SceneCycler : MonoBehaviour
    {
        public List<Object> scenes;
        public int startIndex = 0;

        private int sceneIndex;
        private int previousIndex;
        private int index;
        private AsyncOperation unloadOperation;
        private AsyncOperation loadOperation;
        private bool loading = true;

        private int GetIndex(int index)
        {
            return (int)Mathf.Repeat(index, scenes.Count);
        }

        // Use this for initialization
        void Start()
        {
            previousIndex = startIndex;
            sceneIndex = startIndex;

            StartCoroutine(LoadSceneAsync(scenes[sceneIndex].name, scenes[sceneIndex].name));
        }

        // Update is called once per frame
        void Update()
        {
            if (loading) return;

            bool keyPressed = false;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                previousIndex = GetIndex(index);
                index--;
                sceneIndex = GetIndex(index);
                keyPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                previousIndex = GetIndex(index);
                index++;
                sceneIndex = GetIndex(index);
                keyPressed = true;
            }

            if (keyPressed)
            {
                Debug.Log(sceneIndex);

                StartCoroutine(LoadSceneAsync(scenes[previousIndex].name, scenes[sceneIndex].name));
            }
        }

        IEnumerator LoadSceneAsync(string prevSceneName, string sceneName)
        {
            loading = true;

            // don't unload the previous scene if it's the same as the current one
            if (prevSceneName != sceneName)
            {
                unloadOperation = SceneManager.UnloadSceneAsync(prevSceneName);

                // Wait until the asynchronous scene fully loads
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }

            loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Wait until the asynchronous scene fully loads
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            loading = false;
        }
    }
}