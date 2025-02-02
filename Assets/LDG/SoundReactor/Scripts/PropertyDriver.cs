// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public class PropertyDriver : SerializeableObject
    {
        #region Properties
        public bool isBeat { get { return (level.beatScalar == 1.0f); } }
        #endregion

        #region Fields
        /// <summary>
        /// Use info from the shared driver instead of from this one.
        /// </summary>
        public PropertyDriver sharedDriver;

        /// <summary>
        /// Level this PropertyDriver grabs frequency magnitudes from
        /// </summary>
        public Level level;

        /// <summary>
        /// The amount of travel the level can move.
        /// </summary>
        public Vector3 travel = Vector3.up;

        /// <summary>
        /// Variable that is used to keep the level from scaling up past the travel distance.
        /// </summary>
        public float clipping = 1.0f;

        /// <summary>
        /// Sets the speed at wich the level travels.
        /// </summary>
        public float strength = 1.0f;

        /// <summary>
        /// Flag used to scale the level once per beat
        /// </summary>
        public bool onBeat = false;

        /// <summary>
        /// Marks if a component is missing. It's a property to keep the variable from getting serialized.
        /// </summary>
        public bool componentMissing { get; set; }

        /// <summary>
        /// The level that gets cached after it has been scaled by travel, strength, and has been clipped.
        /// </summary>
        private Vector3 scaledLevel;

        /// <summary>
        /// Flag to signify that scaledLevel should be recalculated.
        /// </summary>
        private bool updateLevelVector = true;

        /// <summary>
        /// Flag to signify that scaledLevel should be recalculated.
        /// </summary>
        private bool updateLevelScalar = true;
        #endregion

        #region MonoBehaviors
        protected virtual void LateUpdate()
        {
            updateLevelVector = true;
            updateLevelScalar = true;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            // we need to remember object reference states before we change them at run time. to do this we
            // need to give EditorApplication a chance to post the PlayModeStateChange.EnteredPlayMode event.
            // by default that event seems to post after the monobehaviour events are called.
            // since I can't change when that event is posted, I have to delay when the dependencies are aquired.
            if (Time.frameCount <= 2) return;
#endif

            if (InheritDependencies() && !componentMissing)
            {
                DoLevel();
            }
        }
        #endregion

        #region Public Methods
        public Note GetMidiNote()
        {
            return level.GetMidiNote();
        }

        /// <summary>
        /// Gets the level vector scaled by travel and strength, and clipped by clipping.
        /// </summary>
        public Vector3 LevelVector()
        {
            // cache this vector once per frame. this means that LevelVector only has to be calculated once
            // per frame instead of per call.
            if (updateLevelVector)
            {
                updateLevelVector = false;

                PropertyDriver driver = (sharedDriver != null) ? sharedDriver : this;

                float s = 0.0f;

                if (driver.onBeat)
                {
                    float beatScale = (level.beatScalar == 1.0f) ? 1.0f : 0.0f;

                    s = Mathf.Clamp(level.fallingLevel * driver.strength * beatScale, 0.0f, driver.clipping);
                }
                else
                {
                    s = Mathf.Clamp(level.fallingLevel * driver.strength, 0.0f, driver.clipping);
                }

                scaledLevel = driver.travel * s;
            }

            return scaledLevel;
        }

        /// <summary>
        /// Gets the level scalar scaled by travel and strength, and clipped by clipping.
        /// </summary>
        public float LevelScalar()
        {
            if (updateLevelScalar)
            {
                updateLevelScalar = false;

                PropertyDriver driver = (sharedDriver != null) ? sharedDriver : this;

                float s;

                if (driver.onBeat)
                {
                    float beatScale = (level.beatScalar == 1.0f) ? 1.0f : 0.0f;

                    s = Mathf.Clamp(level.fallingLevel * driver.strength * beatScale, 0.0f, driver.clipping);
                }
                else
                {
                    s = Mathf.Clamp(level.fallingLevel * driver.strength, 0.0f, driver.clipping);
                }

                scaledLevel.y = driver.travel.magnitude * s;
            }

            return scaledLevel.y;
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Looks for a level on this object, and if there isn't one, continues searching up the hierarchy until
        /// it finds one.
        /// </summary>
        protected virtual bool InheritDependencies()
        {
            if (level) return true;

            Transform parent = transform.parent;
            Level lvl = null;

            if (level == null)
            {
                level = GetComponent<Level>();
            }

            while (level == null)
            {
                if (parent != null)
                {
                    if ((lvl = parent.GetComponent<Level>()))
                    {
                        if (lvl.inheritable)
                        {
                            level = lvl;
                        }
                    }

                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return (level);
        }

        protected virtual void DoLevel()
        {
            // nothing to do here
        }
        #endregion
    }
}
