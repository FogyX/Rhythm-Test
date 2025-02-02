// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum AmplitudeMode
    {
        Linear,
        Decibel
    }

    /// <summary>
    /// A scriptable object used for storing peak data. This data is loaded at play time and
    /// is used to normalize the spectrum magnitudes.
    /// </summary>
    public class PeaksProfile : ScriptableObject
    {
        /// <summary>
        /// FFT window used during recording
        /// </summary>
        public FFTWindow fftWindow = FFTWindow.Hamming;

        /// <summary>
        /// Number of samples to use during recording
        /// </summary>
        public int fftSamples = 2048;

        /// <summary>
        /// Sets the mode to either dB or Linear.
        /// </summary>
        public AmplitudeMode amplitudeMode = AmplitudeMode.Linear;

        /// <summary>
        /// A single peak that contains the highest peak of the entire spectrum.
        /// </summary>
        public float peak;

        /// <summary>
        /// Peaks that contain highest peaks for individual bands within a spectrum. Currently 30
        /// bands are stored. This can be changed to 120 if you desire, but is pointless to go any
        /// higher than that because Sound Reactor only supports up to 120 bands.
        /// </summary>
        public float[] peaks;

        /// <summary>
        /// A unique value used to identify the characteristics of this profile. It's mainly used to
        /// help determine if a profile has been made dirty or not.
        /// </summary>
        [SerializeField]
        private int hash = 0;

        /// <summary>
        /// Flag indicating if the profile has been made dirty or not. A profile becomes dirty if any
        /// of the following fields change:
        ///    FFT Window
        ///    FFT Samples
        ///    Amplitude Mode
        /// </summary>
        public bool isDirty
        {
            get
            {
                return hash != GetHashCode();
            }
        }

        public PeaksProfile()
        {
            ResetPeaks();
        }

        /// <summary>
        /// Sets profile peaks to the input peaks. The input peaks are interpolated to the resolution of the
        /// profile's peaks, which is currently 30. Changing this number to anything higher is really not
        /// necessary unless you want to record anything more than a calibration tone.
        /// </summary>
        public void SetPeaks(float[] peaks, float peak)
        {
            int bands = 30;

            this.peaks = new float[bands];

            for (int i = 0; i < this.peaks.Length; i++)
            {
                this.peaks[i] = Spline.Tween((float)i / (float)(this.peaks.Length - 1), peaks, false, InterpolationMode.Curve);
            }

            this.peak = peak;

            hash = GetHashCode();
        }

        /// <summary>
        /// Gets profile peaks interpolated to the resolution of peaks.
        /// </summary>
        public void GetPeaks(float[] peaks, ref float peak)
        {
            if(peaks.Length == 1)
            {
                peak = this.peak;
                return;
            }

            for (int i = 0; i < peaks.Length; i++)
            {
                peaks[i] = Spline.Tween((float)i / (float)(peaks.Length - 1), this.peaks, false, InterpolationMode.Curve);
            }

            peak = this.peak;
        }

        /// <summary>
        /// Resets the peaks to a very small number. Resets hash to 0 as well so the profile will become dirty.
        /// </summary>
        public void ResetPeaks()
        {
            peaks = new float[30];

            for (int i = 0; i < this.peaks.Length; i++)
            {
                peaks[i] = 0.001f;
            }

            peak = 0.001f;

            hash = 0;
        }

        /// <summary>
        /// Generate hash from string.
        /// </summary>
        private int StringToHashCode(string s)
        {
            int result = 0;

            foreach(char c in s)
            {
                result += c;
            }

            return result * s.Length;
        }

        /// <summary>
        /// Return a hash code generated from the current state of the profile.
        /// 
        /// Reference:
        /// https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
        /// </summary>
        override public int GetHashCode()
        {
            int hash = 17;

            unchecked
            {
                // DARN YOU GetHashCode FOR NOT RETURNING CONSISTENT VALUES! So I will hash using modified string representations then :P
                hash = hash * 31 + StringToHashCode(fftWindow.ToString());
                hash = hash * 31 + StringToHashCode(fftSamples.ToString());
                hash = hash * 31 + StringToHashCode(amplitudeMode.ToString());
            }

            return hash;
        }
    }
}