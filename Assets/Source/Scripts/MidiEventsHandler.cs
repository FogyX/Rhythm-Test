using LDG.SoundReactor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Scripts
{
    public class MidiEventsHandler : MonoBehaviour
    {
        [SerializeField] private AudioMidiSync _audioMidiSync;
        [SerializeField] private int _targetScene;
        
        private void Awake()
        {
            _audioMidiSync.MidiSource.onMidiEvent.AddListener(OnMidiEvent);
        }

        private void OnMidiEvent(Sequencer sequencer, MidiEvent e)
        {
            if (!e.IsMetaMessage) return;
            if (string.IsNullOrEmpty(e.MetaMessage.Marker)) return;

            if (e.MetaMessage.Marker == "GG")
            {
                SceneManager.LoadScene(_targetScene);
            }
        }
    }
}
