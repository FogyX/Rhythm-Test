using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    public class TempoUtil
    {
        private LinkedList<TempoRegion> tempoRegions = new LinkedList<TempoRegion>();
        private LinkedListNode<TempoRegion> tempoRegionNode;
        
        private int divisions = 96;

        public TempoUtil(int divisions = 96)
        {
            this.divisions = divisions;

            tempoRegions.AddFirst(TempoRegion.Default);
        }

        /// <summary>
        /// Inserts a tempo in beats per second.
        /// </summary>
        /// <param name="ticks">Position in ticks in the linked list.</param>
        /// <param name="tempo">Beats per second.</param>
        public void Insert(int ticks, double tempo)
        {
            LinkedListNode<TempoRegion> node = tempoRegions.Last;
            TempoRegion tempoRegion;
            TempoRegion prev = TempoRegion.Default;

            int deltaTicks = 0;
            int durationTicks =  0;

            while(node != null)
            {
                if(node.Previous != null)
                {
                    prev = node.Previous.Value;
                }

                if(node.Next != null)
                {
                    durationTicks = node.Next.Value.Ticks - ticks;
                }

                deltaTicks = ticks - prev.Ticks;

                tempoRegion = new TempoRegion(ticks, deltaTicks, durationTicks, tempo, prev.Time + deltaTicks / prev.Tempo / divisions);

                // update current value with new value
                if (node.Value.Ticks == ticks)
                {
                    node.Value = tempoRegion;

                    node = (node.Previous != null) ? node.Previous : node;

                    Refresh(node);

                    break;
                }

                // add a new tempo region to the list
                if (ticks > node.Value.Ticks)
                {
                    tempoRegions.AddAfter(node, tempoRegion);

                    // rewind by 1 so that the current node will get updated in the subsequent pass
                    node = (node.Previous != null) ? node.Previous : node;

                    Refresh(node);

                    break;
                }

                node = node.Previous;
            }
        }

        public void Remove(int ticks)
        {
            LinkedListNode<TempoRegion> node = tempoRegions.First;
            LinkedListNode<TempoRegion> previous;

            while (node != null)
            {
                if(node.Value.Ticks == ticks)
                {
                    previous = node.Previous;

                    tempoRegions.Remove(node);

                    Refresh(previous);

                    break;
                }

                node = node.Next;
            }

            if(tempoRegions.First == null)
            {
                tempoRegions.AddFirst(TempoRegion.Default);
            }
        }

        public void Clear()
        {
            tempoRegions.Clear();
            tempoRegions.AddFirst(TempoRegion.Default);
        }

        public double HoldTicksToSeconds(int ticks, int holdTicks)
        {
            return TicksToSeconds(ticks + holdTicks) - TicksToSeconds(ticks);
        }

        public double PlayheadTicksToNormalizedTime(int playheadTicks, int noteTicks, int holdTicks)
        {
            double noteTime = TicksToSeconds(noteTicks);
            double curTime = TicksToSeconds(playheadTicks);
            double holdEndTime = TicksToSeconds(noteTicks + holdTicks);

            return Mathf.Repeat((float)((noteTime - curTime) / (holdEndTime - curTime)), 1.0f);
        }

        public double TicksToSeconds(double ticks)
        {
            LinkedListNode<TempoRegion> node = tempoRegions.Last;

            double seconds = 0;

            while (node != null)
            {
                if (ticks >= node.Value.Ticks)
                {
                    seconds = node.Value.TicksToTime(ticks, divisions);
                    break;
                }

                node = node.Previous;
            }

            return seconds;
        }

        public double SecondsToTicks(double seconds)
        {
            LinkedListNode<TempoRegion> node = tempoRegions.Last;

            double ticks = 0;

            while (node != null)
            {
                if (seconds >= node.Value.Time)
                {
                    ticks = node.Value.TimeToTicks(seconds, divisions);
                    break;
                }

                node = node.Previous;
            }

            return ticks;
        }

        public void DebugPrint()
        {
            LinkedListNode<TempoRegion> node = tempoRegions.First;

            while (node != null)
            {
                Debug.Log($"tempo: {node.Value.Tempo}, delta: {node.Value.DeltaTicks}, duration: {node.Value.DurationTicks}, time: {node.Value.Time}");

                node = node.Next;
            }

            Debug.Log("========================================");
        }


        /// <summary>
        /// Used for converting ticks to seconds. The number of time maps will equal the number of tempo entries
        /// in the MIDI file. The time maps are only allocated once when converting a .mid file to the .mid.asset
        /// file.
        /// </summary>
        public static TempoRegion[] CreateTempoRegions(MidiEvent[] midiEvents, MetaMessage[] metaMessages, Header header, int defaultTempo)
        {
            List<TempoRegion> tempoRegions = new List<TempoRegion>();
            MidiEvent me = new MidiEvent();

            double time = 0; // time is in seconds

            int maxTicks = 0;

            // create and intialize default timeMap
            TempoRegion tempoRegion = new TempoRegion
            {
                Ticks = 0,
                DeltaTicks = 0,
                Tempo = 2.0 * (double)defaultTempo / 120.0, // the first must be set to the default tempo,
                Time = 0
            };

            // remember the last message (as if there was one).
            TempoRegion tempoRegionPrev = tempoRegion;

            tempoRegions.Add(tempoRegion);

            for (int i = 0; i < midiEvents.Length; i++)
            {
                // use copy of midi event so expressions are shorter
                me = midiEvents[i];

                // check if this is a meta message
                if (me.IsMetaMessage)
                {
                    // use copy of meta message so expressions are shorter
                    MetaMessage mm = metaMessages[me.MetaMessageIndex];

                    // record the new tempo entry
                    if (mm.MetaType == MetaType.Tempo)
                    {
                        tempoRegion = new TempoRegion
                        {
                            Ticks = me.Ticks,
                            Tempo = 1000000.0 / (double)mm.Tempo,
                            DeltaTicks = me.Ticks - tempoRegionPrev.Ticks
                        };

                        // accumulate the time in seconds for this region
                        time += (double)tempoRegion.DeltaTicks / tempoRegionPrev.Tempo / (double)header.Division;

                        // assign absolute time
                        tempoRegion.Time = time;

                        // add our completed tempoMessage to the list
                        tempoRegions.Add(tempoRegion);

                        // remember previous map
                        tempoRegionPrev = tempoRegion;
                    }
                }

                maxTicks = me.Ticks;
            }

            // calculate the tempo duration in ticks
            {
                int i = 0;

                for (; i < tempoRegions.Count - 1;)
                {
                    tempoRegion = tempoRegions[i];
                    tempoRegion.DurationTicks = tempoRegions[i + 1].Ticks - tempoRegions[i].Ticks;
                    tempoRegions[i] = tempoRegion;

                    i++;
                }

                tempoRegion = tempoRegions[i];
                tempoRegion.DurationTicks = maxTicks - tempoRegion.Ticks;
                tempoRegions[i] = tempoRegion;
            }

            return tempoRegions.ToArray();
        }
		
		public static void RemapTicksToTimeA(MidiEvent[] midiEvents, TempoRegion[] tempoRegions, int division)
        {
            int regionIndex = 0;

            // calculate absolute time
            for (int i = 0; i < midiEvents.Length; i++)
            {
                if (midiEvents[i].Ticks >= tempoRegions[regionIndex].Ticks)
                {
                    // start looking backward to find map ticks that are just short of, or equal to, event ticks.
                    for (int j = tempoRegions.Length - 1; j >= 0; j--)
                    {
                        if (tempoRegions[j].Ticks <= midiEvents[i].Ticks)
                        {
                            // remap ticks to seconds
                            midiEvents[i].Time = (float)tempoRegions[j].TicksToTime(midiEvents[i].Ticks, division);

                            // remember the index
                            regionIndex = j;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is the new, yet for some reason not faster, version of RemapTicksToTimeA. This one is easier to understand :D
        /// 
        /// In a nutshell, all notes are checked to see if they're within tempo regions. If a note is within a region, then its
        /// ticks are converted to seconds. The tempo region index counts up as long as the current notes falls inside the region,
        /// and the note index counts as long as the current note falls within a tempo region.
        /// </summary>
        public static void RemapTicksToTimeB(MidiEvent[] midiEvents, TempoRegion[] tempoRegions, int division)
        {
            int eventIndex = 0;

            for (int regionIndex = 0; regionIndex < tempoRegions.Length;)
            {
                if (tempoRegions[regionIndex].Inside(midiEvents[eventIndex].Ticks))
                {
                    // remap ticks to seconds
                    midiEvents[eventIndex].Time = (float)tempoRegions[regionIndex].TicksToTime(midiEvents[eventIndex].Ticks, division);

                    eventIndex++;

                    if (eventIndex >= midiEvents.Length) break;
                }
                else
                {
                    regionIndex++;
                }
            }
        }

        private void Refresh(LinkedListNode<TempoRegion> node)
        {
            TempoRegion tempoRegion;

            TempoRegion prev = TempoRegion.Default;

            while (node != null)
            {
                if (node.Previous != null)
                {
                    prev = node.Previous.Value;
                }

                tempoRegion = node.Value;

                tempoRegion.DeltaTicks = node.Value.Ticks - prev.Ticks;
                tempoRegion.DurationTicks = (node.Next != null) ? node.Next.Value.Ticks - node.Value.Ticks : 0;
                tempoRegion.Time = prev.Time + tempoRegion.DeltaTicks / prev.Tempo / divisions;

                node.Value = tempoRegion;

                node = node.Next;
            }
        }
    }
}