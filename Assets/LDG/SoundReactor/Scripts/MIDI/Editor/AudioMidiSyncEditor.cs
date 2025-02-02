// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(AudioMidiSync))]
    public class AudioMidiSyncEditor : Editor
    {
        #region SerializedProperty
        SerializedProperty onPropertyChangedProp;
        SerializedProperty audioSourceProp;
        SerializedProperty midiSourceProp;
        SerializedProperty midiSourceDelayedProp;
        SerializedProperty playOnAwakeProp;
        SerializedProperty loopProp;
        SerializedProperty playbackSpeedProp;
        SerializedProperty audioDelayProp;
        SerializedProperty audioStartDelayProp;
        SerializedProperty startTimeProp;
        SerializedProperty syncThresholdProp;
        SerializedProperty logToConsoleProp;
        #endregion

        public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void OnEnable()
        {
            onPropertyChangedProp = serializedObject.FindProperty("onPropertyChanged");
            audioSourceProp = serializedObject.FindProperty("audioSource");
            midiSourceProp = serializedObject.FindProperty("midiSource");
            midiSourceDelayedProp = serializedObject.FindProperty("midiSourceDelayed");

            playOnAwakeProp = serializedObject.FindProperty("playOnAwake");
            loopProp = serializedObject.FindProperty("loop");
            playbackSpeedProp = serializedObject.FindProperty("playbackSpeed");
            audioDelayProp = serializedObject.FindProperty("audioDelay");
            audioStartDelayProp = serializedObject.FindProperty("audioStartDelay");
            startTimeProp = serializedObject.FindProperty("startTime");
            syncThresholdProp = serializedObject.FindProperty("syncThreshold");
            logToConsoleProp = serializedObject.FindProperty("logToConsole");
        }

        public override void OnInspectorGUI()
        {
            AudioMidiSync audioMidiSync = (AudioMidiSync)target;

            //let's do this//this.DrawDefaultInspector();
            serializedObject.Update();

            EditorGUILayout.Separator();

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("Master", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(midiSourceProp, new GUIContent("MIDI Source", "Triggered to play immediately when Play is called"));

            EditorGUILayout.Separator();

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("Delayed", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(audioStartDelayProp, new GUIContent("Start Delay", "Delay in seconds before playing"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(midiSourceDelayedProp, new GUIContent("MIDI Source", "Triggered to play after delay reached"));
            EditorGUILayout.PropertyField(audioSourceProp, new GUIContent("Audio Source", "Triggered to play after delay reached"));
            EditorGUI.indentLevel = 0;


            EditorGUILayout.Separator();

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("Playback", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(playOnAwakeProp, new GUIContent("Play On Awake"));
            EditorGUILayout.PropertyField(loopProp, new GUIContent("Loop"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(playbackSpeedProp, new GUIContent("Speed", "Speed Multiplier"));
            EditorGUILayout.PropertyField(startTimeProp, new GUIContent("Start Time"));

            EditorGUILayout.Separator();

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("Sync", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(syncThresholdProp, new GUIContent("Threshold (+/-)", "The amount of time in seconds (+/-) MIDI can be out of sync with audio before it's forced back in sync."));
            EditorGUILayout.PropertyField(audioDelayProp, new GUIContent("Audio Latency", "This is the time in seconds for the audio to output to the speakers then reach your ears. 0.2 seconds is the default."));

            DrawUILine(Color.grey);




            EditorGUI.indentLevel = 0;

            GUI.enabled = Application.isPlaying;
            if (audioMidiSync.AudioSource && audioMidiSync.AudioSource.clip)
            {
                float time;

                float audioTime = audioMidiSync.AudioSource.Time();

                time = EditorGUILayout.Slider(audioTime, 0.0f, audioMidiSync.AudioSource.clip.length - 0.1f);

                if (Mathf.Abs(time - audioTime) > 0.1f)
                {
                    audioMidiSync.AudioSource.time = time;

                    if (audioMidiSync.MidiSourceDelayed)
                    {
                        audioMidiSync.MidiSourceDelayed.Seek(time - audioMidiSync.AudioDelay);
                    }

                    if (audioMidiSync.MidiSource)
                    {
                        audioMidiSync.MidiSource.Seek(time + audioMidiSync.AudioStartDelay - audioMidiSync.AudioDelay);
                    }
                }

                Repaint();
            }
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))
            {
                audioMidiSync.Play();
                //Debug.Log("Play");
            }

            if (GUILayout.Button("Pause"))
            {
                audioMidiSync.Pause();
                //Debug.Log("Pause");
            }

            if (GUILayout.Button("Stop"))
            {
                audioMidiSync.Stop();
                //Debug.Log("Stop");
            }

            EditorGUILayout.EndHorizontal();

            DrawUILine(Color.grey);

            EditorGUILayout.Separator();

            audioMidiSync.debug = EditorGUILayout.Foldout(audioMidiSync.debug, new GUIContent("Debug"));

            if (audioMidiSync.debug)
            {
                EditorGUI.indentLevel = 2;
                //EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(logToConsoleProp, new GUIContent("Log to console"));

                EditorGUI.indentLevel = 0;
                EditorGUILayout.HelpBox("These events only get called when playing from the editor", MessageType.Info);
                EditorGUILayout.PropertyField(onPropertyChangedProp, new GUIContent("OnPropertyChanged"));
            }

            serializedObject.ApplyModifiedProperties();

#if UNITY_EDITOR
            if (GUI.changed)
            {
                audioMidiSync.onPropertyChanged.Invoke();
            }
#endif
        }
    }
}