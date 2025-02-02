using System;
using LDG.SoundReactor;
using UnityEngine;

namespace Source.Scripts
{
    public class MidiEventHandler : MonoBehaviour
    {
        public void OnMidiEvent(Sequencer sequencer, MidiEvent e)
        {
            if (!e.IsChannelVoiceMessage) return;
            switch (e.ChannelVoiceMessage)
            {
                case ChannelVoiceMessage.NoteOff:
                    break;
                case ChannelVoiceMessage.NoteOn:
                    Debug.Log($"Playing note {e.Note} with hand {e.Channel} and with velocity {e.Velocity}");
                    break;
                case ChannelVoiceMessage.PolyphonicPressure:
                    break;
                case ChannelVoiceMessage.ControlChange:
                    break;
                case ChannelVoiceMessage.ProgramChange:
                    break;
                case ChannelVoiceMessage.ChannelPressure:
                    break;
                case ChannelVoiceMessage.PitchWheelChange:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}