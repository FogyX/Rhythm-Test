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

    public class TouchEffect : MonoBehaviourParticle
    {
        public float LifeTime = 1.0f;

        private Animator animator;

        public struct Parameters
        {
            public Color TouchedColor;
            public Transform Parent;
        }

        public override void Initialize(object parameters)
        {
            Parameters p = (Parameters)parameters;

            Emit(LifeTime);
            SetSpriteMaterialColor(p.TouchedColor);
            transform.SetParent(p.Parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            animator.Play("Effect", 0, 0.0f);
        }

        new private void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }
    }
}