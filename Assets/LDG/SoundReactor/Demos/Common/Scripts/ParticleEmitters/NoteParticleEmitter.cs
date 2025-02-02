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
    using LDG.SoundReactor;
    using LDG.Core;

    public class NoteParticleEmitter : MonoBehaviour
    {
        public enum ColorMode { Track, Note };

        public MonoBehaviourParticlePool notePool;
        public Vector3 velocity = new Vector3(0.0f, 0.0f, -10.0f);
        public float lifeTime = 1.0f;

        public ColorMode colorMode;
        public Color[] colors = new[] { Color.yellow, Color.blue, Color.magenta };

        public void OnLevel(PropertyDriver driver)
        {
            Color color;

            if (notePool != null && driver.isBeat)
            {
                if (colorMode == ColorMode.Note)
                {
                    color = colors[driver.GetMidiNote().NoteIndex % colors.Length];
                }
                else
                {
                    color = colors[driver.GetMidiNote().TrackIndex % colors.Length];
                }

                NoteParticle.Parameters p = new NoteParticle.Parameters
                {
                    Velocity = velocity,
                    Position = Vector3.zero,
                    Size = Vector3.one,
                    LifeTime = lifeTime,
                    Parent = transform,
                    Track = 0
                };

                NoteParticle np = (NoteParticle)notePool.GetParticle();
                np.Initialize(p);
                np.SetMaterialColor("_Color", color);
                np.SetMaterialColor("_EmissionColor", color);
            }
        }
    }
}