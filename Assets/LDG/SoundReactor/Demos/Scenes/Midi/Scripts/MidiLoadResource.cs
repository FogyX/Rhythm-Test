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

    public class MidiLoadResource : MonoBehaviour
    {
        public bool resourcesFolder = true;
        public string midiResource;
        public string audioResource;

        public MidiSource midiSource;
        public AudioSource audioSource;

        // Use this for initialization
        void Start()
        {
            MidiClip midiClip;
            AudioClip audioClip;

            midiSource = (midiSource) ? midiSource : gameObject.GetComponent<MidiSource>();
            audioSource = (audioSource) ? audioSource : gameObject.GetComponent<AudioSource>();

            if (midiSource && audioSource)
            {
                // when set to true, pre-processed MIDI file assets will be loaded from a Resources folder.
                // when set to false, raw MIDI files can be loaded from accessible folders
                if (resourcesFolder)
                {
                    Debug.Log(midiResource);
                    midiClip = Resources.Load<MidiClip>(midiResource);
                }
                else
                {
                    midiClip = ScriptableObject.CreateInstance<MidiClip>();
                    midiClip.Read(midiResource);
                }

                midiSource.clip = midiClip;

                audioClip = Resources.Load<AudioClip>(audioResource);
                audioSource.clip = audioClip;

                audioSource.Play();
                midiSource.Play();
            }
        }
    }
}