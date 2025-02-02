// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    // https://docs.unity3d.com/ScriptReference/Editor.html
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PeaksProfile))]
    public class PeaksProfileEditor : Editor
    {
        static Material mat;

        static int[] sampleOptions =
        {
            64, 128, 256, 512, 1024, 2048, 4096, 8192
        };

        static GUIContent[] sampleOptionStrings = new GUIContent[]
        {
            new GUIContent("64"),
            new GUIContent("128"),
            new GUIContent("256"),
            new GUIContent("512"),
            new GUIContent("1024"),
            new GUIContent("2048"),
            new GUIContent("4096"),
            new GUIContent("8192")
        };

        static public readonly int[] audioChannels =
        {
            (int)AudioChannel.FrontLeft,
            (int)AudioChannel.FrontRight,
            (int)AudioChannel.FrontCenter,
            (int)AudioChannel.Subwoofer,
            (int)AudioChannel.RearLeft,
            (int)AudioChannel.RearRight,
            (int)AudioChannel.AlternativeRearLeft,
            (int)AudioChannel.AlternativeRearRight
        };

        static public readonly GUIContent[] audioChannelStrings = new GUIContent[]
        {
            new GUIContent("Front Left (mono)"),
            new GUIContent("Front Right"),
            new GUIContent("Center"),
            new GUIContent("Subwoofer"),
            new GUIContent("Rear Left"),
            new GUIContent("Rear Right"),
            new GUIContent("Alternative Rear Left"),
            new GUIContent("Alternative Rear Right")
        };

        SerializedProperty windowProp;
        SerializedProperty samplesProp;
        SerializedProperty channelProp;
        SerializedProperty amplitudeProp;
        
        void OnEnable()
        {
            // Setup the SerializedProperties.
            windowProp = serializedObject.FindProperty("fftWindow");
            samplesProp = serializedObject.FindProperty("fftSamples");

            amplitudeProp = serializedObject.FindProperty("amplitudeMode");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            PeaksProfile source = (PeaksProfile)target;

            EditorGUILayout.Foldout(true, "FFT", GlobalStyles.heading);

            EditorGUILayout.PropertyField(windowProp, new GUIContent("Window", "The quality of the spectrum data. List is in desceneding order, lowest to highest."));
            EditorGUILayout.IntPopup(samplesProp, sampleOptionStrings, sampleOptions, new GUIContent("Samples", "The number of samples. Low quality results in better performance, and high quality result in lower performance."));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(amplitudeProp, new GUIContent("Amplitude", "Scales a level's magnitude. Linear is popular for processing, i.e. handling triggers, and Decible is good for visualizing."));

            EditorGUILayout.Space();

            if(GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("Warning", "This will clear saved peaks and mark the asset as dirty. This operation cannot be undone. Continue?", "Yes", "No"))
                {
                    source.ResetPeaks();
                    EditorUtility.SetDirty(source);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();

            if (source.isDirty)
            {
                EditorGUILayout.HelpBox("Peak data is dirty. Attach this asset to a SpectrumSource and hit Record Peaks in play mode.", MessageType.Warning);
            }

        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                DrawPeaks(r);
            }
        }

        private void DrawPeaks(Rect r)
        {
            PeaksProfile peaksProfile = (PeaksProfile)target;

            if (peaksProfile.peaks == null || peaksProfile.peaks.Length < 2) return;

            GUI.BeginClip(r);
            GL.PushMatrix();

            if (mat == null)
            {
                var shader = Shader.Find("Hidden/LDG/UI");
                mat = new Material(shader);
            }

            mat.SetPass(0);

            float hScale = r.width / (float)peaksProfile.peaks.Length;
            float vScale = r.height / 1.0f;

            Vector2 prev = new Vector2(0.0f, r.height - (peaksProfile.peaks[0] * vScale));
            Vector2 curr = Vector2.zero;

            GLU.Begin(GL.LINES);

            for (int i = 1; i < peaksProfile.peaks.Length; i++)
            {
                curr.x = i * hScale;
                curr.y = r.height - (peaksProfile.peaks[i] * vScale);

                if (i % 2 == 1)
                {
                    GLU.Line(prev, curr, Color.green);
                }
                else
                {
                    GLU.Line(prev, curr, Color.green * Color.gray);
                }

                prev = curr;
            }
            
            GL.End();
            

            //GUI.EndGroup();
            GL.PopMatrix();
            GUI.EndClip();
        }
    }
}
