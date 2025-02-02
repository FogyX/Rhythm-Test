// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public sealed class GLU
    {
        private static Vector3 vertexCache;
        private static Color colorCache;
        private static bool firstVertex = true;

        public static void Rect(Rect rect, bool fill, Color color)
        {
            int mode;

            if (fill)
            {
                mode = GL.QUADS;

                GL.Begin(mode);
                GL.Color(color);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(rect.width, 0, 0);
                GL.Vertex3(rect.width, rect.height, 0);
                GL.Vertex3(0, rect.height, 0);
                GL.Vertex3(0, 0, 0);
                GL.End();
            }
            else
            {
				mode = GL.LINES;

                GL.Begin(mode);
                GL.Color(color);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(rect.width, 0, 0);

                GL.Vertex3(rect.width, 0, 0);
                GL.Vertex3(rect.width, rect.height, 0);

                GL.Vertex3(rect.width, rect.height, 0);
                GL.Vertex3(0, rect.height, 0);

                GL.Vertex3(0, rect.height, 0);
                GL.Vertex3(0, 0, 0);
                GL.End();
            }
        }

        public static void Line(Vector2 p1, Vector2 p2, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(p1.x, p1.y, 0.0f);
            GL.Vertex3(p2.x, p2.y, 0.0f);
            GL.End();
        }

        public static void Begin(int mode)
        {
            GL.Begin(mode);

            firstVertex = true;
        }

        public static void Vertex(Vector3 vertex, Color color)
        {
            if (firstVertex)
            {
                vertexCache = vertex;
                colorCache = color;

                firstVertex = false;
            }
            else
            {
                GL.Color(colorCache);
                GL.Vertex(vertexCache);

                GL.Color(color);
                GL.Vertex(vertex);

                vertexCache = vertex;
                colorCache = color;
            }
        }

        public static void Curve(Vector3[] vertices, Color[] colors, int segments, bool closed, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;

#if UNITY_5_6_OR_NEWER
            GL.Begin(GL.LINE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                GL.Vertex(Spline.Tween(normalizedIndex, vertices, closed, mode));
                GL.Color(Spline.Tween(normalizedIndex, colors, closed, mode));
            }
#else
            Begin(GL.LINES);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                Vertex(Spline.Tween(normalizedIndex, vertices, closed, mode), Spline.Tween(normalizedIndex, colors, closed, mode));
            }
#endif
            GL.End();
        }

        public static void CurveVector(Vector3[] vertices, Color[] primaryColors, Color[] secondaryColors, float width, int segments, bool closed, bool anchored, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;
            Vector3 vertexCache = Vector3.zero;
            Vector3 vertex;// = Vector3.zero;
            Vector3 normal;
            Vector3 lineDir;
            float slice = 1.0f / (float)(segmentCount);
            float halfWidth = width * 0.5f;

            GL.Begin(GL.TRIANGLE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                vertex = Spline.Tween(normalizedIndex, vertices, closed, mode);

                // calculate line direction
                if (i == 0)
                {
                    // derive direction by extrapolating next vertex position (this only happens on the first pass)
                    vertexCache = Spline.Tween(normalizedIndex + slice, vertices, closed, mode);
                    lineDir = (vertexCache - vertex).normalized;
                }
                else
                {
                    // derive direction by subtracting previous vertex position
                    lineDir = (vertex - vertexCache).normalized;
                }

                if (closed && (i == 0 || i == segmentCount) || anchored)
                {
                    normal = Vector3.up;
                }
                else
                {
                    normal = Vector3.Cross(Vector3.forward, lineDir);
                }

                if(anchored)
                {
                    // bottom (anchored)
                    GL.Color(Spline.Tween(normalizedIndex, secondaryColors, closed, mode));
                    GL.Vertex(new Vector3(vertex[0], -halfWidth));

                    // top (the moving part)
                    GL.Color(Spline.Tween(normalizedIndex, primaryColors, closed, mode));
                    GL.Vertex(vertex + normal * halfWidth);
                }
                else
                {
                    GL.Color(Spline.Tween(normalizedIndex, primaryColors, closed, mode));

                    // bottom
                    GL.Vertex(vertex + normal * -halfWidth);

                    // top
                    GL.Vertex(vertex + normal * halfWidth);
                }

                vertexCache = vertex;
            }

            GL.End();
        }

        public static void RingVector(Vector3[] vertices, Color[] primaryColors, Color[] secondaryColors, float radius, float width, int segments, bool anchored, float anchoredDiameter, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;
            float halfWidth = width * 0.5f;
            float slice = 1.0f / (float)(segmentCount);
            float anchoredRadius = anchoredDiameter * 0.5f;

            Vector3 vertex;
            Vector3 normal;
            Vector3 lineDir;

            GL.Begin(GL.TRIANGLE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                vertex = Spline.Tween(normalizedIndex, vertices, true, mode);

                // calculate line direction
                if (i == 0)
                {
                    // derive direction by extrapolating next vertex position (this only happens on the first pass)
                    vertexCache = Spline.Tween(normalizedIndex + slice, vertices, true, mode);
                    lineDir = (vertexCache - vertex).normalized;
                }
                else
                {
                    // derive direction by subtracting previous vertex position
                    lineDir = (vertex - vertexCache).normalized;
                }

                // calculate normal perpendicular to the direction the line is being drawn
                if (i == 0 || i == segmentCount)
                {
                    // circles begin and end at the top, so "up" works fine here
                    normal = Vector3.up;
                }
                else
                {
                    // calculate normal on the xy plane.
                    normal = Vector3.Cross(lineDir, Vector3.forward);
                }

                // ring outside
                GL.Color(Spline.Tween(normalizedIndex, primaryColors, true, mode));
                GL.Vertex(vertex + normal * halfWidth);

                // ring inside
                GL.Color(Spline.Tween(normalizedIndex, secondaryColors, true, mode));

                // the inside ring can be anchored
                if (anchored)
                {
                    GL.Vertex(vertex.normalized * anchoredRadius);
                }
                else
                {
                    GL.Vertex(vertex - normal * halfWidth);
                }

                // remember the previous vertex so it can be used to calculate the lineDir
                vertexCache = vertex;
            }

            GL.End();
        }

        public static void Graph(float[] values, float range, float width, float height, Color color, InterpolationMode mode)
        {
            // draw spectrum
#if UNITY_5_6_OR_NEWER
            GL.Begin(GL.LINE_STRIP);

            for (int i = 0; i < width; i++)
            {
                float iLevel = Mathf.Clamp01(1.0f - Spline.Tween((float)i / (width - 1), values, false, mode) / range) * height;

                GL.Color(color);
                GL.Vertex3(i, iLevel, 0);
            }
#else
            GL.Begin(GL.LINES);

            Vector2 xy = Vector2.zero;
            Vector2 xyPrev = xy;

            for (int i = 0; i < width; i++)
            {
                xy.Set(i, Mathf.Clamp01(1.0f - Spline.Tween((float)i / (width - 1), values, false, mode) / range) * height);

                if (i > 0)
                {
                    GL.Color(color);
                    GL.Vertex3(xyPrev.x, xyPrev.y, 0);
                    GL.Vertex3(xy.x, xy.y, 0);
                }

                xyPrev = xy;
            }
#endif

            GL.End();
        }
    }
}