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
    using LDG.SoundReactor;

    [RequireComponent(typeof(NoteParticle))]
    public class NoteEffectParticleEmitter : MonoBehaviour
    {
        public MonoBehaviourParticlePool noteEffectPool;

        private void Awake()
        {
            // don't need to define this under the class because it's passed into the event handler
            NoteParticle noteParticle;
            noteParticle = GetComponent<NoteParticle>();
            noteParticle.OnEmit += OnEmit;
            noteParticle.OnExpire += OnExpire;
        }

        void OnEmit(MonoBehaviourParticle particle)
        {

        }

        void OnExpire(MonoBehaviourParticle particle)
        {
            if (!noteEffectPool) return;

            NoteEffectParticle.Parameters p = new NoteEffectParticle.Parameters
            {
                Color = particle.GetMaterialColor(),
                Position = gameObject.transform.position,
                LifeTime = 1.0f
            };

            NoteEffectParticle deadParticle = (NoteEffectParticle)noteEffectPool.GetParticle();

            deadParticle.Initialize(p);
        }

    }
}