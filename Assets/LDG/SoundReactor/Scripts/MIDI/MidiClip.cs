// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

namespace LDG.SoundReactor
{
    /// <summary>
    /// Contains MIDI header info and MidiTracks.
    /// </summary>
    public class MidiClip : ScriptableObject
    {
        private void OnEnable()
        {
            AssignMetaMessages();
        }

        #region Properties
        /// <summary>
        /// Ticks per beat.
        /// </summary>
        public short divisions { get { return header.Division; } }

        /// <summary>
        /// Number of ticks in this clip.
        /// </summary>
        public int ticks { get { return header.Ticks; } }

        /// <summary>
        /// Returns the number of tracks in this clip.
        /// </summary>
        public int numTracks { get { return (tracks != null) ? tracks.Length : 0; } }

        /// <summary>
        /// The length of the MIDI clip in seconds.
        /// </summary>
        public float length { get { return header.Length; } }

        /// <summary>
        /// Number of notes in this clip.
        /// </summary>
        public int noteCount { get { return header.NoteCount; } }
        #endregion

        #region Fields
        public Object midiFile;

        /// <summary>
        /// Beats per minute
        /// </summary>
        public short defaultTempo = 120;

        /// <summary>
        /// Used in the preview window for the track selector.
        /// </summary>
        [SerializeField]
        public int trackPreviewIndex = 0;

        [SerializeField]
        private Track[] tracks;

        [SerializeField]
        private MetaMessage[] metaMessages;

        [SerializeField]
        private Header header;
        #endregion

        public static event System.Func<MidiEvent, object> OnAssignCustomData;

        #region Editor
#if UNITY_EDITOR
        public void Draw(float width, float height)
        {
            bool drawPreview = false;

            for (int i = 0; i < numTracks; i++)
            {
                if (i != trackPreviewIndex)
                {
                    tracks[i].Draw(width, height, Mathf.Min(30, length), Color.black);
                }
                else
                {
                    drawPreview = true;
                }
            }

            if (drawPreview)
            {
                tracks[trackPreviewIndex].Draw(width, height, Mathf.Min(30, length), Color.green);
            }

        }
#endif
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Returns true if this MIDI file is supported. 
        /// </summary>
        static public bool Validate(string path)
        {
            bool valid = false;

            if (!File.Exists(path)) return false;

            if (File.GetAttributes(path) == FileAttributes.Directory) return false;

            using (MidiReader reader = new MidiReader(File.OpenRead(path), false))
            {
                string chunkId;
                short format;

                // make sure the file is large enough to at least contain a chunkId
                if (reader.BaseStream.Length < 4) return false;

                // get the chunkId
                chunkId = Encoding.UTF8.GetString(reader.ReadBytes(4));

                // validate the file, return false if it's invalid
                if (chunkId != "MThd") return false;

                // skip chunkSize
                reader.BaseStream.Position += 4;

                format = reader.ReadInt16();

                valid = (format <= 1);
            }

            return valid;
        }

        public List<MidiEvent> GetChannelVoiceMessages(int trackIndex)
        {
            List<MidiEvent> midiEvents = new List<MidiEvent>();

            if (trackIndex < tracks.Length)
            {
                tracks[trackIndex].GetChannelVoiceMessages(trackIndex, midiEvents);
            }

            return midiEvents;
        }

        /// <summary>
        /// Enumerates over all the ChannelVoiceMessages of a particular track. These are notes in midi speak.
        /// </summary>
        public IEnumerable<MidiEvent> EnumerateChannelVoiceMessages(int trackIndex)
        {
            if (tracks == null || tracks.Length == 0 || trackIndex >= tracks.Length) yield break;

            foreach (MidiEvent me in tracks[trackIndex].ChannelVoiceMessages)
            {
                yield return me;
            }
        }

        /// <summary>
        /// Enumerates over all the MetaMessages of a particular track. These contain tempo, trackname, and other such meta information.
        /// </summary>
        public IEnumerable<MidiEvent> EnumerateMetaMessages(int trackIndex)
        {
            if (tracks == null || tracks.Length == 0 || trackIndex >= tracks.Length) yield break;

            foreach (MidiEvent me in tracks[trackIndex].MetaMessages)
            {
                MidiEvent meMetaMessage = me;
                meMetaMessage.MetaMessage = metaMessages[me.MetaMessageIndex];

                yield return meMetaMessage;
            }
        }

        /// <summary>
        /// Enumerates over all the MidiEvents of a particular track. These will include both ChannelVoiceMessage and MetaMessage.
        /// Check the isChannelVoice and isMetaMessage to see what kind of MidiEvent is current.
        /// </summary>
        public IEnumerable<MidiEvent> EnumerateMidiEvents(int trackIndex)
        {
            if (tracks == null || tracks.Length == 0 || trackIndex >= tracks.Length) yield break;

            foreach (MidiEvent midiEvent in tracks[trackIndex].MidiEvents)
            {
                MidiEvent me = midiEvent;

                if (me.IsMetaMessage)
                {    
                    me.MetaMessage = metaMessages[me.MetaMessageIndex];
                }

                yield return me;
            }
        }
        #endregion

        #region Public Methods

        public string[] GetTrackNames()
        {
            // no tracks, no names
            if (numTracks <= 0) return null;

            string[] trackNames = new string[numTracks];

            // populate cached names
            for(int i = 0; i < numTracks; i++)
            {
                trackNames[i] = tracks[i].Name;
            }

            // return newly aquired cached names
            return trackNames;
        }

        public IEnumerable<string> TrackNames
        {
            get
            {
                // no tracks, no names
                if (numTracks <= 0) yield break;

                // populate cached names
                for (int i = 0; i < numTracks; i++)
                {
                    yield return tracks[i].Name;
                }
            }
        }

        public string GetTrackName(int index)
        {
            // no tracks, no names
            if (numTracks <= 0) return "";

            return tracks[index].Name;
        }

        /// <summary>
        /// Seeks to a specific time in a MIDI clip. Pass in a handler to handle events. MidiSource OnMidiEvent is not called during seek.
        /// </summary>
        public void Seek(SequencerInternal sequencerInternal, float time, MidiEventHandler midiEventHandler)
        {
            if (tracks != null)
            {
                for (short i = 0; i < tracks.Length; i++)
                {
                    tracks[i].Seek(sequencerInternal, time, metaMessages, midiEventHandler);
                }

                sequencerInternal.Time = time;
            }
        }

        /// <summary>
        /// Increments the internal current event index for each track and posts events that have been triggered.
        /// </summary>
        public bool UpdateTracks(SequencerInternal sequencerInternal, int noteOffset)
        {
            // assume all tracks are done playing. the fancy logic below ensures that once running is set to true, is stays true.
            bool running = false;
            
            if(tracks != null)
            {
                // process track 0 first since it has tempo meta messages.
                running = (tracks[0].Update(sequencerInternal, metaMessages, noteOffset)) ? true : running;

                // process remaining tracks in reverse order.
                //for (short i = 0; i < timeline.midiClip.trackCount; i++)
                for (short i = (short)(sequencerInternal.Clip.numTracks - 1); i >= 1; i--)
                {
                    running = (tracks[i].Update(sequencerInternal, metaMessages, noteOffset)) ? true : running;
                }
            }

            return running;
        }

        /// <summary>
        /// Reads MIDI file.
        /// </summary>
        public void Read(string path)
        {
            Track midiTrack;
            string chunkId;
            uint chunkSize;
            short trackIndex = 1;
            ushort metaMessageIndex = 0;
            List<MetaMessage> metaMessageList = new List<MetaMessage>();
            List<Track> midiTrackList = new List<Track>();

            // used to remember the file position just after getting the chunk info
            long filePos;

            try
            {
                using (MidiReader reader = new MidiReader(File.OpenRead(path), false))
                {
                    // read header information and abort if it's not a valid midi file
                    if (!ReadHeader(reader))
                    {
                        reader.Close();
                        return;
                    }

                    // read until we reach the end of the file
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        // read chunk data
                        chunkId = Encoding.UTF8.GetString(reader.ReadBytes(4));
                        chunkSize = reader.ReadUInt32();

                        // remember the file position so it can be used to skip chunks we don't understand
                        filePos = reader.BaseStream.Position;
                        
                        switch (chunkId)
                        {
                            case "MTrk":

                                // allocate a track. assign a default name. the name will be replaced if one exists in the MIDI file.
                                midiTrack = new Track((short)(trackIndex - 1), "Track " + trackIndex.ToString());

                                midiTrack.Read(reader, metaMessageList, ref metaMessageIndex, filePos + chunkSize);

                                if (midiTrack.IsValid)
                                {
                                    midiTrackList.Add(midiTrack);
                                    trackIndex++;
                                }

                                break;

                            default:
                                // if I understand the MIDI file correctly, this will never be reached, but
                                // if it does, skip the unknown chunk.
                                reader.BaseStream.Position = filePos + chunkSize;

                                break;
                        }
                    }

                    metaMessages = metaMessageList.ToArray();
                    tracks = midiTrackList.ToArray();

                    CalculateTime(defaultTempo);
                    CountNotes();

                    AssignMetaMessages();
                    AssignCustomData();

                    reader.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void AssignMetaMessages()
        {
            if (tracks == null || metaMessages == null) return;
            
            foreach (Track track in tracks)
            {
                track.AssignMetaMessages(metaMessages);
            }
        }

        public void AssignCustomData()
        {
            if (tracks == null || OnAssignCustomData == null) return;

            foreach (Track track in tracks)
            {
                track.AssignCustomData(OnAssignCustomData);
            }
        }
        #endregion

        #region Private Methods
        private bool ReadHeader(MidiReader reader)
        {
            string chunkId;

            // make sure the file is large enough to at least contain a chunkId
            if (reader.BaseStream.Length < 4) return false;

            // get the chunkId
            chunkId = Encoding.UTF8.GetString(reader.ReadBytes(4));

            // validate the file, return false if it's invalid
            if (chunkId != "MThd") return false;
            
            // skip chunkSize
            reader.BaseStream.Position += 4;

            header.Format = reader.ReadInt16();
            header.NumTracks = reader.ReadInt16();
            header.Division = reader.ReadInt16();

            if(header.Format <= 1)
            {
                return true;
            }

            Debug.LogError("Unsupported MIDI file format");

            return false;
        }

        private void CountNotes()
        {
            header.NoteCount = 0;

            for (int i = 0; i < tracks.Length; i++)
            {
                header.NoteCount += tracks[i].CountNotes();
            }
        }

        private void CalculateTime(int defaultTempo)
        {
            // create the tempo time map. this is used to normalize the time to seconds.
            TempoRegion[] tempoRegions = tracks[0].CreateTempoRegions(metaMessages, header, defaultTempo);

            // normalize time for all the tracks. this MUST occur before calculating length in seconds and ticks.
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].RemapTicksToTime(tempoRegions, header.Division);
                tracks[i].CalculateHoldTime();
            }

            // calculate both the length in seconds and ticks
            header.Length = GetSequenceLength();
            header.Ticks = GetSequenceTicks();
        }

        private float GetSequenceLength()
        {
            float length = 0.0f;

            for (int i = 0; i < tracks.Length; i++)
            {
                length = tracks[i].GetMaxLength(length);
            }

            return length;
        }

        private int GetSequenceTicks()
        {
            int ticks = 0;

            for (int i = 0; i < tracks.Length; i++)
            {
                ticks = tracks[i].GetMaxTicks(ticks);
            }

            return ticks;
        }
        #endregion
    }
}
