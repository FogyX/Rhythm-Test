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
    using LDG.SoundReactor;

    public class Earthquake : MonoBehaviour
    {
        public float radius = 1.0f;
        public Vector2 speed;

        private Vector2 sinCos;
        private Vector3 originalPos;

        // Use this for initialization
        void Start()
        {
            sinCos = Vector2.zero;

            originalPos = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 offset = Vector2.zero;

            sinCos.x += speed.x * Time.deltaTime;
            sinCos.y += speed.y * Time.deltaTime;

            offset.x = Mathf.Sin(sinCos.x) * radius;
            offset.y = Mathf.Sin(sinCos.y) * radius;

            transform.localPosition = new Vector3(offset.x + originalPos.x, offset.y + originalPos.y, originalPos.z);
        }

        public void OnLevel(PropertyDriver driver)
        {
            Vector3 level = driver.LevelVector();
            speed.x = level.x;
            speed.y = level.y;
            radius = level.z;
        }
    }
}