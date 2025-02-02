using System;
using System.Collections.Generic;
using System.Linq;
using LDG.SoundReactor;
using UnityEngine;

namespace Source.Scripts.Game
{
    public class Receptor : MonoBehaviour
    {
        [SerializeField] private AudioMidiSync _audioMidiSync;
        

        private readonly List<GameNote> _currentlyCollidingNotes = new(); 

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Clicked");
                if (_currentlyCollidingNotes.Count > 0)
                {
                    GameNote firstNote = _currentlyCollidingNotes[0];
                    _currentlyCollidingNotes.RemoveAt(0);
                    firstNote.gameObject.SetActive(false);
                    Debug.Log($"Removed note: {firstNote.gameObject.name}");
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out GameNote gameNote))
            {
                _currentlyCollidingNotes.Add(gameNote);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
           if (other.gameObject.TryGetComponent(out GameNote gameNote))
           {
               if (gameNote.gameObject.activeSelf)
               {
                   _currentlyCollidingNotes.RemoveAt(0);
                   _audioMidiSync.Stop();
                   Debug.Log("imagine there is failure system worked");
               }
           }
        }
    }   
}