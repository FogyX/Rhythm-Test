using UnityEditor;
using UnityEngine;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(PianoRoll))]
    public class PianoRollEditor : Editor
    {
        private PianoRoll pianoRoll;

        private SerializedProperty audioMidiSyncProp;
        private SerializedProperty spectrumBuilderProp;
        private SerializedProperty pianoInfoProp;
        private SerializedProperty notePoolProp;
        private SerializedProperty viewProp;
        private SerializedProperty inheritStartDelayProp;
        private SerializedProperty timeSpanProp;
        private SerializedProperty viewLengthProp;
        private SerializedProperty viewAnchorProp;

        private void OnEnable()
        {
            pianoRoll = (PianoRoll)target;

            audioMidiSyncProp = serializedObject.FindProperty("audioMidiSync");
            spectrumBuilderProp = serializedObject.FindProperty("spectrumBuilder");
            pianoInfoProp = serializedObject.FindProperty("pianoInfo");
            notePoolProp = serializedObject.FindProperty("notePool");
            viewProp = serializedObject.FindProperty("view");
            inheritStartDelayProp = serializedObject.FindProperty("inheritStartDelay");
            timeSpanProp = serializedObject.FindProperty("timeSpan");
            viewLengthProp = serializedObject.FindProperty("viewLength");
            viewAnchorProp = serializedObject.FindProperty("viewAnchor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Scene References", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(audioMidiSyncProp, new GUIContent("Audio Midi Sync", ""));
            EditorGUILayout.PropertyField(spectrumBuilderProp, new GUIContent("Spectrum Builder", ""));
            EditorGUILayout.PropertyField(notePoolProp, new GUIContent("Note Pool", ""));
            EditorGUILayout.PropertyField(viewProp, new GUIContent("View", ""));
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Data References", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(pianoInfoProp, new GUIContent("Piano Info", "Contains "));
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Time", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            if (pianoRoll.valid)
            {
                GUI.enabled = !pianoRoll.inheritStartDelay;

                if (pianoRoll.inheritStartDelay)
                {
                    timeSpanProp.floatValue = pianoRoll.audioMidiSync.AudioStartDelay;
                }

                if(pianoRoll.audioMidiSync.AudioStartDelay == 0.0f)
                {
                    inheritStartDelayProp.boolValue = false;
                }
            }

            timeSpanProp.floatValue = Mathf.Max(timeSpanProp.floatValue, 0.0001f);
            EditorGUILayout.PropertyField(timeSpanProp, new GUIContent("Span", "The total time in seconds that the front of the note will be in view."));
            GUI.enabled = true;

            GUI.enabled = pianoRoll.valid && pianoRoll.audioMidiSync.AudioStartDelay != 0.0f;
            
            EditorGUI.indentLevel = 2;
            EditorGUILayout.PropertyField(inheritStartDelayProp, new GUIContent("Inherit Start Delay", "Inherit Start Delay from AudioMidiSync. This will be disabled if the Start Delay in AudioMidiSync is 0."));

            GUI.enabled = true;
            EditorGUI.indentLevel = 0;


            EditorGUILayout.Space();

            EditorGUILayout.LabelField("View", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(viewLengthProp, new GUIContent("Length", ""));
            EditorGUILayout.PropertyField(viewAnchorProp, new GUIContent("Anchor", ""));

            serializedObject.ApplyModifiedProperties();
        }
    }
}