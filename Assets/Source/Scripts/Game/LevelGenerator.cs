using System;
using System.Collections.Generic;
using System.Linq;
using LDG.SoundReactor;
using UnityEngine;

namespace Source.Scripts.Game
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private AudioMidiSync _audioMidiSync;
        [SerializeField] private GameNote _notePrefab;
        [SerializeField] private Transform _notesContainer;
        [SerializeField] private float _syncOffset;
        private void Awake()
        {
            MidiClip clip = _audioMidiSync.MidiSource.clip;
            List<float> noteTimings = clip.EnumerateChannelVoiceMessages(1)
                .Where(msg => msg.ChannelVoiceMessage == ChannelVoiceMessage.NoteOn)
                .Select(msg => msg.Time)
                .ToList();
            
            

            float noteMovementSpeed = _notePrefab.MovementSpeed * (1 / _audioMidiSync.PlaybackSpeed);
            float midiSourceDelay = _audioMidiSync.AudioStartDelay; 
            float noteOffset = noteMovementSpeed * (midiSourceDelay + _syncOffset);
            foreach (float timing in noteTimings)
            {
                GameNote note = Instantiate(_notePrefab, _notesContainer);
                note.transform.position = new Vector3( noteOffset + timing * noteMovementSpeed, 0, 0);
            }
        }
    }
}
