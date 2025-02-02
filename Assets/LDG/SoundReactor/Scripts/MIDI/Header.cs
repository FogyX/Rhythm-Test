// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;

namespace LDG.SoundReactor
{
    [Serializable]
    public struct Header
    {
        #region Fields
        /// <summary>
        /// The format of this clip. (0 is one track only, 1 is multiple tracks, 2 is a combination of 1 and 2 (not supported))
        /// </summary>
        public short Format;

        /// <summary>
        /// Number of tracks (tracks that contain notes and/or tempo)
        /// </summary>
        public short NumTracks;

        /// <summary>
        /// Ticks per beat, AKA: Pulses Per Quater note (PPQ)
        /// </summary>
        public short Division;

        /// <summary>
        /// The length of the MIDI clip in ticks.
        /// </summary>
        public int Ticks;

        /// <summary>
        /// The length of the MIDI clip in seconds.
        /// </summary>
        public float Length;

        /// <summary>
        /// Number of notes in this MIDI clip.
        /// </summary>
        public int NoteCount;
        #endregion
    }
}