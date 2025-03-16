using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.InputSystem;
using Assets.Source.Scripts.LevelStructure;
using LDG.SoundReactor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Source.Scripts
{
    public class LevelGenerator : IInitializable
    {
        private readonly AudioMidiSync _audioMidiSync;
        private readonly GameNote _notePrefab;
        private readonly TracksContainer _tracksContainer;
        private readonly NotesContainer _notesContainerPrefab;
        private readonly NoteButton _noteButtonPrefab;
        private readonly float _syncOffset;
        private readonly int _channelToGenerate;
        private readonly float _noteMovementSpeed;
        private readonly MobilePlayerInput _mobilePlayerInput;

        [Inject]
        public LevelGenerator(AudioMidiSync audioMidiSync,
            TracksContainer tracksContainer,
            GameConfig gameConfig,
            MobilePlayerInput mobilePlayerInput = null)
        {
            _audioMidiSync = audioMidiSync;
            _notePrefab = gameConfig.GameNotePrefab;
            _tracksContainer = tracksContainer;
            _notesContainerPrefab = gameConfig.NotesContainerPrefab;
            _noteButtonPrefab = gameConfig.NoteButtonPrefab;
            _syncOffset = gameConfig.SyncOffset;
            _channelToGenerate = gameConfig.ChannelToGenerate;
            _noteMovementSpeed = gameConfig.NoteMovementSpeed;
            _mobilePlayerInput = mobilePlayerInput;
        }

        public void Initialize()
        {
            MidiClip clip = _audioMidiSync.MidiSource.clip;

            List<NoteData> notesData = clip.EnumerateChannelVoiceMessages(_channelToGenerate)
                .Where(msg => msg.ChannelVoiceMessage == ChannelVoiceMessage.NoteOn)
                .Select(msg => new NoteData(msg.Time, msg.Note))
                .ToList();

            int minTrackId = notesData.Min(note => note.TrackId);
            int maxTrackId = notesData.Max(note => note.TrackId);
            int numberOfTracks = maxTrackId - minTrackId + 1;

            foreach (NoteData noteData in notesData)
            {
                noteData.TrackId -= minTrackId;
            }
            
            float normalizedNoteMovementSpeed = _noteMovementSpeed * (1 / _audioMidiSync.PlaybackSpeed);
            float midiSourceDelay = _audioMidiSync.AudioStartDelay;
            float noteOffset = normalizedNoteMovementSpeed * (midiSourceDelay + _syncOffset);
            
            for (int i = 0; i < numberOfTracks; i++)
            {
                NotesContainer notesContainer = Object.Instantiate(_notesContainerPrefab);
                NoteButton noteButton = Object.Instantiate(_noteButtonPrefab, notesContainer.transform);
                noteButton.Construct(i);
                noteButton.transform.position += Vector3.up * 0.2f;
                if (_mobilePlayerInput != null)
                    _mobilePlayerInput.AddNoteButton(noteButton);
                _tracksContainer.AddNotesContainer(notesContainer);
            }

            foreach (NoteData noteData in notesData)
            {
                GameNote newNote = Object.Instantiate(_notePrefab, _tracksContainer.NotesContainers[noteData.TrackId].transform);
                newNote.Construct(noteData.TrackId, _noteMovementSpeed);
                float notePositionX = noteOffset + noteData.Timing * normalizedNoteMovementSpeed;
                newNote.transform.localPosition = new Vector3(notePositionX, 0, 0);
            }
        }

        private class NoteData
        {
            public float Timing { get; set; }
            public int TrackId { get; set; }

            public NoteData(float timing, int trackId)
            {
                Timing = timing;
                TrackId = trackId;
            }
        }
    }
}