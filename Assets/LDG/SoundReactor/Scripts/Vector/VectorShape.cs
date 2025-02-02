// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    /// <summary>
    /// A class containing vertex element data that defines a shape built by the SpectrumBuilder.
    /// </summary>
    public class VectorShape
    {
        /// <summary>
        /// The elements that make up the shape defined by SpectrumBuilder.
        /// </summary>
        public VertexElement[] vertexElements;

        // vertex buffer elements
        private Vector3[] vertices;
        private Vector3[] normals;
        private Color[] mainColors;
        private Color[] restingColors;

        public VectorShape(ColorDriver sharedDriver, Transform transform, bool isLine)
        {
            vertexElements = new VertexElement[transform.childCount];

            Level level;
            VertexElementColor vertexElementColor;
            ColorDriver colorDriver;

            int i = 0;

            foreach (Transform levelTransform in transform)
            {
                level = levelTransform.GetComponent<Level>();

                if ((vertexElementColor = levelTransform.GetComponent<VertexElementColor>()) == null)
                {
                    vertexElementColor = levelTransform.gameObject.AddComponent<VertexElementColor>();
                }

                if ((colorDriver = levelTransform.GetComponent<ColorDriver>()) == null)
                {
                    colorDriver = levelTransform.gameObject.AddComponent<ColorDriver>();
                }

                colorDriver.sharedDriver = sharedDriver;

                if (isLine)
                {
                    vertexElements[i] = new VertexElement(level, levelTransform.localPosition, Vector3.up, new Color(0.0f, 0.5f, 1.0f), Color.magenta);
                }
                else
                {
                    vertexElements[i] = new VertexElement(level, levelTransform.localPosition, Vector3.Normalize(levelTransform.localPosition), new Color(0.0f, 0.5f, 1.0f), Color.magenta);
                }

                vertexElementColor.index = i++;
                vertexElementColor.vectorShape = this;
            }
        }

        /// <summary>
        /// Updates the vertex buffers used by the Draw function in this class. It's used to allocate the buffers,
        /// and update them.
        /// </summary>
        private bool UpdateVertexBuffer(float travel)
        {
            if (vertexElements == null || vertexElements.Length == 0)
            {
                return false;
            }

            int numVertices = vertexElements.Length;

            if (vertices == null || vertices.Length != numVertices)
            {
                vertices = new Vector3[numVertices];
            }

            if (normals == null || normals.Length != numVertices)
            {
                normals = new Vector3[numVertices];
            }

            if (mainColors == null || mainColors.Length != numVertices)
            {
                mainColors = new Color[numVertices];
            }

            if (restingColors == null || restingColors.Length != numVertices)
            {
                restingColors = new Color[numVertices];
            }

            for (int i = 0; i < numVertices; i++)
            {
                if (vertexElements[i].level)
                {
                    vertices[i] = vertexElements[i].vertex + vertexElements[i].normal * vertexElements[i].level.fallingLevel * travel;
                    normals[i] = vertexElements[i].normal;
                    mainColors[i] = vertexElements[i].mainColor;
                    restingColors[i] = vertexElements[i].restingColor;
                }
            }

            return true;
        }

        /// <summary>
        /// Draws the vector shape.
        /// </summary>
        public void Draw(Transform transform, float layoutSize, float elementSize, float travel, bool isLine, bool anchored, float anchoredDiameter, Material material)
        {
            if (!UpdateVertexBuffer(travel)) return;

            if (material)
            {
                // Apply the line material
                material.SetPass(0);
            }

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            if (isLine)
            {
                GLU.CurveVector(vertices, mainColors, restingColors, elementSize, 10, false, anchored, InterpolationMode.Curve);
            }

            if (!isLine)
            {
                GLU.RingVector(vertices, mainColors, restingColors, layoutSize, elementSize, 10, anchored, anchoredDiameter, InterpolationMode.Curve);
            }

            GL.PopMatrix();
        }
    }
}