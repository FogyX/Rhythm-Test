/*
IMPORTANT:
  It is tempting to use this script directly, but don't! It has a habit of changing sometimes. The best thing to do
  if you want to use this script in your project is to duplicate it.
  
INSTRUCTIONS:
  Follow these steps to make a unique version of any script:
    1. Select the script and use Unity's duplicate command.
    2. Open the duplicate script and change its namespace to something other than what it is.
    3. The script is ready to be used!

  (Simply renaming the script will not work. When a script is created it is assigned an ID called a GUID, and that is
   what a scene references, not the name. When Unity duplicates the script it creates a new unique ID, making it
   impervious to SoundReactor updates.)
*/

using UnityEngine;

namespace LDG.Demo
{
    using LDG.Core;

    public class RhythmNote : MonoBehaviourParticle
    {
        public struct Parameters
        {
            public Transform Parent;
            public int TouchIndex;
            public Color Color;
            public Color EmissionColor;
        }

        public RhythmInterface rhythmInterface;

        public TouchIndicatorController touchIndicatorController;

        // setting this to true will cause the notes to delete at the exact time that it should have
        // been touched. use it to calibrate the game object you use to indicate to the player when
        // they should touch the note.
        public bool debug = false;

        private float timer = 0.0f;

        private float touchTime;
        private float minTouchTime;
        private float maxTouchTime;

        private bool touched = false;
        private bool missed = false;

        private Animator animator;
        private NoteTouchIndicatorController.Parameters touchIndicatorParams;

        #region Methods
        override public void Initialize(object parameters)
        {
            Parameters p = (Parameters)parameters;
            float lifeTime;

            touchIndicatorParams.TouchIndex = p.TouchIndex;
            touchIndicatorParams.TouchedColor = p.Color;
            SetMaterialColor(p.Color);
            SetMaterialColor("_EmissionColor", p.EmissionColor);

            transform.SetParent(p.Parent);
            transform.transform.localPosition = Vector3.zero;

            timer = 0.0f;
            missed = false;
            touched = false;

            animator = GetComponent<Animator>();
            animator.speed = rhythmInterface.beatLinePosition / (rhythmInterface.audioStartDelay / rhythmInterface.playbackSpeed);
            animator.SetTrigger("trigger");

            lifeTime = 1.0f / animator.speed;

            touchTime = rhythmInterface.audioStartDelay;

            minTouchTime = touchTime - rhythmInterface.timingThreshold;
            maxTouchTime = touchTime + rhythmInterface.timingThreshold;

            Emit(lifeTime);
        }
        #endregion

        #region MonoBehaviours
        // Update is called once per frame
        new void Update()
        {
            base.Update();

            if (touched) return;

            timer += Time.deltaTime * rhythmInterface.playbackSpeed;

            // only check input if the note is ready to be touched
            if (timer > minTouchTime && timer < maxTouchTime)
            {
                if (touchIndicatorController && touchIndicatorController.TestTouchIndex(touchIndicatorParams.TouchIndex))
                {
                    rhythmInterface.IncrementHit();
                    animator.SetTrigger("touched");
                    ReturnParticle();

                    touchIndicatorController.DoTouched(touchIndicatorParams);
                    touched = true;
                }
            }

            // if this is true then we missed our chance to hit the note
            if (timer > maxTouchTime && !missed)
            {
                rhythmInterface.IncrementMissed();
                touchIndicatorController.DoMissed(touchIndicatorParams);
                missed = true;
            }

            if (debug)
            {
                if (timer > touchTime)
                {
                    rhythmInterface.IncrementHit();
                    animator.SetTrigger("touched");
                    ReturnParticle();

                    touchIndicatorController.DoTouched(touchIndicatorParams);
                    touched = true;
                }
            }
        }
        #endregion
    }
}