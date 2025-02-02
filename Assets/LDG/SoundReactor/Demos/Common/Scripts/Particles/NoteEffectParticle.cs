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

    [RequireComponent(typeof(ParticleSystem))]
    public class NoteEffectParticle : MonoBehaviourParticle
    {
        public struct Parameters
        {
            public Vector3 Position;
            public Transform Parent;
            public Color Color;
            public float LifeTime;
        }

        private ParticleSystem ps;

        new private void Awake()
        {
            base.Awake();

            ps = GetComponent<ParticleSystem>();
        }

        new void Update()
        {
            base.Update();
        }

        override public void Initialize(object parameters)
        {
            Parameters p = (Parameters)parameters;

            if (p.Parent)
            {
                transform.SetParent(p.Parent);
            }

            transform.position = p.Position;

            ParticleSystem.MainModule mm = ps.main;
            mm.startColor = p.Color;

            Emit(p.LifeTime);
        }
    }
}