// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum FrequencyBase { Audio, Midi }

    public enum BandOption
    {
        FullRange,
        StandardRanges,
        OneOctave,
        OneHalfOctave,
        OneThirdOctave,
        OneSixthOctave,
        OneTwelvethOctave,
        A440
    }

    public struct FrequencyTransform
    {
        readonly public float repeat;
        readonly public bool clamp;
        readonly public bool alternate;
        readonly public bool reverse;
        readonly public bool flipLevel;

        public FrequencyTransform(float repeat, bool clamp, bool alternate, bool reverse, bool flipLevel)
        {
            this.repeat = repeat;
            this.clamp = clamp;
            this.alternate = alternate;
            this.reverse = reverse;
            this.flipLevel = flipLevel;
        }
    }

    public struct Octaves
    {
        public const float One = 2.0f;
        public const float OneHalf = 1.41421f;          // 2^(1/2) = 1.41421f
        public const float OneThird = 1.2599f;          // 2^(1/3) = 1.25992f
        public const float OneSixth = 1.2246f;          // 2^(1/6) = 1.12246f
        public const float OneTwelveth = 1.05946f;      // 2^(1/12) = 1.05946f
        public const float OneTwentyFourth = 1.02930f;  // 2^(1/24) = 1.02930f
    }

    public enum FrequencyRangeOption
    {
        Custom,
        FullRange,
        SubBass,
        Bass,
        LowMidrange,
        Midrange,
        HighMidrange,
        Presence,
        Brilliance,
    }

    public enum MidiRangeOption
    {
        Custom,
        Key25,
        Key49,
        Key61,
        Key73,
        Key76,
        Key88,
        Key112,
        Key128,
    }

    public struct FrequencyRange
    {
        public FrequencyRange(float lower, float upper)
        {
            this.lower = lower;
            this.upper = upper;
        }

        readonly public float lower;
        readonly public float upper;
    }

    public struct Frequency
    {
        // MIDI
        readonly public static float LowestMidiFrequency = 8.18f;
        readonly public static float HighestMidiFrequency = 12543.85f;

        // A440
        readonly public static float LowestAudibleFrequency = 19.44544f;
        readonly public static float HighestAudibleFrequency = 19912.13f;
        
        public static float lowerBaseFrequency { get { return _lowerBaseFrequency; } }
        public static float upperBaseFrequency { get { return _upperBaseFrequency; } }

        private static float _lowerBaseFrequency = LowestAudibleFrequency;
        private static float _upperBaseFrequency = HighestAudibleFrequency;

        // the minimum and maximum frequency for all the following ranges must fall within 19.68627 and 19912.13.
        readonly private static float[][] Bands =
        {
            new float[]{ 20f, 20000f },
            new float[]{ 20f, 60.0f, 250.0f, 500.0f, 2000.0f, 4000.0f, 6000.0f, 20000f },
            new float[]{ 20f, 39.37254f, 78.74508f, 157.4902f, 314.9803f, 629.9606f, 1259.921f, 2519.843f, 5039.685f, 10079.37f, 20000f },
            new float[]{ 20f, 27.84059f, 39.37254f, 55.68118f, 78.74508f, 111.3624f, 157.4902f, 222.7247f, 314.9803f, 445.4494f, 629.9606f, 890.8989f, 1259.921f, 1781.798f, 2519.843f, 3563.595f, 5039.685f, 7127.191f, 10079.37f, 14254.38f, 20000f },
            new float[]{ 20f, 24.80314f, 31.25f, 39.37253f, 49.60628f, 62.5f, 78.74506f, 99.21256f, 125f, 157.4901f, 198.4251f, 250f, 314.9803f, 396.8503f, 500f, 629.9605f, 793.7005f, 1000f, 1259.921f, 1587.401f, 2000f, 2519.842f, 3174.802f, 4000f, 5039.684f, 6349.604f, 8000f, 10079.37f, 12699.21f, 16000f, 20000f },
            new float[]{ 20f, 22.09709f, 24.80314f, 27.84058f, 31.25f, 35.07693f, 39.37253f, 44.19417f, 49.60628f, 55.68116f, 62.49999f, 70.15387f, 78.74506f, 88.38834f, 99.21255f, 111.3623f, 125f, 140.3077f, 157.4901f, 176.7767f, 198.4251f, 222.7247f, 250f, 280.6155f, 314.9802f, 353.5533f, 396.8502f, 445.4493f, 499.9999f, 561.231f, 629.9604f, 707.1067f, 793.7004f, 890.8986f, 999.9999f, 1122.462f, 1259.921f, 1414.213f, 1587.401f, 1781.797f, 2000f, 2244.924f, 2519.842f, 2828.427f, 3174.802f, 3563.594f, 4000f, 4489.848f, 5039.684f, 5656.854f, 6349.604f, 7127.189f, 7999.999f, 8979.695f, 10079.37f, 11313.71f, 12699.21f, 14254.38f, 16000f, 17959.39f, 20000f },
            new float[]{ 20f, 20.85688f, 22.09709f, 23.41105f, 24.80315f, 26.27802f, 27.8406f, 29.49609f, 31.25002f, 33.10824f, 35.07696f, 37.16274f, 39.37255f, 41.71377f, 44.1942f, 46.82213f, 49.60632f, 52.55607f, 55.68122f, 58.9922f, 62.50006f, 66.21651f, 70.15395f, 74.32553f, 78.74516f, 83.4276f, 88.38847f, 93.64433f, 99.21271f, 105.1122f, 111.3625f, 117.9845f, 125.0002f, 132.4331f, 140.308f, 148.6512f, 157.4904f, 166.8553f, 176.777f, 187.2887f, 198.4255f, 210.2245f, 222.7251f, 235.9691f, 250.0005f, 264.8664f, 280.6161f, 297.3025f, 314.981f, 333.7108f, 353.5542f, 374.5777f, 396.8512f, 420.4492f, 445.4505f, 471.9384f, 500.0013f, 529.7329f, 561.2325f, 594.6051f, 629.9622f, 667.4218f, 707.1088f, 749.1557f, 793.7029f, 840.8989f, 890.9014f, 943.8772f, 1000.003f, 1059.466f, 1122.466f, 1189.211f, 1259.925f, 1334.844f, 1414.218f, 1498.312f, 1587.406f, 1681.799f, 1781.804f, 1887.755f, 2000.007f, 2118.934f, 2244.932f, 2378.423f, 2519.852f, 2669.69f, 2828.438f, 2996.626f, 3174.814f, 3363.599f, 3563.609f, 3775.512f, 4000.016f, 4237.87f, 4489.867f, 4756.849f, 5039.706f, 5339.382f, 5656.879f, 5993.255f, 6349.633f, 6727.202f, 7127.223f, 7551.03f, 8000.038f, 8475.745f, 8979.739f, 9513.703f, 10079.42f, 10678.77f, 11313.76f, 11986.52f, 12699.27f, 13454.41f, 14254.45f, 15102.07f, 16000.08f, 16951.5f, 17959.49f, 19027.42f, 20000f },

            // http://www.inspiredacoustics.com/en/MIDI_note_numbers_and_center_frequencies
            new float[]{ 20f, 20.60172f, 21.82676f, 23.12465f, 24.49972f, 25.95654f, 27.5f, 29.13523f, 30.86771f, 32.70319f, 34.64783f, 36.7081f, 38.89087f, 41.20344f, 43.65353f, 46.2493f, 48.99942f, 51.91309f, 55f, 58.27047f, 61.73542f, 65.40639f, 69.29565f, 73.4162f, 77.78175f, 82.40688f, 87.30706f, 92.4986f, 97.99885f, 103.8262f, 110f, 116.5409f, 123.4708f, 130.8128f, 138.5913f, 146.8324f, 155.5635f, 164.8138f, 174.6141f, 184.9972f, 195.9977f, 207.6523f, 220f, 233.0819f, 246.9417f, 261.6255f, 277.1826f, 293.6648f, 311.127f, 329.6276f, 349.2282f, 369.9944f, 391.9954f, 415.3047f, 440f, 466.1638f, 493.8833f, 523.2511f, 554.3653f, 587.3295f, 622.254f, 659.2551f, 698.4565f, 739.9888f, 783.9908f, 830.6094f, 880f, 932.3276f, 987.7666f, 1046.502f, 1108.731f, 1174.659f, 1244.508f, 1318.51f, 1396.913f, 1479.978f, 1567.982f, 1661.219f, 1760f, 1864.655f, 1975.533f, 2093.004f, 2217.461f, 2349.318f, 2489.016f, 2637.02f, 2793.826f, 2959.955f, 3135.963f, 3322.438f, 3520f, 3729.31f, 3951.067f, 4186.009f, 4434.922f, 4698.637f, 4978.032f, 5274.041f, 5587.652f, 5919.911f, 6271.926f, 6644.875f, 7040f, 7458.622f, 7902.132f, 8372.018f, 8869.845f, 9397.271f, 9956.063f, 10548.08f, 11175.3f, 11839.82f, 12543.86f, 13289.75f, 14080f, 14917.24f, 15804.26f, 16744.04f, 17739.69f, 18794.54f, 19912.13f, 20000f }
        };

        private static FrequencyRange[] frequencyBasePresets =
        {
            new FrequencyRange(19.44544f, 19912.13f),   // Audio Range
            new FrequencyRange(8.175798f, 12543.86f),   // MIDI Range
        };

        private static FrequencyRange[] audibleRangePresets =
        {
            new FrequencyRange(0, 0),                   // Placeholder for custom
            new FrequencyRange(20f, 20000f),            // Full Range
            new FrequencyRange(20, 60),                 // Sub-bass
            new FrequencyRange(60, 250),                // Bass
            new FrequencyRange(250, 500),               // Low Mid
            new FrequencyRange(500, 2000),              // Mid
            new FrequencyRange(2000, 4000),             // Upper Mid
            new FrequencyRange(4000, 6000),             // Presence
            new FrequencyRange(6000, 20000f),           // Brilliance
        };

        private static FrequencyRange[] midiRangePresets =
        {
            new FrequencyRange(0, 0),                   // Placeholder for custom
            new FrequencyRange(130.8128f, 523.2511f),   // 25 Keys
            new FrequencyRange(65.41f, 1046.5f),        // 49 Keys
            new FrequencyRange(65.41f, 2093.0f),        // 61 Keys
            new FrequencyRange(41.2f, 2637.02f),        // 73 Keys
            new FrequencyRange(41.2f, 3135.96f),        // 76 Keys
            new FrequencyRange(27.5f, 4186.009f),       // 88 Keys
            new FrequencyRange(20.60172f, 12543.86f),   // 112 Keys
            new FrequencyRange(8.175798f, 12543.86f),   // 128 Keys
        };

        public static void SetBaseFrequency(float lower, float upper)
        {
            _lowerBaseFrequency = lower;
            _upperBaseFrequency = upper;
        }

        public static void SetBaseFrequency(FrequencyBase frequencyBase)
        {
            _lowerBaseFrequency = frequencyBasePresets[(int)frequencyBase].lower;
            _upperBaseFrequency = frequencyBasePresets[(int)frequencyBase].upper;
        }

        public static void GetRange(FrequencyBase frequencyBase, out float lower, out float upper)
        {
            lower = frequencyBasePresets[(int)frequencyBase].lower;
            upper = frequencyBasePresets[(int)frequencyBase].upper;
        }

        public static float[] GetBands(BandOption bandOption)
        {
            return Bands[(int)bandOption];
        }
                
        /// <summary>
        /// Transforms linear frequency. The linear frequency is normalized, so it's a value that ranges from 0.0 to 1.0.
        /// </summary>
        public static float TransformLinearFrequency(float linearizedFrequency, FrequencyTransform transform)
        {
            if (!transform.clamp)
            {
                linearizedFrequency *= transform.repeat;

                if (transform.alternate)
                {
                    linearizedFrequency = Mathf.PingPong(linearizedFrequency, 1.0f);
                }
                else
                {
                    linearizedFrequency = linearizedFrequency % 1.0f;
                }
            }

            if (transform.reverse)
            {
                linearizedFrequency = 1.0f - linearizedFrequency;
            }

            return linearizedFrequency;
        }

        /// <summary>
        /// Remaps a linear frequency to a new frequency range between lower and upper frequency, inclusive.
        /// </summary>
        public static float RemapLinearFrequency(float linearFrequency, float lower, float upper)
        {
            // lower
            float l = LinearizeFrequency(lower);

            // upper
            float u = LinearizeFrequency(upper);

            // delta
            float d = u - l;

            return Mathf.Clamp01(l + (d * linearFrequency));
        }

        /// <summary>
        /// Returns the number of FrequencyBands given a particular BandOption.
        /// </summary>
        /// <param name="bandOption"></param>
        public static int GetBandCount(BandOption bandOption)
        {
            return Bands[(int)bandOption].Length;
        }

        /// <summary>
        /// Gets a frequency range preset.
        /// </summary>
        public static void GetRangePreset(out float lower, out float upper, int preset, FrequencyBase frequencyMode)
        {
            if (frequencyMode == FrequencyBase.Audio)
            {
                lower = audibleRangePresets[(int)preset].lower;
                upper = audibleRangePresets[(int)preset].upper;
            }
            else
            {
                lower = midiRangePresets[(int)preset].lower;
                upper = midiRangePresets[(int)preset].upper;
            }
        }

        /// <summary>
        /// Converts a linear magnitude to decibel and normalizes it.
        /// 
        /// Reference:
        /// http://www.playdotsound.com/portfolio-item/decibel-db-to-float-value-calculator-making-sense-of-linear-values-in-audio-tools/
        /// </summary>
        public static float LinearToDecibelCentered01(float linear)
        {
            // convert linear to db, center the range -80db to 0db (it becomes -40 to 40), then normalize
            // doing it this way makes the upper half of the spectrum more pronounced. the downside is
            // that the spectrum doesn't come to life as quickly.
            return Mathf.Max(Mathf.Log10(linear) * 20.0f + 40.0f, 0.0f) * 0.00625f;  // 0.00625 = 1 / 80 * 0.5
        }

        /// <summary>
        /// Converts a linear magnitude to decibel.
        /// </summary>
        public static float LinearToDecibel(float linear)
        {
            return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
        }

        /// <summary>
        /// Converts a decibel magnitude to linear.
        /// </summary>
        /// <returns></returns>
        public static float DecibelToLinear(float decibel)
        {
            return Mathf.Pow(10.0f, decibel / 20.0f);
        }

        /// <summary>
        /// Converts a frequency to a spectrum index.
        /// 
        /// Reference:
        /// https://answers.unity.com/questions/175173/audio-analysis-pass-filter.html
        /// 
        /// Note:
        /// This is a lossy conversion.
        /// </summary>
        public static int FrequencyToSpectrumIndex(float frequency, int arrayLength, float sampleRate)
        {
            return Mathf.Min((int)(frequency * 2.0f * (float)(arrayLength - 1) / sampleRate), arrayLength - 1);
        }

        /// <summary>
        /// Converts a spectrum index to a frequency.
        /// 
        /// Note:
        /// This is a lossy conversion.
        /// </summary>
        // lossy
        public static float SpectrumIndexToFrequency(int index, int arrayLength, float sampleRate)
        {
            return (float)index * (sampleRate * 0.5f) / (float)(arrayLength - 1);
        }

        /// <summary>
        /// Converts a frequency to an array index given a specific frequency range, inclusive.
        /// </summary>
        public static int FrequencyToArrayIndex (float frequency, int arrayLength, float lower, float upper)
        {
            float f = Mathf.Log10(frequency);
            float l = Mathf.Log10(lower);
            float u = Mathf.Log10(upper);
            float d = u - l;

            float normalizedIndex = (f - l) / d;

            return Mathf.FloorToInt(normalizedIndex * (float)(arrayLength - 1));
        }

        /// <summary>
        /// Converts a frequency to an array index between LowestAudibleFrequency and HighestAudibleFrequency, inclusive.
        /// </summary>
        public static int FrequencyToArrayIndex(float frequency, int arrayLength)
        {
            return FrequencyToArrayIndex(frequency, arrayLength, _lowerBaseFrequency, _upperBaseFrequency);
        }

        /// <summary>
        /// Converts frequency to a linearized frequency given a lower and upper frequency (inclusive),
        /// and then normalized.
        /// </summary>
        public static float LinearizeFrequency(float frequency, float lower, float upper)
        {
            float f = Mathf.Log10(frequency);
            float l = Mathf.Log10(lower);
            float u = Mathf.Log10(upper);
            float d = u - l;

            return (f - l) / d;
        }
        
        /// <summary>
        /// Converts frequency to a linearized frequency between LowestAudibleFrequency and HighestAudibleFrequency (inclusive),
        /// and then normalized.
        /// </summary>
        public static float LinearizeFrequency(float frequency)
        {
            return LinearizeFrequency(frequency, _lowerBaseFrequency, _upperBaseFrequency);
        }

        /// <summary>
        /// Converts linear frequency to just frequency in the specified lower and upper range, inclusive
        /// </summary>
        public static float UnlinearizeFrequency(float linearizeFrequency, float lower, float upper)
        {
            float u = Mathf.Log10(upper);
            float l = Mathf.Log10(lower);

            float p = Mathf.Lerp(l, u, linearizeFrequency);

            return Mathf.Pow(10, p);
        }

        /// <summary>
        /// Converts linear frequency to just frequency in the range between LowestAudibleFrequency and HighestAudibleFrequency (inclusive)
        /// </summary>
        public static float UnlinearizeFrequency(float linearizeFrequency)
        {
            return UnlinearizeFrequency(linearizeFrequency, _lowerBaseFrequency, _upperBaseFrequency);
        }

        /// <summary>
        /// Converts a band in the range of lower and upper to an octave
        /// </summary>
        public static float OctavesFromFrequencyBand(float lower, float upper)
        {
            return Mathf.Log(upper / lower) / Mathf.Log(2.0f);
        }

        /// <summary>
        /// Gets the center frequency of a band in the range of upper and lower
        /// </summary>
        public static float CenterFrequency(float upper, float lower)
        {
            return Mathf.Sqrt(upper * lower);
        }

        /// <summary>
        /// Gets the upper frequency of an octave
        /// </summary>
        public static float UpperFrequency(float frequency, float octave)
        {
            return frequency * octave;
        }

        /// <summary>
        /// Gets the lower frequency of an octave
        /// </summary>
        public static float LowerFrequency(float frequency, float octave)
        {
            return frequency / octave;
        }

        /// <summary>
        /// Gets the width of a band given a frequency and octave
        /// </summary>
        public static float BandWidth(float frequency, float octave)
        {
            return (frequency * octave) - (frequency / octave);
        }

        public static float Velocity(float w, float f)
        {
            return w * f;
        }

        public static float WaveLength(float v, float f)
        {
            return v / f;
        }

        public static float FrequencyToTime(float f)
        {
            return 1.0f / f;
        }

        public static float TimeToFrequency(float T)
        {
            return 1.0f / T;
        }

        // wavelength
        // velocity
        public static float WaveToFrequency(float w, float v)
        {
            return v / w;
        }

        public static float FrequencyFromAngle(float w)
        {
            return w / (2.0f * Mathf.PI);
        }
    }
}