// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LDG.SoundReactor
{
    /// <summary>
    /// Used to identify the meta type for a meta message.
    /// </summary>
    public enum MetaType
    {
        /// <summary>
        /// Reserved as a placeholder
        /// </summary>
        None = -1,

        /// <summary>
        /// Optional sequence number. Should exist at the beginning of track 0.
        /// </summary>
        SequenceNumber = 0x0,

        /// <summary>
        /// Optional text. Can exist anywhere along any track.
        /// </summary>
        Text = 0x01,

        /// <summary>
        /// Optional text. Should exist at the beginning of track 0.
        /// </summary>
        CopyrightNotice = 0x02,

        /// <summary>
        /// Optional text. Name of the track or sequence name. Sequence name is implied if it exists at the start of track 0.
        /// </summary>
        TrackName = 0x03,

        /// <summary>
        /// Optional text. Should exist at the beginning of a track.
        /// </summary>
        InstrumentName = 0x04,

        /// <summary>
        /// Optional text. Syllable or word to be sung. Can exist anywhere along any track.
        /// </summary>
        Lyrics = 0x05,

        /// <summary>
        /// Optional text. General information used during editing by the musician/editor. Can exist anywhere along any track.
        /// </summary>
        Marker = 0x06,

        /// <summary>
        /// Optional text. Used to cue actors, camera movements, stage hands, and so on. Can exist anywhere along track 0.
        /// </summary>
        CuePoint = 0x07,

        /// <summary>
        /// Optional text. Name of the patch/program. Will be called before ProgramChange events.
        /// </summary>
        ProgramName = 0x08,

        /// <summary>
        /// Optional text. Name of the hardware that created the MIDI. This will be called before ProgramName. Irrevelant to Sound Reactor.
        /// </summary>
        DeviceName = 0x09,

        /// <summary>
        /// Not supported.
        /// </summary>
        ChannelPrefix = 0x20,

        /// <summary>
        /// Not supported.
        /// </summary>
        MidiPort = 0x21,

        /// <summary>
        /// The very last event in a track marking the end of the track.
        /// </summary>
        EndOfTrack = 0x2F,

        /// <summary>
        /// Optional tempo, otherwise assume 120 beats per minute. Can exist anywhere along track 0.
        /// </summary>
        Tempo = 0x51,

        /// <summary>
        /// Not supported.
        /// </summary>
        SMPTEOffset = 0x54,

        /// <summary>
        /// Optional time signature. Defines the time signature for the current MIDI, otherwise assume 4/4 timing. Can exist anywhere along track 0.
        /// </summary>
        TimeSignature = 0x58,

        /// <summary>
        /// Defines the number of sharps/flats in the key signature. Can exist anywhere along track 0.
        /// </summary>
        KeySignature = 0x59,

        /// <summary>
        /// Not supported.
        /// </summary>
        SequencerSpecific = 0x7F
    }

    [Serializable]
    public class MetaMessage
    {
        #region Fields
        /// <summary>
        /// Type of meta data. Use this to determine which helper property to use.
        /// </summary>
        [SerializeField]
        private MetaType metaType;
        public MetaType MetaType { get { return metaType; } }

        /// <summary>
        /// Meta message data bytes. Interpret this with one of the appropriate class properties.
        /// </summary>
        [SerializeField]
        private byte[] data;
        #endregion

        #region Properties: helpers to interpret data
        /// <summary>
        /// Tempo: Microseconds per beat. For beats per second it's: 1,000,000 / Tempo. For beats per minute it's: 60,000,000 / Tempo.
        /// </summary>
        public int Tempo { get { return ((data[2] << 16) | (data[1] << 8) | (data[0])); } }

        /// <summary>
        /// SequenceNumber: This is a MIDI format 2 feature which is not supported.
        /// </summary>
        public int SeqeunceNumber { get { return ((data[1] << 8) | (data[0])); } }

        /// <summary>
        /// KeySignature: Sharp and Flat identifier. Negative numbers are flats, and positive numbers are sharps. 0 is key of C.
        /// </summary>
        public sbyte SharpFlat { get { return (sbyte)data[0]; } }

        /// <summary>
        /// Minor or Major scale to be used. 0 is Major, and 1 is Minor.
        /// </summary>
        public byte Scale { get { return data[1]; } }

        /// <summary>
        /// Channel to apply subsequent meta messages to.
        /// </summary>
        public byte ChannelPrefix { get { return data[0]; } }

        /// <summary>
        /// TimeSignature: Beats per measure.
        /// </summary>
        public byte Numerator { get { return data[0]; } }

        /// <summary>
        /// TimeSignature: Quarter notes per beat.
        /// </summary>
        public byte Denominator { get { return data[1]; } }

        /// <summary>
        /// TimeSignature: Ticks per metronome click.
        /// </summary>
        public byte MetronomePulse { get { return data[2]; } }

        /// <summary>
        /// TimeSignature: Notes per quarter note.
        /// </summary>
        public byte ThirtySecondthBeat { get { return data[3]; } }

        /// <summary>
        /// SMPTE: Frames per second. 0 = 24, 1 = 25, 2 = 29.97, 3 = 30
        /// </summary>
        public byte FpsCode { get { return (byte)(data[0] >> 5); } }

        /// <summary>
        /// SMPTE: Hours since the MIDI started playing.
        /// </summary>
        public byte Hours { get { return (byte)(0x1F & data[0]); } }

        /// <summary>
        /// SMPTE: Minutes into the current hour.
        /// </summary>
        public byte Minutes { get { return data[1]; } }

        /// <summary>
        /// SMPTE: Seconds into the current minute.
        /// </summary>
        public byte Seconds { get { return data[2]; } }

        /// <summary>
        /// SMPTE: not sure
        /// </summary>
        public byte Frames { get { return data[3]; } }

        /// <summary>
        /// SMPTE: not sure
        /// </summary>
        public byte SubFrames { get { return data[4]; } }

        /// <summary>
        /// Text: Generic text that can exist anywhere during a sequence.
        /// </summary>
        public string Text { get { return Regex.Replace(Encoding.UTF8.GetString(data), @"[^\u0009\u000A\u000D\u0020-\u007E]", ""); } }

        /// <summary>
        /// CopyrightNotice: Copyright notice that exists at delta time 0.
        /// </summary>
        public string CopyrightNotice { get { return Text; } }

        /// <summary>
        /// TrackName: Name of the track and exists at delta time 0.
        /// </summary>
        public string TrackName { get { return Text; } }

        /// <summary>
        /// InstrumentName: Name of the instrument being played. This can exist anywhere in the sequence.
        /// </summary>
        public string InstrumentName { get { return Text; } }

        /// <summary>
        /// Lyrics: Words and phrases which can exist anywhere in a sequence.
        /// </summary>
        public string Lyrics { get { return Text; } }

        /// <summary>
        /// Marker: Marker that can exist anywhere in a sequence.
        /// </summary>
        public string Marker { get { return Text; } }

        /// <summary>
        /// CuePoint: Cue point that can exist anywhere in a sequence.
        /// </summary>
        public string CuePoint { get { return Text; } }
        #endregion

        #region Public Methods
        public void SetString(MetaType type, string value)
        {
            switch(type)
            {
                // the following are all strings
                case MetaType.Text:
                case MetaType.CopyrightNotice:
                case MetaType.TrackName:
                case MetaType.InstrumentName:
                case MetaType.Lyrics:
                case MetaType.Marker:
                case MetaType.CuePoint:
                    data = Encoding.ASCII.GetBytes(value);
                    metaType = type;
                    break;
            }
        }

        /// <summary>
        /// Set the tempo in beats per minute.
        /// </summary>
        /// <param name="tempo">Beats per minute.</param>
        public void SetTempo(int tempo)
        {
            metaType = MetaType.Tempo;
            data = BitConverter.GetBytes(60000000 / tempo);
            Array.Resize(ref data, 3);
        }

        public void SetEndOfTrack()
        {
            metaType = MetaType.EndOfTrack;
            data = new byte[] { };// Array.Empty<byte>();
        }

        public void Read(MidiReader reader)
        {
            MetaType metaType = (MetaType)reader.ReadByte();
            int length = (int)reader.ReadVLQ();

            this.metaType = metaType;

            switch (metaType)
            {
                // the following two messages need to consider endian order.
                case MetaType.Tempo:
                case MetaType.SequenceNumber:
                    data = reader.ReadBytes(length, true);
                    break;

                // the rest of the messages are read one byte at a time in the order they exist in the file.

                // special case message since data[2] needs to be converted to a 2^dd
                case MetaType.TimeSignature:
                    data = reader.ReadBytes(length);
                    data[2] = (byte)Math.Pow(2, data[2]);
                    break;
                
                // contains array of byte values but are not strings
                case MetaType.ChannelPrefix:
                case MetaType.SMPTEOffset:
                case MetaType.KeySignature:

                // the following are all strings
                case MetaType.Text:
                case MetaType.CopyrightNotice:
                case MetaType.TrackName:
                case MetaType.InstrumentName:
                case MetaType.Lyrics:
                case MetaType.Marker:
                case MetaType.CuePoint:
                    data = reader.ReadBytes(length);
                    break;
                
                // skip past the sequencer specific messages since these can be quite large.
                case MetaType.SequencerSpecific:
                    reader.BaseStream.Position += length;
                    break;

                case MetaType.EndOfTrack:
                    data = new byte[] { };// Array.Empty<byte>();
                    break;

                // skip any unknowns just in case (although I'm pretty sure I've covered them all)
                default:
                    Debug.Log(String.Format("unknown meta message: {0}", metaType.ToString("X")));
                    reader.BaseStream.Position += length;
                    break;
            }
        }

        public void Write(MidiWriter writer)
        {
            writer.Write((byte)metaType);
            writer.WriteVLQ(data.Length);
            
            switch (metaType)
            {
                // the following two messages need to consider endian order.
                case MetaType.Tempo:
                case MetaType.SequenceNumber:
                    writer.WriteBytes(data, true);
                    break;

                // the rest of the messages are read one byte at a time in the order they exist in the file.

                // special case message since data[2] needs to be converted to a 2^dd
                case MetaType.TimeSignature:
                    byte temp = data[2];
                    data[2] = (byte)(Math.Log10(data[2]) / Math.Log10(2));
                    writer.Write(data);
                    data[2] = temp;
                    
                    break;

                // contains array of byte values but are not strings
                case MetaType.ChannelPrefix:
                case MetaType.SMPTEOffset:
                case MetaType.KeySignature:

                // the following are all strings
                case MetaType.Text:
                case MetaType.CopyrightNotice:
                case MetaType.TrackName:
                case MetaType.InstrumentName:
                case MetaType.Lyrics:
                case MetaType.Marker:
                case MetaType.CuePoint:
                    writer.Write(data);
                    break;
                case MetaType.EndOfTrack:
                    break;

                default:
                    Debug.LogError("unknown meta type");
                    break;
            }
        }
        #endregion
    }
}