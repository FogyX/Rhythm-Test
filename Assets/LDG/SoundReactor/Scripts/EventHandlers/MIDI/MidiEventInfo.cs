using UnityEngine;
using UnityEngine.UI;

using LDG.SoundReactor;

public class MidiEventInfo : MonoBehaviour
{
    public Text noteCountText;
    public Text noteCountMaxText;
    public Text timeText;
    public Text tempoText;
    public Text divisionsText;
    public Text progressText;

    int noteCounter = 0;
    int noteCounterMax = 0;
    int progress = 0;
    int divisions = 480;
    Sequencer sequencer;

    private void Update()
    {
        if (sequencer != null)
        {
            progress = (int)(sequencer.Time / sequencer.MidiClip.length * 100);

            float t = sequencer.Time;
            string minSec = string.Format("{0:00}:{1:00}.{2:00}", (int)t / 60, (int)t % 60, (int)(t * 100.0f) % 100);

            timeText.text = minSec;
            noteCounter = sequencer.NoteCounter;
        }

        noteCountMaxText.text = noteCounterMax.ToString();
        noteCountText.text = noteCounter.ToString();
        progressText.text = progress.ToString() + "%";
        divisionsText.text = divisions.ToString();
    }

    public void OnMidiEvent(Sequencer sequencer, MidiEvent e)
    {
        if (!this.enabled || !gameObject.activeSelf) return;

        this.sequencer = sequencer;

        switch(sequencer.PlayState)
        {
            case LDG.SoundReactor.PlayState.End:
                //Debug.Log("end reached");
                break;

            case LDG.SoundReactor.PlayState.Play:
                //Debug.Log("play");
                divisions = sequencer.MidiClip.divisions;
                noteCounterMax = noteCounter;
                break;

            case LDG.SoundReactor.PlayState.Stop:
                //Debug.Log("stop");
                break;

            case LDG.SoundReactor.PlayState.Pause:
                //Debug.Log("pause");
                break;
        }

        // all possible channel voice messages
        if (e.IsChannelVoiceMessage)
        {
            switch(e.ChannelVoiceMessage)
            {
                case ChannelVoiceMessage.NoteOff:
                    // handle message
                    break;
                case ChannelVoiceMessage.NoteOn:
                    // handle message
                    break;
                case ChannelVoiceMessage.PolyphonicPressure:
                    // handle message
                    break;
                case ChannelVoiceMessage.ControlChange:
                    // handle message
                    break;
                case ChannelVoiceMessage.ProgramChange:
                    // handle message
                    break;
                case ChannelVoiceMessage.ChannelPressure:
                    // handle message
                    break;
                case ChannelVoiceMessage.PitchWheelChange:
                    // handle message
                    break;
            }
        }

        // all possible meta messages
        if (e.IsMetaMessage)
        {
            switch (e.MetaMessage.MetaType)
            {
                case MetaType.Text:
                    // handle message
                    //Debug.Log("text: " + e.MetaMessage.Text);
                    break;
                case MetaType.CopyrightNotice:
                    // handle message
                    //Debug.Log("copyright notice" + e.MetaMessage.CopyrightNotice);
                    break;
                case MetaType.TrackName:
                    // handle message
                    //Debug.Log("track name: " + e.MetaMessage.TrackName);
                    break;
                case MetaType.InstrumentName:
                    // handle message
                    //Debug.Log("instrument name: " + e.MetaMessage.InstrumentName);
                    break;
                case MetaType.Lyrics:
                    // handle message
                    //Debug.Log("lyrics: " + e.MetaMessage.Lyrics);
                    break;
                case MetaType.Marker:
                    // handle message
                    //Debug.Log("marker: " + e.MetaMessage.Marker);
                    break;
                case MetaType.CuePoint:
                    // handle message
                    //Debug.Log("cue point: " + e.MetaMessage.CuePoint);
                    break;
                case MetaType.ChannelPrefix:
                    // handle message
                    //Debug.Log("channel prefix: " + e.MetaMessage.ChannelPrefix);
                    break;
                case MetaType.EndOfTrack:
                    // handle message
                    //Debug.Log("end of track");
                    break;
                case MetaType.Tempo:
                    // convert midi tempo to beats per minute.
                    tempoText.text = (60000000 / e.MetaMessage.Tempo).ToString();

                    // same as above except the sequencer stores tempo as beats per second, so multiply by 60 to convert to beats per minute.
                    //tempoText.text = ((int)(sequencer.Tempo * 60.0)).ToString();
                    break;
                case MetaType.SMPTEOffset: // not supported
                    // handle message
                    /*
                    Debug.Log
                    (
                        "hours: " + e.MetaMessage.Hours +
                        ", minutes: " + e.MetaMessage.Minutes +
                        ", seconds: " + e.MetaMessage.Seconds +
                        ", frames: " + e.MetaMessage.Frames +
                        ", sub frames: " + e.MetaMessage.SubFrames
                    );
                    */
                    break;
                case MetaType.TimeSignature:
                    // handle message
                    /*
                    Debug.Log
                    (
                        "numerator: " + e.MetaMessage.Numerator +
                        ", denominator: " + e.MetaMessage.Denominator +
                        ", metronome pulse: " + e.MetaMessage.MetronomePulse +
                        ", 32th beat: " + e.MetaMessage.ThirtySecondthBeat
                    );
                    */
                    break;
                case MetaType.KeySignature:
                    // handle message
                    //Debug.Log("sf: " + e.MetaMessage.SharpFlat + ", scale: " + e.MetaMessage.Scale);
                    break;
                case MetaType.SequenceNumber:
                    // handle message
                    //Debug.Log("sequence number: " + e.MetaMessage.SeqeunceNumber);
                    break;
            }
        }
    }
}
