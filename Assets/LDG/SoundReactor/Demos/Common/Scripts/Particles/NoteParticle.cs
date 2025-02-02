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

    public class NoteParticle : MonoBehaviourParticle
    {
        public struct Parameters
        {
            public Vector3 Velocity;
            public Vector3 Position;
            public Vector3 Size;
            public float LifeTime;
            public Transform Parent;
            public int Track;
        }

        private Vector3 velocity;

        new void Awake()
        {
            base.Awake();
        }

        new void Update()
        {
            base.Update();

            transform.localPosition += velocity * Time.deltaTime;
        }

        override public void Initialize(object parameters)
        {
            Parameters p = (Parameters)parameters;

            if (p.Parent)
            {
                transform.SetParent(p.Parent);
            }

            p.Position.y += (float)p.Track * 0.01f;

            transform.localPosition = p.Position;
            transform.localScale = p.Size;

            velocity = p.Velocity;

            Emit(p.LifeTime);
        }
    }
}