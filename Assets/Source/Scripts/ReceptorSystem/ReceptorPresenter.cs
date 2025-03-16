using System;
using VContainer;

namespace Source.Scripts
{
    public class ReceptorPresenter : IDisposable
    {
        private readonly Receptor _receptor;
        private readonly ReceptorView _receptorView;

        [Inject]
        public ReceptorPresenter(Receptor receptor, ReceptorView receptorView)
        {
            _receptor = receptor;
            _receptorView = receptorView;

            _receptorView.NoteEntered += OnNoteEntered; 
            _receptorView.NoteExited += OnNoteExited;
        }

        private void OnNoteExited()
        {
            _receptor.RemoveFirstNote();
        }

        private void OnNoteEntered(GameNote gameNote)
        {
            _receptor.AddNote(gameNote);
        }

        public void Dispose()
        {
            _receptorView.NoteEntered -= OnNoteEntered;
            _receptorView.NoteExited -= OnNoteExited;
        }
    }
}