using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(PianoInfo))]
    public class PianoInfoEditor : Editor
    {
        PianoInfo pianoInfo;

        private void OnEnable()
        {
            pianoInfo = (PianoInfo)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            if(GUILayout.Button(new GUIContent("Set Default", "Sets default values for the built in Piano Keys")))
            {
                if(EditorUtility.DisplayDialog("Resetting to default", "Operation cannot be undone. Are you sure you would like to proceed?", "Yes", "Cancel"))
                {
                    pianoInfo.SetDefault();
                }
            }
        }
    }
}