// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    /// <summary>
    /// Class that stores vertex information like: Vertex, Normal, Main Color, Resting Color,
    /// and a reference to a Level.
    /// </summary>
    public class VertexElement
    {
        public Vector3 vertex;
        public Vector3 normal;
        public Color mainColor;
        public Color restingColor;
        public Level level;

        public VertexElement(Level level)
        {
            vertex = Vector3.zero;
            normal = Vector3.up;
            mainColor = Color.white;
            restingColor = Color.black;
            this.level = level;
        }

        public VertexElement(Level level, Vector3 vertex, Vector3 normal, Color mainColor, Color restingColor)
        {
            this.vertex = vertex;
            this.normal = normal;
            this.mainColor = mainColor;
            this.restingColor = restingColor;
            this.level = level;
        }
    }
}