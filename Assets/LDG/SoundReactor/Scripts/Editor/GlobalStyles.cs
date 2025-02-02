// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [InitializeOnLoad]
    public class GlobalStyles
    {
        static public GUIStyle heading = new GUIStyle();
        static public GUIStyle missingComponent = new GUIStyle();

        static GlobalStyles()
        {
            heading.fontStyle = FontStyle.Bold;
            
            missingComponent.fontStyle = FontStyle.Bold;
            missingComponent.normal.textColor = Color.red;
        }
    }
}