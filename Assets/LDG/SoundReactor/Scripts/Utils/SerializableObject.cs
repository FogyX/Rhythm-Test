// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;

namespace LDG.SoundReactor
{
    /// <summary>
    /// Used by MonoBehaviours whose values need to be remembered after leaving play mode
    /// </summary>
    public class SerializeableObject : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private bool _isPrefab = false;

        [HideInInspector]
        public bool isPrefab { get{ return _isPrefab; } }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
#if UNITY_2018_2_OR_NEWER
                _isPrefab = (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab);
#else
                _isPrefab = (PrefabUtility.GetPrefabType(gameObject) != PrefabType.None);
#endif // UNITY_2018_2_OR_NEWER
            }
        }
#endif // UNITY_EDITOR
    }
}
