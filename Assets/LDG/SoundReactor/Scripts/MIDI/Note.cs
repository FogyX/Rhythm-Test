// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

namespace LDG.SoundReactor
{
    public struct Note
    {
        static readonly public Note Empty = new Note();

        public Note(MidiEvent me)
        {
            TrackIndex = me.TrackIndex;
            NoteIndex = me.Note;
            NormalizedVelocity = me.NormalizedVelocity;
            Velocity = me.Velocity;
            HoldTime = me.HoldTime;
            Ticks = me.Ticks;
            Time = me.Time;
        }

        #region Fields
        /// <summary>
        /// The index of the track this note came from.
        /// </summary>
        public short TrackIndex;

        /// <summary>
        /// The index of this note. This note can be offset by MidiSource->Note Offset.
        /// </summary>
        public short NoteIndex;

        /// <summary>
        /// The normalized velocity of the note [0.0, 1.0].
        /// </summary>
        public float NormalizedVelocity;

        /// <summary>
        /// The velocity of the note [0, 127]. Looking for the normalized version? It's now called NormalizedVelocity.
        /// </summary>
        public byte Velocity;

        /// <summary>
        /// Length of time in seconds the note is held down for for a given tempo stored in the MIDI file.
        /// </summary>
        public float HoldTime;

        /// <summary>
        /// Current ticks of this note.
        /// </summary>
        public int Ticks;

        /// <summary>
        /// Current time in seconds.
        /// </summary>
        public float Time;
        #endregion
    }
}