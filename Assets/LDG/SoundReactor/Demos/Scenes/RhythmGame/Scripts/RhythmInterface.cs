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
using UnityEngine.UI;

namespace LDG.Demo
{
    using LDG.SoundReactor;

    [ExecuteInEditMode]
    public class RhythmInterface : MonoBehaviour
    {
        #region Properties
        public float audioStartDelay
        {
            get { return (audioMidiSync != null) ? audioMidiSync.AudioStartDelay : 0.0f; }
        }

        public MidiSource midiSource
        {
            get { return (audioMidiSync != null) ? audioMidiSync.MidiSource : null; }
        }

        public MidiClip midiClip
        {
            get { return (audioMidiSync != null) ? midiSource.clip : null; }
        }

        public float playbackSpeed
        {
            get { return (audioMidiSync != null) ? audioMidiSync.PlaybackSpeed : 1.0f; }
        }

        public int hitCounter { get; private set; }
        public int missedCounter { get; private set; }
        public int consecutiveCounter { get; private set; }
        #endregion

        #region Fields
        public AudioMidiSync audioMidiSync;
        public Transform track;

        [Tooltip("Length of the track notes slide along. This is also the position the note flies to in the animation attached to the note.")]
        public float trackLength = 46.0f;

        [Tooltip("Indicator for when a note should be tapped")]
        public Transform beatLine;
        public float beatLinePosition = 0.75f;

        [Tooltip("The window of time that a note can be tapped")]
        public float timingThreshold = 0.05f;

        public Text hitCounterText;
        public Text missedCounterText;
        public Text consecutiveCounterText;
        public Text completedText;
        #endregion

        #region MonoBehaviours
        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update()
        {
            if (hitCounterText) hitCounterText.text = hitCounter.ToString();
            if (missedCounterText) missedCounterText.text = missedCounter.ToString();
            if (consecutiveCounterText) consecutiveCounterText.text = consecutiveCounter.ToString();

            if (completedText)
            {
                if (midiClip)
                {
                    int completed = (int)(midiSource.time / midiClip.length * 100.0f);

                    completedText.text = string.Format("{0}%", completed);
                }
            }

            Vector3 pos = beatLine.localPosition;
            pos.y = trackLength * -beatLinePosition;
            beatLine.localPosition = pos;

            Vector3 scale = track.localScale;
            scale.y = trackLength;
            track.localScale = scale;
        }
        #endregion

        #region PUblic Methods
        public void Reset()
        {
            hitCounter = 0;
            missedCounter = 0;
            consecutiveCounter = 0;
        }

        public void IncrementHit()
        {
            hitCounter++;
            consecutiveCounter++;
        }

        public void IncrementMissed()
        {
            missedCounter++;
            consecutiveCounter = 0;
        }

        /// <summary>
        /// Handler that can be registered with MidiSource
        /// </summary>
        public void ResetCounters(Sequencer sequencer, MidiEvent e)
        {
            switch (sequencer.PlayState)
            {
                case PlayState.Play:
                    Reset();
                    break;
            }
        }
        #endregion
    }
}