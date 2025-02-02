// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class Level : SerializeableObject
    {
        #region Properties
        [SerializeField]
        private float _normalizedLevel = 0.0f;
        private float _levelBeat = 0.0f;
        private float _fallingNormalizedLevel = 0.0f;
        private float _levelScalar = 1.0f;
        private float _beatScalar = 0.0f;
        private float _amplitude = 1.0f;

        /// <summary>
        /// A value that is set that defines what position it is vertically. It's a normalized position, i.e. a
        /// position based on the percentage of the height. For example, 0.5 is the level at roughly center, 0.0
        /// is the position at the bottom, and 1.0 is the top.
        /// </summary>
        public float normalizedLevel
        {
            get { return _normalizedLevel; }
            protected set { _normalizedLevel = value; }
        }
        
        /// <summary>
        /// The level value when a beat was detected
        /// </summary>
        public float levelBeat
        {
            get
            {
                return _levelBeat;
            }
        }
        
        /// <summary>
        /// A level that constantly falls from the highest recorded peak on any given frame.
        /// </summary>
        public float fallingLevel
        {
            get
            {
                return _fallingNormalizedLevel * _levelScalar * _amplitude;
            }
        }
        
        /// <summary>
        /// Same as fallingLevel, except it returns 0 if the value is below the normalizedLevel.
        /// </summary>
        public float levelScalar
        {
            get
            {
                return _levelScalar;
            }
        }
        
        /// <summary>
        /// Contains a 1.0 for a single frame if a beat was detected, otherwise it contains 0.0.
        /// </summary>
        public float beatScalar
        {
            get
            {
                return _beatScalar;
            }
        }
        #endregion

        #region Fields
        /// <summary>
        /// Set the fall speed source.
        /// </summary>
        public FallSpeedSource fallSpeedSource = FallSpeedSource.SpectrumFilter;

        /// <summary>
        /// The speed the level will fall by.
        /// </summary>
        public float fallSpeed = 1.0f;

        /// <summary>
        /// The SpectrumFilter to grab spectrum values from
        /// </summary>
        public SpectrumFilter spectrumFilter;

        /// <summary>
        /// Frequency mode used by this level.
        /// </summary>
        public FrequencyBase frequencyMode = FrequencyBase.Audio;

        /// <summary>
        /// Frequency range from 0.0 to 1.0. Frequency at 0.0 is the lowest possible frequency, and 1.0 is the highest.
        /// </summary>
        public float linearizedFrequency = 0.0f;

        /// <summary>
        /// MIDI note [0-127]. Use this to get the note when FrequencyMode is set to MIDI.
        /// </summary>
        public int midiNote
        {
            get
            {
                return Mathf.FloorToInt(linearizedFrequency * 127.0f);
            }
        }

        /// <summary>
        /// Flag whether or not a PropertyDriver can inherit this level
        /// </summary>
        public bool inheritable = true;

        /// <summary>
        /// Contains the value of level from the last frame.
        /// </summary>
        private float levelPrev;
        
        /// <summary>
        /// Direction the level is moving relative to the last recorded level.
        /// </summary>
        private float levelDir = -1;

        private float levelDirPrev = -1;

        /// <summary>
        /// Is set to true if the level changes direction.
        /// </summary>
        private bool changedDir = true;

        /// <summary>
        /// Value recorded for the purpose of detecting an up, down, or up and down beat.
        /// </summary>
        private float levelDatum = 0.0f;
        #endregion

        #region Public Methods
        public Note GetMidiNote()
        {
            if (spectrumFilter == null) return Note.Empty;
            
            return spectrumFilter.GetMidiNote(linearizedFrequency);
        }
        /// <summary>
        /// Sets this level's frequency and transform information. The frequency is in linear space and ranges from 0 to 1.
        /// lowerFrequency and upperFrequency is a logarithm.
        /// </summary>
        public void Set(float linearizedFrequency, float normalizedLevel, FrequencyBase frequencyMode, float lowerFrequency, float upperFrequency, FrequencyTransform transform)
        {
            this.frequencyMode = frequencyMode;

            if (transform.flipLevel)
            {
                normalizedLevel = 1.0f - normalizedLevel;
            }

            linearizedFrequency = Frequency.TransformLinearFrequency(linearizedFrequency, transform);

            Frequency.SetBaseFrequency(frequencyMode);

            this.linearizedFrequency = Frequency.RemapLinearFrequency(linearizedFrequency, lowerFrequency, upperFrequency);
            
            this.normalizedLevel = normalizedLevel;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Looks for a SpectrumFilter on this object, and if there isn't one, it continues searching up the hierarchy until
        /// it finds one.
        /// </summary>
        bool InheritDependencies()
        {
            if (spectrumFilter) return true;

            Transform parent = transform.parent;

            if(spectrumFilter == null)
            {
                spectrumFilter = GetComponent<SpectrumFilter>();
            }
            
            while (spectrumFilter == null)
            {
                if(parent != null)
                {
                    spectrumFilter = parent.GetComponent<SpectrumFilter>();

                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return (spectrumFilter);
        }

        #endregion // end Private Methods

        #region MonoBehaviours
        /// <summary>
        /// Grab dependencies and calculate beats. This is done in LateUpdate because SpectrumSource updates the 
        /// spectrum in Update. If they're both done in Update there's no gaurantee that the spectrum won't be
        /// updateted in the midst of updating the levels.
        /// </summary>
        void LateUpdate()
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
                float level;
                _beatScalar = 0.0f;

                // get current level and amplitude
                level = spectrumFilter.GetNormalizedLevel(linearizedFrequency);
                _amplitude = spectrumFilter.GetAmplitude(linearizedFrequency);

                if (!changedDir)
                {
                    if (level != levelPrev)
                    {
                        levelDirPrev = levelDir;
                        levelDir = Mathf.Sign(level - levelPrev);

                        if (levelDir != levelDirPrev)
                        {
                            // record the position of the previous level now that it has changed directions
                            levelDatum = levelPrev;
                            changedDir = true;
                        }
                    }
                }
                
                // only begin checking for a beat if the level changes direction.
                if (changedDir)
                {
                    float levelDelta = level - levelDatum;

                    switch (spectrumFilter.beatTrigger)
                    {
                        case BeatTrigger.Ascend:
                            levelDatum = Mathf.Min(levelDatum, level);

                            if (levelDelta >= spectrumFilter.beatSensitivity)
                            {
                                _beatScalar = 1.0f;
                            }
                            break;
                        case BeatTrigger.Descend:
                            levelDatum = Mathf.Max(levelDatum, level);

                            if (levelDelta <= -spectrumFilter.beatSensitivity)
                            {
                                _beatScalar = 1.0f;
                            }
                            break;
                        case BeatTrigger.AscendAndDescend:
                            if (Mathf.Abs(levelDelta) >= spectrumFilter.beatSensitivity)
                            {
                                _beatScalar = 1.0f;
                            }
                            break;
                    }

                    // beat detected so reset states
                    if(_beatScalar == 1.0f)
                    {
                        changedDir = false;
                        _levelBeat = level;

                        levelDir = Mathf.Sign(levelDelta);
                    }
                }

                levelPrev = level;

                switch(fallSpeedSource)
                {
                    case FallSpeedSource.SpectrumFilter:
                        _fallingNormalizedLevel -= Time.deltaTime * spectrumFilter.fallSpeed;
                        break;

                    case FallSpeedSource.Level:
                        _fallingNormalizedLevel -= Time.deltaTime * fallSpeed;
                        break;
                }

                _fallingNormalizedLevel = Mathf.Max(level, _fallingNormalizedLevel);

                _levelScalar = (_fallingNormalizedLevel >= normalizedLevel) ? 1.0f : 0.0f;
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.19f, 0.65f, 0.86f, 0.9f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
        #endregion // end Unity Methods
    }
}
