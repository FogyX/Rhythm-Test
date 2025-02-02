using UnityEngine;

namespace LDG.SoundReactor
{
    public class InputDevice : MonoBehaviour
    {
        public bool listenOnAwake = true;

        // Use this for initialization
        void Awake()
        {
            if (listenOnAwake)
            {
                Listen();
            }
        }

        public bool Listen()
        {
            AudioSource audio = GetComponent<AudioSource>();

            // check and see if there are any input devices and if there's an audio source
            if (Microphone.devices.Length > 0 || audio != null)
            {
                // cycle through all the devices and use the one that can recording
                for (int i = 0; i < Microphone.devices.Length; i++)
                {
                    // attempt recording
                    audio.clip = Microphone.Start(Microphone.devices[i], true, 10, 44100);
                    audio.loop = true;

                    // should be recording already but we'll check anyway just to make sure
                    if (Microphone.IsRecording(Microphone.devices[i]))
                    {
                        // the input device is recording, but we need to wait for it to initialize
                        while (!(Microphone.GetPosition(Microphone.devices[i]) > 0)) { }

                        audio.Play();

                        return true;
                    }
                }
            }

            return false;
        }
    }
}