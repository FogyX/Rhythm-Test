// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(EQ))]
    public class EQEditor : Editor
    {
        static float max = 2.0f;
        static float min = 0.0f;

        public override void OnInspectorGUI()
        {
            EQ eq = (EQ)target;

            int iMin;
            int iMax;

            eq.bandFilterOption = (BandFilters)EditorGUILayout.EnumPopup("Filter Band", eq.bandFilterOption);

            EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            
            eq.slope = EditorGUILayout.Slider("Slope", eq.slope, min, max);
            eq.offset = EditorGUILayout.Slider("Offset", eq.offset, 0.0f, 2.0f);
            eq.master = EditorGUILayout.Slider("Master", eq.master, 0.0f, 20.0f);

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            
            eq.levelShape = (LevelMode)EditorGUILayout.EnumPopup("Mode", eq.levelShape);

            float upperPreset = 0;
            float lowerPreset = 0;

            Frequency.GetRangePreset(out lowerPreset, out upperPreset, (int)eq.bandFilterOption + 1, FrequencyBase.Audio);

            iMin = Frequency.FrequencyToArrayIndex(lowerPreset, eq.levels.Length);
            iMax = Frequency.FrequencyToArrayIndex(upperPreset, eq.levels.Length);

            for (int i = 0; i < eq.levels.Length; i++)
            {
                if (i >= iMin && i <= iMax)
                {
                    eq.bandFilters[i] = 1f;
                }
                else
                {
                    eq.bandFilters[i] = 0f;
                }
            }
            
            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < eq.levels.Length; i++)
            {
                float linearizedFrequency = 0.0f;
                int nLevels = eq.levels.Length;
                float level = 0.0f;

                if (eq.bandFilters[i] == 1.0f)
                {
                    switch (eq.levelShape)
                    {
                        case LevelMode.Custom:
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), eq.levels[i], min, max);
                            break;

                        case LevelMode.Flat:
                            level = Mathf.Clamp(eq.slope + eq.offset - 1.0f, 0, 2);
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), level, min, max);
                            break;

                        case LevelMode.Mute:
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), 0.0f, min, max);
                            break;

                        case LevelMode.Max:
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), max * eq.bandFilters[i], min, max);

                            break;

                        case LevelMode.LinearAscending:
                            level = Mathf.Clamp(((float)i / (nLevels - 1) * eq.slope + eq.offset), 0, 2);
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), level, min, max);
                            break;

                        case LevelMode.LinearDescending:
                            level = Mathf.Clamp(((1.0f - (float)i / (nLevels - 1)) * eq.slope + eq.offset), 0, 2);
                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), level, min, max);
                            break;

                        case LevelMode.SquareAscending:
                            linearizedFrequency = (float)i / (float)(eq.levels.Length - 1);
                            level = Mathf.Clamp((linearizedFrequency * linearizedFrequency * eq.slope + eq.offset), 0, 2);

                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), level, min, max);
                            break;

                        case LevelMode.SquareDescending:
                            linearizedFrequency = (float)i / (float)(eq.levels.Length - 1);
                            level = Mathf.Clamp(((1.0f - linearizedFrequency * linearizedFrequency) * eq.slope + eq.offset), 0, 2);

                            eq.levels[i] = EditorGUILayout.Slider((i + 1).ToString(), level, min, max);
                            break;
                    }
                }
            }
            EditorGUI.indentLevel = 0;
            
            if(EditorGUI.EndChangeCheck())
            {
                eq.levelShape = LevelMode.Custom;
            }

            Undo.RecordObject(eq, "EQ");
        }
    }
}