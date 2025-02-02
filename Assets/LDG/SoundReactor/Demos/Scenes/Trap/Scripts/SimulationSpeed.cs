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

    public class SimulationSpeed : MonoBehaviour
    {
        ParticleSystem ps;

        // Use this for initialization
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void OnLevel(PropertyDriver driver)
        {
            float level = driver.LevelScalar();

            if (ps)
            {
#if UNITY_4_6
			ps.playbackSpeed = 1.0f + level;
#else
                ParticleSystem.MainModule module;
                module = ps.main;

                module.simulationSpeed = 1.0f + level;
#endif
            }
        }
    }
}