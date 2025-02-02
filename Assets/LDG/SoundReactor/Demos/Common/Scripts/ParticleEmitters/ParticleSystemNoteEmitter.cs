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

    public class ParticleSystemNoteEmitter : MonoBehaviour
    {
        public Color[] colors = new Color[] { Color.white };
        public int noteOffset = -15;
        public int keys = 88;
        public float spacing = 1;
        public float moveSpeed = 5;
        public float lifetime = 5;

        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();

        private ParticleSystem ps = null;

        private void Start()
        {
            if(!(ps = GetComponent<ParticleSystem>()))
            {
                Debug.LogWarning("ParticleEmitterDriver can't find a ParticleSystem.", this);
                this.enabled = false;
            }
        }

        public void OnMidiEvent(Sequencer seqeuncer, MidiEvent e)
        {
            if (!this.enabled || !gameObject.activeSelf) return;

            float offset;

            if (e.IsChannelVoiceMessage)
            {
                switch (e.ChannelVoiceMessage)
                {
                    case ChannelVoiceMessage.NoteOn:
                        if (e.Velocity != 0 && e.Note > 0 && e.Note <= 128)
                        {
                            offset = (float)keys * spacing / 2.0f - spacing * 0.5f;

                            ep.startLifetime = lifetime;
                            ep.position = new Vector3((e.Note + noteOffset) * spacing - offset, 0, 0);
                            ep.startColor = colors[seqeuncer.TrackIndex % colors.Length];
                            ep.velocity = new Vector3(0, 0, moveSpeed);

                            ps.Emit(ep, 1);
                        }

                        break;
                }
            }
        }
    }
}
