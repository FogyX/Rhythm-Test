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
    using LDG.Core;

    public class RhythmEmitter : MonoBehaviour
    {
        public MonoBehaviourParticlePool notePool;

        public Color[] noteColors = { Color.red, Color.green, Color.blue };

        public void Start()
        {
            // disable this game object if there isn't a rhythmNote to cache
            if (!notePool)
            {
                gameObject.SetActive(false);

                Debug.LogError("RhythmNote not set. Disabling RhythmEmitter.", gameObject);

                return;
            }
        }

        // assigned to, and called by, EventDriver
        public void OnLevel(PropertyDriver driver)
        {
            // abord if a rhythm note isn't attached
            if (!notePool) return;

            // emit a note if a beat is detected
            if (driver.isBeat)
            {
                // get our midi note
                Note midiNote = driver.GetMidiNote();

                // instantiate the note
                EmitNote((short)(midiNote.NoteIndex % noteColors.Length));
            }
        }

        private void EmitNote(short index)
        {
            RhythmNote rn;

            RhythmNote.Parameters p = new RhythmNote.Parameters
            {
                Parent = transform,
                TouchIndex = index,
                Color = noteColors[index],
                EmissionColor = noteColors[index],
            };

            rn = (RhythmNote)notePool.GetParticle();
            rn.Initialize(p);
        }
    }
}