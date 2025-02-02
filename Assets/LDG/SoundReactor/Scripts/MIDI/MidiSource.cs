// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEngine.Events;

namespace LDG.SoundReactor
{
    public class MidiSource : MonoBehaviour
    {
        /// <summary>
        /// Event handler for handling MIDI events
        /// </summary>
        [System.Serializable]
        public class MidiEvent : UnityEvent<Sequencer, SoundReactor.MidiEvent> { }

        /// <summary>
        /// Returns true if the MIDI clip is playing.
        /// </summary>
        public bool isPlaying { get { return (sequencer != null) ? sequencer.IsPlaying : false; } }

        /// <summary>
        /// Identifies whether the clip is currently paused or not.
        /// </summary>
        public bool isPaused { get { return (sequencer != null) ? sequencer.IsPaused : false; } }

        /// <summary>
        /// Current time of the MIDI clip.
        /// </summary>
        public float time { get { return (sequencer != null) ? sequencer.Time : 0.0f; } set { if(sequencer != null) sequencer.Time = value; } }

        public bool usingExternalNotes { get { return (externalNotes != null); } }

        [SerializeField]
        private MidiClip _clip;

        /// <summary>
        /// The MIDI clip to be played.
        /// </summary>
        public MidiClip clip
        {
            get { return _clip; }

            set
            {
                if (value != _clip && value != null)
                {
                    sequencer = new Sequencer(value);
                    sequencer.OnMidiEvent += OnMidiEvent;
                }

                // don't let the sequencer exist if a clip doesn't
                if(value == null)
                {
                    sequencer = null;
                }
                
                _clip = value;
            }
        }

        [SerializeField]
        private int _noteOffset = 0;
        public int noteOffset
        {
            get { return _noteOffset; }

            set
            {
                if (value != _noteOffset)
                {
                    sequencer.ResetVelocities();
                }

                _noteOffset = value;
            }
        }

        public PlayState playState { get { return (sequencer != null) ? sequencer.PlayState : PlayState.Stop; } }

        /// <summary>
        /// Stops events from occuring, but the MIDI clip continues to play
        /// </summary>
        public bool mute = false;

        /// <summary>
        /// Playback speed
        /// </summary>
        public float speed = 1.0f;

        /// <summary>
        /// MIDI clip will automatically play if set to true
        /// </summary>
        public bool playOnAwake = true;

        /// <summary>
        /// MIDI clip will loop if set to true
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// MIDI event
        /// </summary>
        public MidiEvent onMidiEvent;

        private Sequencer sequencer;
        private bool awoken = false;

        private Note[] externalNotes = null;

#if UNITY_EDITOR
        private int noteOffsetOld = 0;
#endif

        private void Awake()
        {
            if (_clip)
            {
                sequencer = new Sequencer(_clip);
                sequencer.OnMidiEvent += OnMidiEvent;
            }
        }

        // Use this for initialization
        void Start()
        {
            awoken = true;

#if UNITY_EDITOR
            noteOffsetOld = noteOffset;
#endif
        }

        void OnMidiEvent(Sequencer sequencer, SoundReactor.MidiEvent e)
        {
            if (!mute)
            {
                onMidiEvent?.Invoke(sequencer, e);
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (noteOffsetOld != _noteOffset)
            {
                noteOffsetOld = _noteOffset;
                sequencer.ResetVelocities();
            }
#endif
            
            // update the sequencer if it's not null
            if (sequencer != null)
            {
                sequencer.Loop = loop;

                if (awoken && playOnAwake)
                {
                    Play();
                    awoken = false;
                }

                sequencer.Update(Time.deltaTime, speed, noteOffset);
            }
            else // create sequencer if a clip exists
            {
                if (_clip)
                {
                    sequencer = new Sequencer(_clip);
                    sequencer.OnMidiEvent += OnMidiEvent;
                }
            }
        }

        public Note GetMidiNote(float normalizedIndex)
        {
            if(externalNotes != null)
            {
                return externalNotes[Mathf.FloorToInt(normalizedIndex * 127.0f)];
            }
            else if (sequencer != null)
            {
                return sequencer.GetMidiNote(normalizedIndex);
            }

            return Note.Empty;
        }

        public void GetNoteVelocities(float[] noteVelocities)
        {
            if (externalNotes != null)
            {
                for (int i = 0; i < noteVelocities.Length; i++)
                {
                    noteVelocities[i] = externalNotes[i].NormalizedVelocity;
                }
            }
            else
            {
                // return 0.0 if muted, or the sequencer and the clip are null
                if (mute || (sequencer == null && _clip == null))
                {
                    System.Array.Clear(noteVelocities, 0, noteVelocities.Length);
                }
                else if (sequencer != null)
                {
                    sequencer.GetNoteVelocities(noteVelocities);
                }
            }
        }

        public void Pause()
        {
            if(sequencer != null)
            {
                sequencer.Pause();
            }
        }

        public void UnPause()
        {
            if (sequencer != null)
            {
                sequencer.UnPause();
            }
        }

        public void Play()
        {
            if (sequencer != null)
            {
                sequencer.Play();
            }
        }

        public void PlayDelayed(float delay)
        {
            if(sequencer != null)
            {
                sequencer.PlayDelayed(delay);
            }
        }

        public void Stop()
        {
            if (sequencer != null)
            {
                sequencer.UnPause();
                sequencer.Stop();
            }
        }

        /// <summary>
        /// Seeks to a specific time in a MIDI clip. Events are not raised.
        /// </summary>
        public void Seek(float time)
        {
            Seek(time, null);
        }

        /// <summary>
        /// Seeks to a specific time in a MIDI clip while raising events in handler if it's not null.
        /// </summary>
        public void Seek(float time, MidiEventHandler midiEventHandler)
        {
            if (sequencer != null)
            {
                sequencer.Seek(time, midiEventHandler);
            }
        }

        /// <summary>
        /// Provides a way for the application to override the midi sequencer with custom notes. Do this when the application
        /// needs to set note values instead of using a MidiClip. This is useful when creating a custom note editor that runs
        /// inside Unity.
        /// </summary>
        /// <param name="externalNotes">Pass in an array of 128 notes, otherwise an exception will be thrown.</param>
        public void SetExternalNotes(Note[] externalNotes)
        {
            if (externalNotes != null && externalNotes.Length != 128)
            {
                throw new System.Exception("The array size for external notes must be 128. Otherwise, set to null to restore normal functionality.");
            }

            this.externalNotes = externalNotes;
        }
    }
}