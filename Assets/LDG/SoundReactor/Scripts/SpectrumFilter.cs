// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum BeatTrigger { Ascend, Descend, AscendAndDescend }

    public enum FallSpeedSource { SpectrumFilter, Level }

    [DisallowMultipleComponent]
    public class SpectrumFilter : SerializeableObject
    {
        #region Fields
        public SpectrumSource spectrumSource;
        public EQ eq;

        /// <summary>
        /// Defines how the spectrum is interpolated: it's either linear or cuved.
        /// </summary>
        public InterpolationMode interpolationMode = InterpolationMode.Linear;

        /// <summary>
        /// Scales the spectrum.
        /// </summary>
        public float scale = 1.0f;

        /// <summary>
        /// Defines the speed at which the spectrum will fall in normalized space.
        /// </summary>
        public float fallSpeed = 2.0f;

        /// <summary>
        /// Defines how far the spectrum has to vary before a beat is detected. This is in normalized space.
        /// </summary>
        public float beatSensitivity = 0.2f;

        /// <summary>
        /// Defines what kind of spectrum motions can trigger a beat.
        /// </summary>
        public BeatTrigger beatTrigger = BeatTrigger.Ascend;

        private bool eqSearched = false;
        #endregion

        #region Unity Editor
#if UNITY_EDITOR
        // The following is only used in the editor to draw the falling level.
        [System.NonSerialized]
        public float[] fallingLevels = null;
        float linearizedFrequency = 0.0f;

        public void UpdateFallingLevels()
        {
            if (spectrumSource.levelCount == 0) return;

            if (fallingLevels == null || fallingLevels.Length != spectrumSource.levelCount)
            {
                fallingLevels = new float[spectrumSource.levelCount];

                for (int i = 0; i < spectrumSource.levelCount; i++)
                {
                    fallingLevels[i] = 0.001f;
                }
            }

            for (int i = 0; i < spectrumSource.levelCount; i++)
            {
                linearizedFrequency = (float)i / ((float)spectrumSource.levelCount - 1.0f);

                fallingLevels[i] -= Time.deltaTime * fallSpeed * GetAmplitude(linearizedFrequency);
                fallingLevels[i] = Mathf.Max(fallingLevels[i], GetScaledLevel(linearizedFrequency));
            }
        }
#endif
        #endregion

        #region Public Methods
        public Note GetMidiNote(float linearizedFrequency)
        {
            return (spectrumSource) ? spectrumSource.GetMidiNote(linearizedFrequency) : Note.Empty;
        }

        public float GetAmplitude(float linearizedFrequency)
        {
            float eqValue = 1.0f;
            float amplitude = 1.0f;

            if (spectrumSource)
            {
                amplitude = spectrumSource.amplitudeScale;
            }

            if (eq)
            {
                eqValue = eq.GetLevel(linearizedFrequency, interpolationMode);
            }

            return  eqValue * scale * amplitude;
        }

        /// <summary>
        /// Returns the level magnitude at a given linear frequency. The value is also scaled by the EQ if it's assigned one.
        /// </summary>
        public float GetScaledLevel(float linearizedFrequency)
        {
            float level = 0.0f;
            float eqValue = 1.0f;
            float amplitudeScale = 1.0f;

            if(spectrumSource)
            {
                level = spectrumSource.GetLevel(linearizedFrequency, interpolationMode);
                amplitudeScale = spectrumSource.amplitudeScale;
            }

            if(eq)
            {
                eqValue = eq.GetLevel(linearizedFrequency, interpolationMode);
            }

            return level * eqValue * scale * amplitudeScale;
        }

        /// <summary>
        /// Returns the normalized level magnitude at a given linear frequency.
        /// </summary>
        public float GetNormalizedLevel(float linearizedFrequency)
        {
            float level = 0.0f;

            if (spectrumSource)
            {
                level = spectrumSource.GetLevel(linearizedFrequency, interpolationMode);
            }

            return level;
        }
        #endregion

        #region Monobehavior
        void Update()
        {
#if UNITY_EDITOR
            // we need to remember object reference states before we change them at run time. to do this we
            // need to give EditorApplication a chance to post the PlayModeStateChange.EnteredPlayMode event.
            // by default that event seems to post after the monobehaviour events are called.
            // since I can't change when that event is posted, I have to delay when the dependencies are aquired.
            if (Time.frameCount <= 2) return;
#endif

            if (InheritDependencies())
            {
#if UNITY_EDITOR
                UpdateFallingLevels();
#endif
            }
        }
        #endregion
        /// <summary>
        /// Looks for a spectrum source and eq on this object, and if there isn't one, it continues searching up
        /// the hierarchy until it finds one.
        /// </summary>
        bool InheritDependencies()
        {
            Transform parent;

            if (eq == null && !eqSearched)
            {
                eqSearched = true;

                parent = transform.parent;

                eq = GetComponent<EQ>();

                while (eq == null)
                {
                    if (parent != null)
                    {
                        if (spectrumSource == null)
                        {
                            eq = parent.GetComponent<EQ>();
                        }

                        parent = parent.parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (spectrumSource == null)
            {
                parent = transform.parent;

                spectrumSource = GetComponent<SpectrumSource>();

                while (spectrumSource == null)
                {
                    if (parent != null)
                    {
                        if (spectrumSource == null)
                        {
                            spectrumSource = parent.GetComponent<SpectrumSource>();
                        }

                        parent = parent.parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return (spectrumSource != null);
        }
    }
}