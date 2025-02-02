// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

// The following copywrite notice is for the parts of this file that resemble the original work
// created by Keijiro in the project located here:
// https://github.com/keijiro/unity-audio-spectrum
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    // https://docs.unity3d.com/ScriptReference/Editor.html
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpectrumSource))]
    public class SpectrumSourceEditor : Editor
    {
        #region IntPopup Descriptions
        static int[] bandOptions =
        {
            (int)BandOption.FullRange,
            (int)BandOption.StandardRanges,
            (int)BandOption.OneOctave,
            (int)BandOption.OneHalfOctave,
            (int)BandOption.OneThirdOctave,
            (int)BandOption.OneSixthOctave,
            (int)BandOption.OneTwelvethOctave,
            (int)BandOption.A440,
        };

        static GUIContent[] bandOptionStrings = new GUIContent[]
        {
            new GUIContent("1 band (Full Range)"),
            new GUIContent("7 Bands (Standard Ranges)"),
            new GUIContent("10 Bands (One Octave)"),
            new GUIContent("20 Bands (One Half Octave)"),
            new GUIContent("30 Bands (One 3rd Octave)"),
            new GUIContent("60 Bands (One 6th Octave)"),
            new GUIContent("120 Bands (One 12th Octave)"),
            new GUIContent("120 Bands (A440)")
        };

        static int[] audioChannels =
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

        static GUIContent[] audioChannelStrings = new GUIContent[]
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
        #endregion

        #region Serialized Property
        SerializedProperty sourceModeProp;
		SerializedProperty globalFallbackProp;
        SerializedProperty midiSourceProp;
        SerializedProperty audioSourceProp;
        SerializedProperty peaksProfileProp;

        SerializedProperty channelProp;
        SerializedProperty bandOptionProperty;
        SerializedProperty normalizeProp;
        #endregion

        #region Editor Methods
        void OnEnable()
        {
            // Setup the SerializedProperties.
            sourceModeProp = serializedObject.FindProperty("sourceMode");
			globalFallbackProp = serializedObject.FindProperty("globalFallback");
            midiSourceProp = serializedObject.FindProperty("midiSource");
            audioSourceProp = serializedObject.FindProperty("audioSource");
            peaksProfileProp = serializedObject.FindProperty("peaksProfile");

            channelProp = serializedObject.FindProperty("audioChannel");
            bandOptionProperty = serializedObject.FindProperty("bandOption");
            normalizeProp = serializedObject.FindProperty("normalizeMode");
        }

        public override void OnInspectorGUI()
        {
            // update serialized object property values with their target value counterparts
            serializedObject.Update();

            SpectrumSource spectrumSource = (SpectrumSource)target;

            EditorGUILayout.Space();

			EditorGUILayout.LabelField("Spectrum", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            //EditorGUILayout.IntPopup(sourceModeProp, sourceModeStrings, sourceMode, new GUIContent("Mode", ""));
            EditorGUILayout.PropertyField(sourceModeProp, new GUIContent("Mode"));
            //EditorGUILayout.Space();

            if (spectrumSource.sourceMode == FrequencyBase.Audio)
            {
                EditorGUILayout.PropertyField(audioSourceProp, new GUIContent("Source"));
				EditorGUILayout.PropertyField(globalFallbackProp, new GUIContent("Global Fallback", "Generate spectrum from all sounds being played. Front Left (mono) only."));
				
                EditorGUILayout.IntPopup(channelProp, audioChannelStrings, audioChannels, new GUIContent("Channel", "The audio channel to pull spectrum data from. If the channel doesn't exist, then it'll fall back to the highest supported channel during play mode."));
            }
            else
            {
                EditorGUILayout.PropertyField(midiSourceProp, new GUIContent("Source"));
                //EditorGUILayout.IntPopup(channelProp, midiChannelStrings, midiChannels, new GUIContent("Channel", "The MIDI channel to pull spectrum data from."));
            }

            //EditorGUILayout.Space();

            if (spectrumSource.sourceMode == FrequencyBase.Audio)
            {
                EditorGUILayout.PropertyField(peaksProfileProp, new GUIContent("Peaks Profile", "Audio Spectrum Profile. Create one from the Assets menu."));

                if (!spectrumSource.peaksProfile)
                {
                    EditorGUILayout.HelpBox("A Peaks Profile must be attached for the spectrum to work.", MessageType.Warning);
                }

                // takes care of recording peaks
                if (Application.isPlaying && spectrumSource.peaksProfile != null && spectrumSource.peaksProfile.isDirty)
                {
                    if (spectrumSource.recordProfile)
                    {
                        if (GUILayout.Button("Stop Recording"))
                        {
                            spectrumSource.recordProfile = false;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Record Peaks"))
                        {
                            spectrumSource.RecordPeaks();
                        }
                    }
                }

                EditorGUI.indentLevel = 0;

                // disable this script if peaks are being recorded
                GUI.enabled = !spectrumSource.recordProfile;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Presentation", EditorStyles.boldLabel);
                EditorGUI.indentLevel = 1;


                EditorGUILayout.IntPopup(bandOptionProperty, bandOptionStrings, bandOptions, new GUIContent("Bands", "The number of times to divide the standard audio frequency."));
                EditorGUILayout.PropertyField(normalizeProp, new GUIContent("Normalize", "Scales level to be between 0 and 1 either by the highest peak value, or individually per level peak. Raw just passes the original spectrum data through."));
                
                if (spectrumSource.audioSource && spectrumSource.audioSource.clip)
                {
                    spectrumSource.audioChannel = Mathf.Clamp(spectrumSource.audioChannel, 0, (spectrumSource.audioSource.clip.channels - 1));
                }
            }

            EditorGUI.indentLevel = 0;

            // update target values with their serialized object property value counterparts
            serializedObject.ApplyModifiedProperties();

            // ensure all the selected spectrum sources are refreshed with the new property values
            if (GUI.changed)
            {
                foreach (SpectrumSource src in targets)
                {
                    src.Refresh();
                }
            }
        }
        #endregion
    }
}