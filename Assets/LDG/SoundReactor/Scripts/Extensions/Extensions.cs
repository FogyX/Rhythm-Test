using UnityEngine;

namespace LDG.SoundReactor
{
    public static class Extensions
    {
        public static float Time(this AudioSource audioSource)
        {
            return (float)((double)audioSource.timeSamples / (double)audioSource.clip.frequency);
        }
    }
}