using UnityEngine;

namespace LDG.SoundReactor
{
    [System.Serializable]
    public struct PianoKeyInfo
    {
        static public PianoKeyInfo None;

        public float width;
        public float offset;
    }

    [CreateAssetMenu(fileName = "NewPianoInfo", menuName = "SoundReactor/Piano Info", order = 300)]
    public class PianoInfo : ScriptableObject
    {
        // There must be 12 of these, one for each of the 12 unique keys on a piano.
        public PianoKeyInfo[] pianoKeyInfos;

        public void SetDefault()
        {
            pianoKeyInfos = new PianoKeyInfo[]
            {
                new PianoKeyInfo() { width = 1.641559f, offset = 0.3571379f },
                new PianoKeyInfo() { width = 0.829786f, offset = -0.002607569f },
                new PianoKeyInfo() { width = 1.641559f, offset = 0.07142365f },
                new PianoKeyInfo() { width = 0.829786f, offset = 0.1454417f },
                new PianoKeyInfo() { width = 1.641559f, offset = -0.2142913f },
                new PianoKeyInfo() { width = 1.641559f, offset = 0.4999928f },
                new PianoKeyInfo() { width = 0.829786f, offset = 0.1402352f },
                new PianoKeyInfo() { width = 1.641559f, offset = 0.214278f },
                new PianoKeyInfo() { width = 0.829786f, offset = 0.07141566f },
                new PianoKeyInfo() { width = 1.641559f, offset = -0.07143688f },
                new PianoKeyInfo() { width = 0.829786f, offset = 0.002607822f },
                new PianoKeyInfo() { width = 1.641559f, offset = -0.3571522f },
            };
        }

        /// <summary>
        /// Get's the specified PianoKeyInfo for a given MIDI note.
        /// </summary>
        /// <param name="note">A MIDI note [0,128]</param>
        /// <returns></returns>
        public PianoKeyInfo GetPianoKeyInfo(int note)
        {
            if (pianoKeyInfos == null || pianoKeyInfos.Length == 0)
            {
                return PianoKeyInfo.None;
            }

            return pianoKeyInfos[note % 12];
        }
    }
}