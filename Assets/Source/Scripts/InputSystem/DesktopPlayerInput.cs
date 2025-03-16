using Source.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Source.Scripts.InputSystem
{
    class DesktopPlayerInput : IPlayerInput, ITickable
    {
        public event Action<int> NotePressed;

        private readonly Dictionary<KeyCode, int> _keyToNoteMap;

        [Inject]
        public DesktopPlayerInput(GameConfig gameConfig)
        {
            _keyToNoteMap = gameConfig.NoteInputPairs.ToDictionary(pair => pair.Key, pair => pair.NoteId);
        }

        public void Tick()
        {
            if (Input.anyKeyDown)
                foreach (var keyNotePair in _keyToNoteMap)
                     if (Input.GetKeyDown(keyNotePair.Key))
                         NotePressed?.Invoke(keyNotePair.Value);
        }
    }
}
