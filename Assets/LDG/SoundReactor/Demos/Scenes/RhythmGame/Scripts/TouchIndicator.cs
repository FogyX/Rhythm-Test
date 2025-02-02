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
    using LDG.Core;

    public class TouchIndicator : MonoBehaviour
    {
        public MonoBehaviourParticlePool particlePool;

        private Animator animator;


        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void DoTouched(Color touchedColor)
        {
            animator.SetTrigger("touched");

            TouchEffect.Parameters parameters = new TouchEffect.Parameters()
            {
                TouchedColor = touchedColor,
                Parent = transform
            };

            TouchEffect touchEffect = (TouchEffect)particlePool.GetParticle();
            touchEffect.Initialize(parameters);
        }

        public void DoMissed()
        {
            animator.SetTrigger("missed");
        }
    }
}