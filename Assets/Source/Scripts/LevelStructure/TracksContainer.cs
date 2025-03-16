using Source.Scripts;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Assets.Source.Scripts.LevelStructure
{
    public class TracksContainer : MonoBehaviour
    {
        private float _spacing;
        private readonly List<NotesContainer> _notesContainers = new();

        public IReadOnlyList<NotesContainer> NotesContainers => _notesContainers;

        [Inject]
        public void Construct(GameConfig config)
        {
            _spacing = config.NotesSpacing;
        }

        public void AddNotesContainer(NotesContainer notesContainer)
        {
            _notesContainers.Add(notesContainer);
            notesContainer.transform.parent = transform;
            UpdateTrackPositions();
        }

        private void UpdateTrackPositions()
        {
            if (_notesContainers.Count == 0)
                return;

            float totalWidth = (_notesContainers.Count - 1) * _spacing;
            float startZ = totalWidth / 2f;

            for (int i = 0; i < _notesContainers.Count; i++)
            {
                float zPos = startZ - i * _spacing;
                _notesContainers[i].transform.localPosition = new Vector3(0, 0, zPos);
            }
        }
    }
}
