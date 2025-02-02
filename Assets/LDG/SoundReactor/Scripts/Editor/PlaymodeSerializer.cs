// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LDG.SoundReactor
{
#if UNITY_2017_2_OR_NEWER
#else
    public enum PlayModeStateChange
    {
        EnteredEditMode = 0,
        ExitingEditMode = 1,
        EnteredPlayMode = 2,
        ExitingPlayMode = 3
    }
#endif

    /// <summary>
    /// Enables the editor to remember changes made in play mode
    /// </summary>
    [InitializeOnLoad]
    public class PlaymodeSerializer
    {
        static Dictionary<int, SerializedObject> serializedObjectDict = new Dictionary<int, SerializedObject>();

        /// <summary>
        /// Constructor
        /// </summary>
        static PlaymodeSerializer()
        {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnPlaymodeChanged;
#endif
        }

#if UNITY_2017_2_OR_NEWER
#else
        static public void OnPlaymodeChanged()
        {
            if (EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnPlayModeStateChanged(PlayModeStateChange.EnteredPlayMode);
            }

            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnPlayModeStateChanged(PlayModeStateChange.ExitingPlayMode);
            }

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnPlayModeStateChanged(PlayModeStateChange.EnteredEditMode);
            }
        }
#endif

        /// <summary>
        /// Used to serialize playmode properties in order to restore properties AFTER coming back from play mode.
        /// </summary>
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    SerializeObjects();
                    //Debug.Log(serializedObjectDict.Count + " objects in dictionary during EnteredPlayMode event");
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    //Debug.Log(serializedObjectDict.Count + " objects in dictionary during ExitingPlayMode event");
                    SerializeProperties();

                    break;

                case PlayModeStateChange.EnteredEditMode:
                    //Debug.Log(serializedObjectDict.Count + " objects in dictionary during EnteredEditMode event");
                    ApplyModifiedProperties();

                    break;
            }
        }

        static void SerializeObjects()
        {
            SerializedObject serializedObject;

            SerializeableObject[] objects = null;
            objects = Resources.FindObjectsOfTypeAll<SerializeableObject>();

            serializedObjectDict.Clear();

            foreach (SerializeableObject obj in objects)
            {
                if (!obj.isPrefab)
                {
                    if (!serializedObjectDict.TryGetValue(obj.GetInstanceID(), out serializedObject))
                    {
                        serializedObject = new SerializedObject(obj);

                        serializedObjectDict.Add(obj.GetInstanceID(), serializedObject);
                    }

                    serializedObject.Update();
                }
            }
        }

        static void SerializeProperties()
        {
            SerializedObject serializedObject;
            SerializedProperty property;

            SerializeableObject[] objects = null;
            objects = Resources.FindObjectsOfTypeAll<SerializeableObject>();

            foreach (SerializeableObject obj in objects)
            {
                if (!obj.isPrefab)
                {
                    if (serializedObjectDict.TryGetValue(obj.GetInstanceID(), out serializedObject))
                    {
                        property = (new SerializedObject(obj)).GetIterator();

                        property.Next(true);

                        do
                        {
                            if (property.propertyType != SerializedPropertyType.ObjectReference)
                            {
                                serializedObject.CopyFromSerializedProperty(property);
                            }

                        } while (property.Next(false));
                    }
                }
            }
            
        }

        static void ApplyModifiedProperties()
        {
            foreach (KeyValuePair<int, SerializedObject> keyValue in serializedObjectDict)
            {
                if (keyValue.Value.targetObject != null)
                {
                    keyValue.Value.ApplyModifiedProperties();
                }
            }
        }
    }
}