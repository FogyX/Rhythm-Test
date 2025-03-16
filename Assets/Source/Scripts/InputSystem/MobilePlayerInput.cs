using System;
using System.Collections.Generic;

namespace Assets.Source.Scripts.InputSystem
{
    public class MobilePlayerInput : IPlayerInput
    {
        public event Action<int> NotePressed;

        private readonly List<NoteButton> _noteButtons = new();

        public void AddNoteButton(NoteButton button)
        {
            _noteButtons.Add(button);
            button.Pressed += OnButtonPressed;
        }

        private void OnButtonPressed(int noteId)
        {
            NotePressed?.Invoke(noteId);
        }

        public void Dispose()
        {
            foreach (NoteButton noteButton in _noteButtons)
                noteButton.Pressed -= OnButtonPressed;
        }
    }
}
