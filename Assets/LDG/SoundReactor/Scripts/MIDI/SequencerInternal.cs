// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum PlayState { Playing, Play, Stop, End, Pause };
    public delegate void MidiEventHandler(Sequencer sequencer, MidiEvent e);

    /// <summary>
    /// Internal class used by MidiSequencer to keep track of midi tracks and events.
    /// </summary>
    public class SequencerInternal
    {
        #region Structs
        private struct NoteState
        {
            public bool NoteOn;
            public float NormalizedVelocity;
        }
        #endregion

        #region Events
        private event MidiEventHandler onMidiEvent;
        public event MidiEventHandler OnMidiEvent
        {
            add
            {
                onMidiEvent += value;
            }

            remove
            {
                onMidiEvent -= value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Current play state of the sequence.
        /// </summary>
        public PlayState PlayState { get { return playState; } protected set { playState = value; } }
        private PlayState playState = PlayState.End;

        /// <summary>
        /// Identifies whether the clip is currently playing or not.
        /// </summary>
        public bool IsPlaying { get { return isPlaying; } }
        private bool isPlaying = false;

        /// <summary>
        /// Identifies whether the clip is currently paused or not.
        /// </summary>
        public bool IsPaused { get { return isPaused; } }
        private bool isPaused = false;

        /// <summary>
        /// Ticks in microseconds since the MIDI started playing.
        /// </summary>
        public double Ticks { get { return ticks; } set { ticks = value; } }
        private double ticks = 0;

        /// <summary>
        /// Time in seconds since the midi started.
        /// </summary>
        public float Time { get { return time; } set { time = value; } }
        private float time = 0;

        /// <summary>
        /// Current note count.
        /// </summary>
        public int NoteCounter { get { return noteCounter; } set { noteCounter = value; } }
        private int noteCounter = 0;
        /// <summary>
        /// Index of the track being played.
        /// </summary>
        public short TrackIndex { get { return trackIndex; } }
        private short trackIndex = 0;

        /// <summary>
        /// MIDI clip this timeline is responsible for playing.
        /// </summary>
        public MidiClip Clip { get { return midiClip; } }
        private MidiClip midiClip;

        // sequencer responsible for playing this timeline
        public Sequencer Sequencer { get { return sequencer; } }
        private Sequencer sequencer;
        #endregion

        #region Fields
        /// <summary>
        /// Beats per second. A MIDI always starts with 2 beats per seconds, aka 120 beats per minute.
        /// </summary>
        public double Tempo = 2.0;

        /// <summary>
        /// Changes the playback speed. A value of 1 will play back at normal speed.
        /// </summary>
        public float PlaybackSpeed = 1.0f;

        /// <summary>
        /// Tell the clip to loop or not.
        /// </summary>
        public bool Loop = true;
        
        // stores the current event index (progress in time) for a given track. the sequencer will increment these as the clip
        // continues to play.
        private int[] currentEventIndex;
        
        // simple note information
        private Note[] notes;

        // state of a note. this is used to keep track of note offs so a velocity can be forced to 0 for
        // one single frame. this allows a Level to detect it as a beat.
        private NoteState[] noteStates;
        
        private bool endReached = false;
        private bool endReachedPrev = false;
        private bool isTimeSet = false;
        private float delay = 0.0f;
        #endregion

        #region Constructor
        public SequencerInternal(MidiClip midiClip, Sequencer sequencer)
        {
            this.sequencer = sequencer;
            notes = new Note[128];
            noteStates = new NoteState[128];
            
            this.midiClip = midiClip;
            currentEventIndex = new int[midiClip.numTracks];

            Reset();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Plays MIDI clip.
        /// </summary>
        public void Play()
        {
            PlayDelayed(0.0f);
        }

        /// <summary>
        /// Plays MIDI clip after a delay.
        /// </summary>
        public void PlayDelayed(float delay)
        {
            if (!isTimeSet)
            {
                if (!isPaused || !isPlaying)
                {
                    //SetState(PlayState.Stop);
                    Reset();
                }
            }

            isPlaying = true;
            isPaused = false;
            endReached = false;
            endReachedPrev = false;

            SetState(PlayState.Play);

            this.delay = delay;
        }

        /// <summary>
        /// Stops the MIDI clip and resets the time. This clears note velocities.
        /// </summary>
        public void Stop()
        {
            isPlaying = false;
            SetState(PlayState.Stop);
            Reset();
        }

        /// <summary>
        /// Pauses the MIDI clip and remembers the time. This does NOT clear note velocities.
        /// </summary>
        public void Pause()
        {
            isPaused = true;
            SetState(PlayState.Pause);
        }

        /// <summary>
        /// Resumes playing the MIDI clip from the time it was paused.
        /// </summary>
        public void UnPause()
        {
            isPaused = false;
            SetState(PlayState.Play);
        }

        /// <summary>
        /// Seeks to a specific time in a MIDI clip while raising events in handler if it's not null.
        /// </summary>
        public void Seek(float time, MidiEventHandler midiEventHandler = null)
        {
            isTimeSet = true;

            Reset();
            midiClip.Seek(this, time, midiEventHandler);
        }

        /// <summary>
        /// Gets the last note played. The note could come from any track.
        /// </summary>
        public Note GetMidiNote(float normalizedIndex)
        {
            if (notes == null || notes.Length == 0) return Note.Empty;
            
            return notes[Mathf.RoundToInt(normalizedIndex * (float)(notes.Length - 1))];
        }

        /// <summary>
        /// Gets the specified MIDI note from the track that last played that note.
        /// </summary>
        public Note GetMidiNote(int index)
        {
            return notes[index];
        }

        /// <summary>
        /// Gets all the MIDI note velocities that were last play on the MIDI tracks.
        /// </summary>
        public void GetNoteVelocities(float[] noteVelocities)
        {
            if (endReached)
            {
                for (int i = 0; i < notes.Length; i++)
                {
                    noteVelocities[i] = 0.0f;
                }
            }
            else
            {
                for (int i = 0; i < notes.Length; i++)
                {
                    noteVelocities[i] = notes[i].NormalizedVelocity;
                }
            }
        }

        /// <summary>
        /// Update the time and position of all the event indices.
        /// </summary>
        public void Update(float deltaTime, float speed, int noteOffset)
        {
            if (isPlaying && !isPaused && delay <= 0.0f)
            {
                isTimeSet = false;

                playState = PlayState.Playing;
                PlaybackSpeed = speed;

                if (!endReached)
                {
                    double ticksPerBeat = (double)midiClip.divisions * Tempo;
                    float inc = (deltaTime * speed);

                    time += inc;
                    ticks += ticksPerBeat * (double)inc;
                }
                else
                {
                    ticks = midiClip.ticks;
                    time = midiClip.length;
                }

                endReached = !midiClip.UpdateTracks(this, noteOffset);

                ProcVelocities(true);

                if (endReached)
                {
                    if(endReached != endReachedPrev) SetState(PlayState.End);

                    if (Loop)
                    {
                        Reset();
                        SetState(PlayState.Play);
                    }
                }

                endReachedPrev = endReached;
            }
            else
            {
                delay -= deltaTime;

                if (delay < 0.0f) delay = 0.0f;
            }
        }

        /// <summary>
        /// Reset note velocities.
        /// </summary>
        public void ResetVelocities()
        {
            for (int i = 0; i < notes.Length; i++)
            {
                notes[i].NormalizedVelocity = 0.0f;
                noteStates[i].NormalizedVelocity = 0.0f;
            }
        }

        /// <summary>
        /// Reset note states and note indices.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < currentEventIndex.Length; i++)
            {
                currentEventIndex[i] = 0;
            }

            ResetVelocities();

            Tempo = 2.0;  // DO NOT CHANGE. MIDI expects this to be 2 (beats per second) at the start of a MIDI sequence.
            ticks = 0;
            noteCounter = 0;
            trackIndex = 0;
            time = 0.0f;
            endReached = false;
            endReachedPrev = false;
        }

        public void SetMidiNoteData(short trackIndex, short noteIndex, byte velocity, float normalizedVelocity, float holdTime)
        {
            notes[noteIndex].TrackIndex = trackIndex;
            notes[noteIndex].NoteIndex = noteIndex;
            notes[noteIndex].NormalizedVelocity = normalizedVelocity;
            notes[noteIndex].Velocity = velocity;
            notes[noteIndex].HoldTime = holdTime;

            noteStates[noteIndex].NormalizedVelocity = normalizedVelocity;

            if (normalizedVelocity == 0.0f)
            {
                noteStates[noteIndex].NoteOn = false;
            }
            else
            {
                noteCounter++;
            }
        }

        public int GetCurrentEventIndex(short trackIndex)
        {
            this.trackIndex = trackIndex;
            return currentEventIndex[this.trackIndex];
        }

        public void SetCurrentEventIndex(short trackIndex, int eventIndex)
        {
            this.trackIndex = trackIndex;
            currentEventIndex[this.trackIndex] = eventIndex;
        }
        
        public void SetState(PlayState state)
        {
            playState = state;
            RaiseMidiEvent(MidiEvent.Empty);
        }
        
        public void RaiseMidiEvent(MidiEvent e)
        {
            if (onMidiEvent != null)
            {
                onMidiEvent(sequencer, e);
                playState = PlayState.Playing;
            }
        }
        #endregion

        #region Private Methods
        private void ProcVelocities(bool segmentNotes)
        {
            // segment notes. this forces a 0.0 velocity for at least one frame between consecutive notes. in other words,
            // consecutive notes usually end up being a non zero value. for example, if a note is being pressed repeatedly
            // without a delay, and at a velocity of 64, the velocity will be 64 every frame. it does get a note off or
            // note on with a value of 0.0, but it's immediately set back to 64 in the same frame, which doesn't give
            // Sound Reactor a chance to "react".
            if (segmentNotes)
            {
                for (int i = 0; i < noteStates.Length; i++)
                {
                    if (!noteStates[i].NoteOn)
                    {
                        noteStates[i].NoteOn = true;
                        notes[i].NormalizedVelocity = 0.0f;
                    }
                    else
                    {
                        notes[i].NormalizedVelocity = noteStates[i].NormalizedVelocity;
                    }
                }
            }
            else
            {
                for (int i = 0; i < noteStates.Length; i++)
                {
                    notes[i].NormalizedVelocity = noteStates[i].NormalizedVelocity;
                }
            }
        }
        #endregion
    }
}