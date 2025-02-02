// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    //Sub-bass          20 to 60Hz
    //Bass              60 to 250Hz
    //Low midrange      250 to 500Hz
    //Midrange          500 to 2000Hz
    //Upper midrange    2000 to 4000Hz
    //Presence          4000 to 6000Hz
    //Brilliance        6000 to 20000Hz

    public enum LevelMode
    {
        Custom,
        Flat,
        LinearAscending,
        LinearDescending,
        SquareAscending,
        SquareDescending,
        Mute,
        Max,
    };

    public enum BandFilters
    {
        FullRange,
        SubBass,
        Bass,
        LowMid,
        Mid,
        UpperMid,
        Presence,
        Brilliance,
    };

    [DisallowMultipleComponent]
    public class EQ : SerializeableObject
    {
        public float[] levels = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public float[] bandFilters = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public float slope = 1.0f;
        public float offset = 1.0f;
        public float master = 1.0f;
        
        public BandFilters bandFilterOption = BandFilters.FullRange;
        public LevelMode levelShape = LevelMode.Flat;

        public float GetLevel(float linearizedFrequency, InterpolationMode interpolationMode)
        {
            float level = Spline.Tween(linearizedFrequency, levels, false, interpolationMode);
            float filter = Spline.Tween(linearizedFrequency, bandFilters, false, interpolationMode);

            // get level, scale, and raise the roof
            return level * filter * master;
        }
    }
}