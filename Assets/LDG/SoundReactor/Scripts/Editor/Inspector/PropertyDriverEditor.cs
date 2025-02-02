// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PropertyDriver))]
    public class PropertyDriverEditor : Editor
    {
        protected SerializedProperty travelProp;

        SerializedProperty levelProp;
        SerializedProperty sharedMotionProp;
        SerializedProperty maxLevelProp;
        SerializedProperty strengthProp;
        SerializedProperty onBeatProp;

        private void OnEnable()
        {
            InitProperties();
        }

        public virtual void InitProperties()
        {
            levelProp = serializedObject.FindProperty("level");
            sharedMotionProp = serializedObject.FindProperty("sharedDriver");
            travelProp = serializedObject.FindProperty("travel");
            maxLevelProp = serializedObject.FindProperty("clipping");
            strengthProp = serializedObject.FindProperty("strength");
            onBeatProp = serializedObject.FindProperty("onBeat");
        }

        public virtual void TravelGUI()
        {
            EditorGUILayout.PropertyField(travelProp, new GUIContent("Travel", "Defines the travel distance. For example, setting this to a value of 10 for a scale driver would cause the object to scale up by 10."));
        }

        public virtual bool ErrorGUI()
        {
            return true;
        }

        public virtual void ExtendedGUI()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PropertyDriver driver = (PropertyDriver)target;

            EditorGUILayout.PropertyField(levelProp, new GUIContent("Level", "The Level this driver uses to calculate the travel distance. If this is set to None, then it'll try to find a Level at runtime by looking up through the hierarchy."));
            EditorGUILayout.PropertyField(sharedMotionProp, new GUIContent("Shared Driver", "Travel, Max Level, Strength, and On Beat are all overidden by the ones in this shared driver."));

            GUI.enabled = (driver.sharedDriver == null);

            TravelGUI();

            EditorGUILayout.PropertyField(strengthProp, new GUIContent("Strength", "Scales the level's magnitude. This is useful to get values to reach their max travel distance sooner."));
            EditorGUILayout.PropertyField(maxLevelProp, new GUIContent("Clipping", "Clips the level's magnitude. A value of 1 will allow the level to reach 100% of its magnitude."));
            EditorGUILayout.PropertyField(onBeatProp, new GUIContent("On Beat", "Only compute a travel distance when a beat is detected, otherwise the travel is set to 0. Beats occur for one frame only."));

            ExtendedGUI();

            ErrorGUI();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(PositionDriver))]
    public class PositionDriverEditor : PropertyDriverEditor
    {
        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RotateDriver))]
    public class RotateDriverEditor : PropertyDriverEditor
    {
        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScaleDriver))]
    public class ScaleDriverEditor : PropertyDriverEditor
    {
        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(EventDriver))]
    public class EventDriverEditor : PropertyDriverEditor
    {
        SerializedProperty onLevelUpdate = null;

        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            onLevelUpdate = serializedObject.FindProperty("onLevel");

            base.InitProperties();
        }

        public override void ExtendedGUI()
        {
            EditorGUILayout.PropertyField(onLevelUpdate, true, null);
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ForceDriver))]
    public class ForceDriverEditor : PropertyDriverEditor
    {
        SerializedProperty forceModeProp;
        SerializedProperty forceTypeProp;
        SerializedProperty maxAngularVelocityProp;

        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();

            forceModeProp = serializedObject.FindProperty("forceMode");
            forceTypeProp = serializedObject.FindProperty("forceType");
            maxAngularVelocityProp = serializedObject.FindProperty("maxAngularVelocity");
        }

        public override bool ErrorGUI()
        {
            if (((ForceDriver)target).componentMissing)
            {
                EditorGUILayout.HelpBox("No RigidBody to drive", MessageType.Warning);
                //EditorGUILayout.LabelField("No RigidBody to drive", GlobalStyles.missingComponent);
                return false;
            }

            return true;
        }

        public override void ExtendedGUI()
        {
            EditorGUILayout.PropertyField(forceModeProp, new GUIContent("Force Mode"));
            EditorGUILayout.PropertyField(forceTypeProp, new GUIContent("Force Type"));
            EditorGUILayout.PropertyField(maxAngularVelocityProp, new GUIContent("Max Angular Velocity"));
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ParticleEmitterDriver))]
    public class ParticleEmitterDriverEditor : PropertyDriverEditor
    {
        SerializedProperty travelYProp;

        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();

            travelYProp = travelProp.FindPropertyRelative("y");
        }

        public override void TravelGUI()
        {
            EditorGUILayout.PropertyField(travelYProp, new GUIContent("Travel"));
        }

        public override bool ErrorGUI()
        {
            if (((ParticleEmitterDriver)target).componentMissing)
            {
                EditorGUILayout.HelpBox("No ParticleSystem to drive", MessageType.Warning);
                //EditorGUILayout.LabelField("No ParticleSystem to drive", GlobalStyles.missingComponent);
                return false;
            }

            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorDriver))]
    public class ColorDriverEditor : PropertyDriverEditor
    {
        SerializedProperty travelYProp;

        SerializedProperty colorModeProp;
        SerializedProperty stationaryToggleProp;
        SerializedProperty mainColorProp;
        SerializedProperty restingColorProp;
        SerializedProperty indexProp;
        
        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();

            travelYProp = travelProp.FindPropertyRelative("y");

            colorModeProp = serializedObject.FindProperty("colorMode");
            stationaryToggleProp = serializedObject.FindProperty("stationaryToggle");
            mainColorProp = serializedObject.FindProperty("mainColor");
            restingColorProp = serializedObject.FindProperty("restingColor");
            indexProp = serializedObject.FindProperty("materialIndex");
        }

        public override void TravelGUI()
        {
            EditorGUILayout.PropertyField(travelYProp, new GUIContent("Travel"));
        }

        public override bool ErrorGUI()
        {
            if (((ColorDriver)target).componentMissing)
            {
                EditorGUILayout.HelpBox("There are no color properties to drive", MessageType.Warning);
                return false;
            }

            return true;
        }

        public override void ExtendedGUI()
        {
            ColorDriver colorDriver = (ColorDriver)target;

            EditorGUILayout.PropertyField(colorModeProp, new GUIContent("Color Mode", "Allows you to choose how the Gradient color is applied."));

            if (colorDriver.colorMode != ColorMode.Frequency)
            {
                EditorGUILayout.PropertyField(stationaryToggleProp, new GUIContent("Stationary", "Check this if a Segmented Levels shape was built with the SpectrumBuilder."));
            }

            EditorGUILayout.PropertyField(indexProp, new GUIContent("Material Index", "The index to a material for the attached mesh."));
            EditorGUILayout.PropertyField(mainColorProp, new GUIContent("Main Color", "Used to color object by magnitude or frequency."));
            EditorGUILayout.PropertyField(restingColorProp, new GUIContent("Resting Color", "Used by segmented levels built with the SpectrumBuilder."));
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MidiTrackColorDriver))]
    public class MidiTrackColorDriverEditor : PropertyDriverEditor
    {
        SerializedProperty travelYProp;
        
        SerializedProperty trackColorsProp;
        SerializedProperty restingColorProp;
        SerializedProperty indexProp;

        private void OnEnable()
        {
            InitProperties();
        }

        public override void InitProperties()
        {
            base.InitProperties();

            travelYProp = travelProp.FindPropertyRelative("y");
            
            trackColorsProp = serializedObject.FindProperty("trackColors");
            restingColorProp = serializedObject.FindProperty("restingColor");
            indexProp = serializedObject.FindProperty("materialIndex");
        }

        public override void TravelGUI()
        {
            EditorGUILayout.PropertyField(travelYProp, new GUIContent("Travel"));
        }

        public override bool ErrorGUI()
        {
            if (((MidiTrackColorDriver)target).componentMissing)
            {
                EditorGUILayout.HelpBox("There are no color properties to drive", MessageType.Warning);
                return false;
            }

            return true;
        }

        public override void ExtendedGUI()
        {
            MidiTrackColorDriver colorDriver = (MidiTrackColorDriver)target;

            EditorGUILayout.PropertyField(indexProp, new GUIContent("Material Index", "The index to a material for the attached mesh."));

            EditorGUILayout.PropertyField(trackColorsProp, new GUIContent("Track Color", "Used by segmented levels built with the SpectrumBuilder."), true);
            EditorGUILayout.PropertyField(restingColorProp, new GUIContent("Resting Color", "Used by segmented levels built with the SpectrumBuilder."));
        }
    }
}