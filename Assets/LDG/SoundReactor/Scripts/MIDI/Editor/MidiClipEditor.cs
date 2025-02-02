// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MidiClip))]
    public class MidiClipEditor : Editor
    {
        #region Fields
        static Material mat;

        SerializedProperty midiFileProp;
        SerializedProperty trackPreviewIndexProp;
        SerializedProperty defaultTempoProp;

        static int[] trackIndices;
        static GUIContent[] trackContents;
        #endregion
        
        #region Private Methods
        private void InitProperties()
        {
            // Setup the SerializedProperties.
            midiFileProp = serializedObject.FindProperty("midiFile");
            trackPreviewIndexProp = serializedObject.FindProperty("trackPreviewIndex");
            defaultTempoProp = serializedObject.FindProperty("defaultTempo");
        }

        private void PopulateTrackInt()
        {
            MidiClip midiClip = (MidiClip)target;

            string[] trackNames = midiClip.GetTrackNames();

            trackContents = new GUIContent[midiClip.numTracks];
            trackIndices = new int[midiClip.numTracks];

            for(int i = 0; i < midiClip.numTracks; i++)
            {
                trackContents[i] = new GUIContent(trackNames[i]);
                trackIndices[i] = i;
            }
        }
        #endregion

        #region Editor Methods
        void OnEnable()
        {
            InitProperties();
            PopulateTrackInt();
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                DrawNotes(r);
            }
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            MidiClip midiClip = (MidiClip)target;

            EditorGUILayout.PropertyField(defaultTempoProp, new GUIContent("Default Tempo", "Keep value at 120 unless the MIDI file doesn't contain a tempo."));
            EditorGUILayout.IntPopup(trackPreviewIndexProp, trackContents, trackIndices, new GUIContent("Preview Track", ""));

            if (GUILayout.Button("Reload"))
            {
                if (midiFileProp != null)
                {
                    foreach (MidiClip clip in targets)
                    {
                        string path = AssetDatabase.GetAssetPath(clip.midiFile);

                        if (MidiClip.Validate(path))
                        {
                            clip.Read(path);
                        }
                        else
                        {
                            Debug.LogError("Only format 0 and 1 are supported");
                        }
                    }

                    PopulateTrackInt();

                    EditorUtility.SetDirty(midiClip);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Public Methods
        private void DrawNotes(Rect r)
        {
            MidiClip midiClip = (MidiClip)target;

            GUI.BeginClip(r);
            GL.PushMatrix();

            if (mat == null)
            {
                var shader = Shader.Find("Hidden/LDG/UI");
                mat = new Material(shader);
            }

            mat.SetPass(0);

            GLU.Begin(GL.LINES);
            midiClip.Draw(r.width, r.height);
            GL.End();

            //GUI.EndGroup();
            GL.PopMatrix();
            GUI.EndClip();
        }
        #endregion
    }
}
