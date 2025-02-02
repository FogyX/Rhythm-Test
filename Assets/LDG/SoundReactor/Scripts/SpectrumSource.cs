// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

// The following copywrite notice is for the parts of this file that resemble the original work
// created by Keijiro in the project located here:
// https://github.com/keijiro/unity-audio-spectrum
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.IO;

namespace LDG.SoundReactor
{
    public enum NormalizeMode
    {
        Raw,
        Peak,
        PeakBand
    }

    // https://docs.unity3d.com/Manual/class-AudioClip.html
    public enum AudioChannel
    {
        FrontLeft,
        FrontRight,
        FrontCenter,
        Subwoofer,
        RearLeft,
        RearRight,
        AlternativeRearLeft,
        AlternativeRearRight
    }

    [DisallowMultipleComponent]
    public class SpectrumSource : SerializeableObject
    {
        #region Properties
        private float _peakLevel = 0.00001f;
        private bool _recordProfile = false;

        /// <summary>
        /// Returns the number of channels stored in audioSource.
        /// </summary>
        public int channels
        {
            get
            {
                if (audioSource)
                {
                    return audioSource.clip.channels;
                }

                return 1;
            }
        }

        /// <summary>
        /// Contains the peak level of the entire spectrum.
        /// </summary>
        public float peakLevel
        {
            get
            {
                return _peakLevel;
            }
        }

        /// <summary>
        /// The number of bands for a particular BandOption.
        /// </summary>
        public int bandCount
        {
            get
            {
                if (peaksProfile)
                {
                    return Frequency.GetBandCount(bandOption) - 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// The number of levels.
        /// </summary>
        public int levelCount
        {
            get
            {
                if (levels != null)
                {
                    return levels.Length;
                }

                return 0;
            }
        }

        /// <summary>
        /// A flag used for indicating that a profile should or shouldn't be recording.
        /// </summary>
        public bool recordProfile
        {
            get
            {
                return _recordProfile;
            }

            set
            {
                _recordProfile = value;
            }
        }
        #endregion

        #region Fields
        public MidiSource midiSource;
		public bool globalFallback = true;
        public AudioSource audioSource;
        public PeaksProfile peaksProfile;

        public FrequencyBase sourceMode = FrequencyBase.Audio;
        public int audioChannel = (int)AudioChannel.FrontLeft;
        public BandOption bandOption = BandOption.StandardRanges;
        public NormalizeMode normalizeMode = NormalizeMode.PeakBand;

        public float amplitudeScale = 1.0f;

        // spectrum index is in Hz [0, 24000] and their values are the raw amplitude
        private float[] spectrumData;

        // these become the size of band count
        private float[] peaks;
        private float[] levels;
        #endregion

        #region Public Methods
#if UNITY_EDITOR
        float prevTime;

        public void RecordPeaks()
        {
            if (audioSource)
            {
                Refresh();
                audioSource.Play();
                bandOption = BandOption.OneTwelvethOctave;

                prevTime = 0.0f;
                recordProfile = true;
            }
        }

        void AutoSavePeaks()
        {
            if (audioSource && audioSource.clip && recordProfile && peaksProfile)
            {
                if ((audioSource.time - prevTime) < 0.0f)
                {
                    peaksProfile.SetPeaks(peaks, peakLevel);

                    EditorUtility.SetDirty(peaksProfile);
                    
                    Debug.Log("Saved peaks for: " + audioSource.clip.name);

                    recordProfile = false;

                    EditorUtility.SetDirty(this);
                }

                prevTime = audioSource.time;
            }
        }
#endif

        public float GetLevel(float linearizedFrequency, InterpolationMode interpolationMode)
        {
            return Spline.Tween(linearizedFrequency, levels, false, interpolationMode);
        }

        public Note GetMidiNote(float normalizedIndex)
        {
            return (midiSource) ? midiSource.GetMidiNote(normalizedIndex) : Note.Empty;
        }

        public void Refresh()
        {
            if (peaksProfile)
            {
                UpdateBuffers();
                ResetPeaks();
            }
        }
        #endregion

        #region Unity Overrides
        private void Update()
        {
            if (peaksProfile)
            {
                UpdateBuffers();
                UpdateSpectrum();
#if UNITY_EDITOR
                AutoSavePeaks();
#endif
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the peaks. If there is a spectrum profile asset, then the peaks will be loaded from there.
        /// </summary>
        void ResetPeaks()
        {
            if (peaksProfile && !peaksProfile.isDirty && peaks != null)
            {
                peaksProfile.GetPeaks(peaks, ref _peakLevel);
            }
            else
            {
                if (peaks != null && peaks.Length == bandCount)
                {
                    for (int i = 0; i < bandCount; i++)
                    {
                        peaks[i] = 0.001f;
                    }
                }

                _peakLevel = 0.001f;
            }
        }

        /// <summary>
        /// Allocate buffers dynamically so they can be changed at runtime without crashing the app
        /// </summary>
        void UpdateBuffers()
        {
            if (sourceMode == FrequencyBase.Audio)
            {
                if (spectrumData == null || spectrumData.Length != peaksProfile.fftSamples)
                {
                    spectrumData = new float[peaksProfile.fftSamples];
                }

                if (levels == null || levels.Length != bandCount)
                {
                    levels = new float[bandCount];
                }

                if (peaks == null || peaks.Length != bandCount)
                {
                    peaks = new float[bandCount];

                    ResetPeaks();
                }
            }
            else
            {
                if (levels == null || levels.Length != 128)
                {
                    levels = new float[128];
                }
            }
        }

        /// <summary>
        /// Updates spectrum peaks and levels from a given audio channel.
        /// </summary>
        void UpdateSpectrum()
        {
            float lower;
            float upper;

            // get spectrum data from a particular audio channel
            if (sourceMode == FrequencyBase.Audio)
            {
                // leave now if a spectrum profile doesn't exist
                if (!peaksProfile) return;

                if (audioSource && audioSource.clip)
                {
                    // get filtered audioChannel.
                    int audioChannel = Mathf.Clamp(this.audioChannel, 0, (audioSource.clip.channels - 1));

                    // get spectrum from sudioSource.clip
                    audioSource.GetSpectrumData(spectrumData, audioChannel, peaksProfile.fftWindow);
                }
                else if(globalFallback)
                {
                    // get spectrum from all sounds currently playing
                    AudioListener.GetSpectrumData(spectrumData, 0, peaksProfile.fftWindow);
                }
                
                float[] frequencyBands = Frequency.GetBands(bandOption);
                
                // gather band peaks
                for (var bi = 0; bi < bandCount; bi++)
                {
                    lower = frequencyBands[bi];
                    upper = frequencyBands[bi + 1];

                    int minIndex = Frequency.FrequencyToSpectrumIndex(lower, spectrumData.Length, AudioSettings.outputSampleRate);
                    int maxIndex = Frequency.FrequencyToSpectrumIndex(upper, spectrumData.Length, AudioSettings.outputSampleRate);

                    var bandPeak = 0.0f;

                    for (var fi = minIndex; fi <= maxIndex; fi++)
                    {
                        bandPeak = Mathf.Max(bandPeak, spectrumData[fi]);
                    }

                    switch (peaksProfile.amplitudeMode)
                    {
                        case AmplitudeMode.Linear:
                            break;

                        case AmplitudeMode.Decibel:
                            bandPeak = Mathf.Max(Frequency.LinearToDecibelCentered01(bandPeak), 0.0f);
                            break;
                    }

                    levels[bi] = bandPeak;

                    // record peaks
                    //if (_recordProfile)
                    {
                        //Debug.Log("what!");
                        peaks[bi] = Mathf.Max(bandPeak, peaks[bi]);
                        _peakLevel = Mathf.Max(bandPeak, peakLevel);
                    }
                }

                // normalize levels
                for (var bi = 0; bi < bandCount; bi++)
                {
                    switch (normalizeMode)
                    {
                        case NormalizeMode.Raw:
                            //levels[bi] = levels[bi];
                            break;
                        case NormalizeMode.PeakBand:
                            levels[bi] = levels[bi] / peaks[bi];
                            break;
                        case NormalizeMode.Peak:
                            levels[bi] = levels[bi] / peakLevel;
                            break;
                    }
                }
            }
            else
            {
                if (midiSource == null) return;

                midiSource.GetNoteVelocities(levels);
            }
        }
        #endregion

        #region Developer
#if UNITY_EDITOR
        /// <summary>
        /// Write frequencies to a file. Use this if you'd like to add your own custom frequency range.
        /// </summary>
        void Awake()
        {
            //WriteOctaveBands(2);
            //WritePianoOctave();
        }

        float A440(int n)
        {
            return Mathf.Pow(2, (float)(n - 49) / 12.0f) * 440.0f;
        }

        void WritePianoOctave()
        {
            using (StreamWriter file = new StreamWriter("d:/Piano.txt"))
            {
                for (int i = -24; i < 120; i++)
                {
                    file.Write(A440(i) + "f, ");
                }

                file.Close();
            }
        }

        void WriteOctaveBands(float octaves)
        {
            float centerFreq = Frequency.LowestAudibleFrequency;
            float octave = Mathf.Pow(2, 1f / octaves);

            using (StreamWriter file = new StreamWriter("d:/Octaves.txt"))
            {
                file.Write(centerFreq + "f, ");

                while (centerFreq < 20000)
                {
                    centerFreq *= octave;

                    file.Write(centerFreq + "f, ");
                }

                file.Close();
            }
        }
#endif
        #endregion
    }
}