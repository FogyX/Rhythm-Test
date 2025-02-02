// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    /// <summary>
    /// This class is used to set the color of a vertex stored in a vector shape. It's attached to
    /// a Level, and the color is updated by a ColorDriver.
    /// </summary>
    public class VertexElementColor : MonoBehaviour
    {
        /// <summary>
        /// Index of the vertex to modify colors of.
        /// </summary>
        public int index;

        /// <summary>
        /// Main color which is applied to vectors in different ways depending on shape and builder settings.
        /// </summary>
        public Color mainColor = new Color(0.0f, 0.5f, 1.0f);

        /// <summary>
        /// Resting color which is applied to vectors in different ways depending on shape and builder settings.
        /// </summary>
        public Color restingColor = Color.magenta;

        /// <summary>
        /// The vector shape this class points to.
        /// </summary>
        public VectorShape vectorShape;

        private void Update()
        {
            if (vectorShape != null)
            {
                vectorShape.vertexElements[index].mainColor = mainColor;
                vectorShape.vertexElements[index].restingColor = restingColor;
            }
        }
    }
}
