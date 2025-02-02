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

using UnityEngine;

namespace LDG.Demo
{
    public class SelfDestruct : MonoBehaviour
    {
        public float timer = 1.0f;
        public GameObject spawnObject;
        public Transform spawnParent;

        private float counter = 0.0f;

        // Use this for initialization
        void Start()
        {
            counter = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            counter += Time.deltaTime;

            if (counter > timer)
            {
                if (spawnObject)
                {
                    Instantiate(spawnObject, transform.position, spawnObject.transform.rotation, spawnParent);
                }

                Destroy(gameObject);
            }
        }
    }
}