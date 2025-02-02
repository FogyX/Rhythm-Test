using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(MidiSource))]
    public class MidiSourceEditor : Editor
    {
        private SerializedProperty clipProp;
        private SerializedProperty noteOffsetProp;
        private SerializedProperty muteProp;
        private SerializedProperty speedProp;
        private SerializedProperty playOnAwakeProp;
        private SerializedProperty loopProp;
        private SerializedProperty onMidiEventProp;

        public void OnEnable()
        {
            clipProp = serializedObject.FindProperty("_clip");
            noteOffsetProp = serializedObject.FindProperty("_noteOffset");
            muteProp = serializedObject.FindProperty("mute");
            speedProp = serializedObject.FindProperty("speed");
            playOnAwakeProp = serializedObject.FindProperty("playOnAwake");
            loopProp = serializedObject.FindProperty("loop");
            onMidiEventProp = serializedObject.FindProperty("onMidiEvent");
        }

        public override void OnInspectorGUI()
        {
            MidiSource midiSource = target as MidiSource;

            serializedObject.Update();

            if(midiSource.usingExternalNotes)
            {
                EditorGUILayout.HelpBox("All notes are currently being handled by a script.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.PropertyField(clipProp);
                EditorGUILayout.PropertyField(noteOffsetProp);
                EditorGUILayout.PropertyField(muteProp);
                EditorGUILayout.PropertyField(speedProp);
                EditorGUILayout.PropertyField(playOnAwakeProp);
                EditorGUILayout.PropertyField(loopProp);
                EditorGUILayout.PropertyField(onMidiEventProp);
                
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Play"))
                {
                    midiSource.Play();
                    //Debug.Log("Play");
                }

                if (GUILayout.Button("Pause"))
                {
                    midiSource.Pause();
                    //Debug.Log("Pause");
                }

                if (GUILayout.Button("Stop"))
                {
                    midiSource.Stop();
                    //Debug.Log("Stop");
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}