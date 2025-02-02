using UnityEngine;

namespace LDG.SoundReactor
{
    using LDG.Core;

    public enum ViewAnchor
    {
        Top,
        Bottom
    }

    [ExecuteInEditMode]
    public class PianoRoll : MonoBehaviour
    {
        public AudioMidiSync audioMidiSync;
        public SpectrumBuilder spectrumBuilder;
        public PianoInfo pianoInfo;

        public MonoBehaviourParticlePool notePool;
        public Transform view;
        public bool inheritStartDelay = false;
        public float timeSpan;
        public float viewLength = 40;

        public ViewAnchor viewAnchor;

        public Color[] colors = new Color[] { Color.white };

        public bool valid { get { return audioMidiSync && view && pianoInfo && spectrumBuilder && notePool; } }

        private void Start()
        {
            if (valid)
            {
                Vector3 stencilScale = view.localScale;
                stencilScale.z = viewLength;
                view.localScale = stencilScale;
            }
            else
            {
                Debug.LogError("PianoRoll: Not all references are set");
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (valid)
            {
                float zPos = (viewAnchor == ViewAnchor.Top) ? 0.0f : viewLength;
                float dir = (viewAnchor == ViewAnchor.Top) ? -1f : 1f;

                Vector3 pos = view.localPosition;
                pos.z = 0;
                view.localPosition = pos;
                
                pos = notePool.transform.localPosition;
                pos.z = zPos;
                notePool.transform.localPosition = pos;
                 
                Vector3 stencilScale = view.localScale;
                stencilScale.z = viewLength * dir;
                stencilScale.x = spectrumBuilder.numColumns + spectrumBuilder.levelSize.x + 0.3f;
                view.localScale = stencilScale;
            }
        }
#endif
        public void ClearNotePool()
        {
            if(notePool)
            {
                notePool.DeactivateAll();
            }
        }

        public void OnMidiEvent(Sequencer sequencer, MidiEvent e)
        {
            if (!this.enabled || !gameObject.activeSelf || !valid) return;

            if (this.enabled && e.IsChannelVoiceMessage)
            {
                switch (e.ChannelVoiceMessage)
                {
                    case ChannelVoiceMessage.NoteOn:
                        if (e.Velocity != 0.0f && e.Note >= spectrumBuilder.firstKey && e.Note <= spectrumBuilder.lastKey)
                        {
                            PianoRollNote noteParticle;
                            PianoKeyInfo keyInfo = pianoInfo.GetPianoKeyInfo(e.Note);

                            float audioStartDelay = (audioMidiSync.AudioStartDelay == 0) ? 0.0001f : audioMidiSync.AudioStartDelay;
                            float timeSpan = (inheritStartDelay) ? audioStartDelay : this.timeSpan;

                            float spacing = spectrumBuilder.spacing;
                            float moveSpeed = viewLength / timeSpan;
                            float leftOfCenterOffset = (float)spectrumBuilder.numColumns * spacing / 2.0f - spacing * 0.5f;

                            // set the note particle's parameters
                            PianoRollNote.Parameters p = new PianoRollNote.Parameters()
                            {
                                Velocity = new Vector3(0.0f, 0.0f, -moveSpeed * audioMidiSync.PlaybackSpeed),
                                Position = new Vector3((e.Note - spectrumBuilder.firstKey) * spacing - leftOfCenterOffset + keyInfo.offset, 0.0f, 0.0f),
                                Size = new Vector3(keyInfo.width, 0.0f, Mathf.Max(moveSpeed * e.HoldTime, 0.5f)),
                                LifeTime = (e.HoldTime + timeSpan) / audioMidiSync.PlaybackSpeed,
                                Parent = notePool.transform,
                                Track = sequencer.TrackIndex
                            };

                            // get the next note particle
                            noteParticle = (PianoRollNote)notePool.GetParticle();

                            // initialize the note particle
                            noteParticle.Initialize(p);

                            noteParticle.SetMaterialColor(colors[sequencer.TrackIndex % colors.Length]);
                            noteParticle.SetMaterialFloat("_Width", p.Size.x);
                            noteParticle.SetMaterialFloat("_Length", p.Size.z);
                        }
                        break;
                }
            }
        }

    }
}