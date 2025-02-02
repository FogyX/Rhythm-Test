// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

namespace LDG.SoundReactor
{
    /// <summary>
    /// Front facing class that exposes SequenceInternal as read only, with the exception of the OnMidiEvent handler.
    /// </summary>
    public class Sequencer
    {
        #region Events
        public event MidiEventHandler OnMidiEvent
        {
            add
            {
                sequencerInternal.OnMidiEvent += value;
            }

            remove
            {
                sequencerInternal.OnMidiEvent -= value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Current play state of the sequence.
        /// </summary>
        public PlayState PlayState { get { return sequencerInternal.PlayState; } }

        /// <summary>
        /// Identifies whether the clip is currently playing or not.
        /// </summary>
        public bool IsPlaying { get { return sequencerInternal.IsPlaying; } }

        /// <summary>
        /// Identifies whether the clip is currently paused or not.
        /// </summary>
        public bool IsPaused { get { return sequencerInternal.IsPaused; } }

        /// <summary>
        /// Ticks in microseconds since the MIDI started playing.
        /// </summary>
        public double Ticks { get { return sequencerInternal.Ticks; } }

        /// <summary>
        /// Time in seconds since the MIDI started playing.
        /// </summary>
        public float Time
        {
            get
            {
                return sequencerInternal.Time;
            }

            set
            {
                if (value < sequencerInternal.Time)
                {
                    sequencerInternal.Seek(value, null);
                }
                else
                {
                    sequencerInternal.Time = value;
                }
            }
        }

        /// <summary>
        /// Current note count.
        /// </summary>
        public int NoteCounter { get { return sequencerInternal.NoteCounter; } }

        /// <summary>
        /// Index of the track being played.
        /// </summary>
        public short TrackIndex { get { return sequencerInternal.TrackIndex; } }

        /// <summary>
        /// The MidiClip being played.
        /// </summary>
        public MidiClip MidiClip { get { return sequencerInternal.Clip; } }

        /// <summary>
        /// Gets the current beats per second. This is 2 by default until it is changed by a MIDI tempo message.
        /// </summary>
        public double Tempo { get { return sequencerInternal.Tempo; } }

        /// <summary>
        /// Changes the playback speed. A value of 1 will play back at normal speed.
        /// </summary>
        public float PlaybackSpeed { get { return sequencerInternal.PlaybackSpeed; } set { sequencerInternal.PlaybackSpeed = value; } }

        /// <summary>
        /// Tell the clip to loop or not.
        /// </summary>
        public bool Loop { get { return sequencerInternal.Loop; } set { sequencerInternal.Loop = value; } }
        #endregion

        #region Fields
        private SequencerInternal sequencerInternal;
        #endregion

        #region Constructor
        public Sequencer(MidiClip midiClip)
        {
            sequencerInternal = new SequencerInternal(midiClip, this);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Plays MIDI clip.
        /// </summary>
        public void Play()
        {
            sequencerInternal.Play();
        }

        /// <summary>
        /// Plays MIDI clip after a delay.
        /// </summary>
        public void PlayDelayed(float delay)
        {
            sequencerInternal.PlayDelayed(delay);
        }

        /// <summary>
        /// Stops the MIDI clip and resets the time. This clears note velocities.
        /// </summary>
        public void Stop()
        {
            sequencerInternal.Stop();
        }

        /// <summary>
        /// Pauses the MIDI clip and remembers the time. This does NOT clear note velocities.
        /// </summary>
        public void Pause()
        {
            sequencerInternal.Pause();
        }

        /// <summary>
        /// Continues playing the MIDI clip.
        /// </summary>
        public void UnPause()
        {
            sequencerInternal.UnPause();
        }

        /// <summary>
        /// Seeks to a specific time in a MIDI clip while raising events in handler if it's not null.
        /// </summary>
        public void Seek(float time, MidiEventHandler midiEventHandler = null)
        {
            sequencerInternal.Seek(time, midiEventHandler);
        }

        /// <summary>
        /// Gets the specified MIDI note from the track that last played that note.
        /// </summary>
        public Note GetMidiNote(float normalizedIndex)
        {
            return sequencerInternal.GetMidiNote(normalizedIndex);
        }

        /// <summary>
        /// Gets the specified MIDI note from the track that last played that note.
        /// </summary>
        public Note GetMidiNote(int index)
        {
            return sequencerInternal.GetMidiNote(index);
        }

        /// <summary>
        /// Gets all the MIDI note velocities.
        /// </summary>
        public void GetNoteVelocities(float[] noteVelocities)
        {
            sequencerInternal.GetNoteVelocities(noteVelocities);
        }
       
        /// <summary>
        /// Update the time and position of all the event indices.
        /// </summary>
        public void Update(float deltaTime, float speed, int noteOffset)
        {
            sequencerInternal.Update(deltaTime, speed, noteOffset);
        }

        /// <summary>
        /// Reset note velocities.
        /// </summary>
        public void ResetVelocities()
        {
            sequencerInternal.ResetVelocities();
        }

        /// <summary>
        /// Reset note states and note indices.
        /// </summary>
        public void Reset()
        {
            sequencerInternal.Reset();
        }
        
        #endregion
    }
}