using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.InputSystem;
using LDG.SoundReactor;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Source.Scripts
{
    public class Receptor
    {
        private readonly float _goodGradeDistance;
        private readonly float _greatGradeDistance;
        private readonly float _perfectGradeDistance;

        private readonly Transform _receptorViewTransform;
        private readonly List<GameNote> _notesInProcess = new();

        private IPlayerInput _playerInput;
        
        public event Action<NoteTouchGrade> NoteTouched;
        
        [Inject]
        public Receptor(GameConfig config, ReceptorView receptorView, IPlayerInput playerInput)
        {
            _goodGradeDistance = config.GoodGradeDistance;
            _greatGradeDistance = config.GreatGradeDistance;
            _perfectGradeDistance = config.PerfectGradeDistance;

            _receptorViewTransform = receptorView.transform;

            _playerInput = playerInput;
            _playerInput.NotePressed += OnNotePressed;
        }   

        private void OnNotePressed(int noteId)
        {
            NoteTouchGrade grade = NoteTouchGrade.Miss;

            if (_notesInProcess.Count > 0)
            {
                GameNote firstNote = _notesInProcess.FirstOrDefault(note => note.TrackId == noteId);

                if (firstNote != null)
                {
                    float distance = Mathf.Abs(_receptorViewTransform.position.x - firstNote.transform.position.x);

                    if (distance <= _perfectGradeDistance)
                        grade = NoteTouchGrade.Perfect;
                    else if (distance <= _greatGradeDistance)
                        grade = NoteTouchGrade.Great;
                    else if (distance <= _goodGradeDistance)
                        grade = NoteTouchGrade.Good;

                    _notesInProcess.Remove(firstNote);
                    firstNote.gameObject.SetActive(false);
                }
            }

            NoteTouched?.Invoke(grade);
        }

        public void AddNote(GameNote gameNote)
        {
            _notesInProcess.Add(gameNote);
        }

        public void RemoveFirstNote()
        {
            _notesInProcess.RemoveAt(0);
            NoteTouched?.Invoke(NoteTouchGrade.Miss);
        }
    }   
}