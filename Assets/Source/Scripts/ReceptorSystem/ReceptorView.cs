using System;
using UnityEditor;
using UnityEngine;

namespace Source.Scripts
{
    public class ReceptorView : MonoBehaviour
    {
        public event Action<GameNote> NoteEntered;
        public event Action NoteExited;


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GameConfig config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/Source/GameConfig.asset");
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, new Vector3(config.GoodGradeDistance, 0));
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, new Vector3(config.GreatGradeDistance, 0));
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position, new Vector3(config.PerfectGradeDistance, 0));
        }
#endif
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out GameNote gameNote))
            {
                NoteEntered?.Invoke(gameNote);
            }
        }

        private void OnTriggerExit(Collider other)
        {
           if (other.gameObject.TryGetComponent(out GameNote gameNote))
           {
               if (gameNote.gameObject.activeSelf)
               {
                   NoteExited?.Invoke();
               }
           }
        }
    }
}