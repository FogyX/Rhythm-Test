// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpectrumFilter))]
    public class SpectrumFilterEditor : Editor
    {
        SerializedProperty spectrumSourceProp;
        SerializedProperty eqProp;

        SerializedProperty interpolationTypeProp;
        SerializedProperty scaleProp;
        SerializedProperty fallSpeedProp;
        SerializedProperty beatTriggerProp;
        SerializedProperty beatSensitivityProp;

        private void OnEnable()
        {
            spectrumSourceProp = serializedObject.FindProperty("spectrumSource");
            eqProp = serializedObject.FindProperty("eq");

            interpolationTypeProp = serializedObject.FindProperty("interpolationMode");
            scaleProp = serializedObject.FindProperty("scale");
            fallSpeedProp = serializedObject.FindProperty("fallSpeed");
            beatTriggerProp = serializedObject.FindProperty("beatTrigger");
            beatSensitivityProp = serializedObject.FindProperty("beatSensitivity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpectrumFilter spectrumFilter = (SpectrumFilter)target;

            if (spectrumFilter.spectrumSource != null)
            {
                Frequency.SetBaseFrequency((FrequencyBase)spectrumFilter.spectrumSource.sourceMode);
            }

            SpectrumGUILayout.SpectrumField(spectrumFilter, spectrumFilter.fallingLevels, 2.0f, -1.0f);

            EditorGUILayout.PropertyField(spectrumSourceProp, new GUIContent("SpectrumSource", "The SpectrumSource to filter. If this is set to None, then it'll try to find a SpectrumSource at runtime by looking up through the hierarchy."));
            EditorGUILayout.PropertyField(eqProp, new GUIContent("EQ", "Allows the user to make finer adjustments to the spectrum."));
            EditorGUILayout.PropertyField(interpolationTypeProp, new GUIContent("Interpolation", "Sets whether the values in between levels ease in and out or are straight lines."));
            EditorGUILayout.PropertyField(scaleProp, new GUIContent("Scale", "Scales all magnitudes equally for the given SpectrumSource"));
            EditorGUILayout.PropertyField(fallSpeedProp, new GUIContent("Fall Speed", "The time in seconds a level at maximum height will reach its minimum height."));
            EditorGUILayout.PropertyField(beatTriggerProp, new GUIContent("Beat Trigger", "Sets what kind of spectrum motion can trigger a beat."));
            EditorGUILayout.PropertyField(beatSensitivityProp, new GUIContent("Beat Sensitivity", "The percentage threshold that a level has to move to trigger a beat. 1 equals 100%"));

            // Update frequently while it's playing.
            if (EditorApplication.isPlaying || GUI.changed)
            {
                Repaint();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}