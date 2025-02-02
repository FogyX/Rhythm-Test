// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

namespace LDG.SoundReactor
{
    /// <summary>
    /// Stores everything to do with time for each tempo meta message. See <see cref="Track.CreateTempoTimeMaps(MetaMessage[])"/>
    /// <para>This struct is only allocated when converting a .midi to a .midi.asset file (ScriptableObject).</para>
    /// </summary>
    public struct TempoRegion
    {
        public static readonly TempoRegion Default = new TempoRegion(0, 0, 0, 2, 0);

        /// <summary>
        /// Tempo in beats per second
        /// </summary>
        public double Tempo;

        /// <summary>
        /// Absolute time in seconds since the start of the MIDI sequence. This is the
        /// absolute time, i.e. time adjusted/scaled by the tempo.
        /// </summary>
        public double Time;

        /// <summary>
        /// Raw ticks grabbed straight from the MIDI file.
        /// </summary>
        public int Ticks;

        /// <summary>
        /// The duration in number of ticks for this region.
        /// </summary>
        public int DurationTicks;

        /// <summary>
        /// The duration in ticks since the last tempo meta message.
        /// 
        /// <para>Just like Ticks, this is the absolute unscaled time in microseconds.</para>
        /// 
        /// <para>A group is considered a series of events with delta times of zero followed by
        /// a single event that has a delta time greater than zero. Example: (0, 0, 0, 400)
        /// is a group containing 4 events.</para>
        /// </summary>
        public int DeltaTicks;

        /// <summary>
        /// Create a tempo region.
        /// </summary>
        /// <param name="ticks">Start position in ticks.</param>
        /// <param name="deltaTicks">Ticks since the last tempo change.</param>
        /// <param name="durationTicks">Ticks until the next tempo change.</param>
        /// <param name="tempo">Tempo in beats per second.</param>
        /// <param name="time">Start position in seconds.</param>
        public TempoRegion(int ticks, int deltaTicks, int durationTicks, double tempo, double time)
        {
            Tempo = tempo;
            Time = time;
            Ticks = ticks;
            DurationTicks = durationTicks;
            DeltaTicks = deltaTicks;
        }

        public double TicksToTime(double ticks, int divisions)
        {
            double seconds = (ticks - (double)Ticks) / Tempo / (double)divisions;
            
            return Time + seconds;
        }

        public double TimeToTicks(double time, int divisions)
        {
            double ticks = (time - (double)Time) * (double)divisions * Tempo;

            return (double)Ticks + ticks;
        }

        public bool Inside(int ticks)
        {
            return (ticks >= Ticks && ticks <= Ticks + DurationTicks);
        }
    }
}
