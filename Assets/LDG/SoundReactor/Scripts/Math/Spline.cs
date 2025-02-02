// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public enum InterpolationMode
    {
        Linear,
        Curve
    }

    /// <summary>
    /// Catmull-rom spline
    /// </summary>
    static public class Spline
    {
        static public float Tween(float normalizedIndex, float[] values, bool closed, InterpolationMode interpolationMode)
        {
            if (values == null || values.Length == 0) return 0.0f;

            if(values.Length == 1)
            {
                return values[0];
            }

            int nextIndex;
            int index;
            int nextNextIndex;
            int prevIndex;

            int maxArrayLength = Mathf.Max(0, values.Length - 1);

            // reconstruct index as a floating point number
            float floatIndex = normalizedIndex * (float)(maxArrayLength);

            // capture fractional part of the index to use as the interpolation scalar
            float t = floatIndex - (int)floatIndex;
            //float t = (floatIndex % 1);

            // get the integral part of the float to use as the index
            index = Mathf.Max(0, (int)floatIndex);

            // get the next index by adding 1, but don't let it excede one less than the array length
            nextIndex = (closed) ? (int)Mathf.Repeat(index + 1, maxArrayLength) : Mathf.Min(index + 1, maxArrayLength);

            float value = 0.0f;

            switch (interpolationMode)
            {
                case InterpolationMode.Linear:

                    value = Mathf.Lerp(values[index], values[nextIndex], t);

                    break;

                case InterpolationMode.Curve:

                    if (closed)
                    {
                        nextNextIndex = (int)Mathf.Repeat(index + 2, maxArrayLength);
                        prevIndex = (int)Mathf.Repeat(index - 1, maxArrayLength);
                    }
                    else
                    {
                        nextNextIndex = Mathf.Min(index + 2, maxArrayLength);
                        prevIndex = Mathf.Max(index - 1, 0);
                    }

                    value = Cubic(values[prevIndex], values[index], values[nextIndex], values[nextNextIndex], t);

                    break;
            }

            return value;
        }

        /// <summary>
        /// Interpolate between values in the values array.
        /// </summary>
        /// <param name="normalizedIndex">Pass in a value between 0 and 1.</param>
        /// <param name="values">Array of values to interpolate between.</param>
        /// <param name="closed">Use the first value as the very last value in the array.</param>
        /// <param name="interpolationMode">Can interplocate as linear or curved.</param>
        /// <returns>The new interpolated value.</returns>
        static public Vector3 Tween(float normalizedIndex, Vector3[] values, bool closed, InterpolationMode interpolationMode)
        {
            if (values == null || values.Length == 0) return Vector3.zero;

            if (values.Length == 1)
            {
                return values[0];
            }

            int nextIndex;
            int index;
            int nextNextIndex;
            int prevIndex;

            int maxArrayLength = Mathf.Max(0, values.Length - 1);

            // reconstruct index as a floating point number
            float floatIndex = normalizedIndex * (float)(maxArrayLength);

            // capture fractional part of the index to use as the interpolation scalar
            float t = floatIndex - (int)floatIndex;
            //float t = (floatIndex % 1);

            // get the integral part of the float to use as the index
            index = Mathf.Max(0, (int)floatIndex);

            // get the next index by adding 1, but don't let it excede one less than the array length
            nextIndex = (closed) ? (int)Mathf.Repeat(index + 1, maxArrayLength) : Mathf.Min(index + 1, maxArrayLength);

            Vector3 value = Vector3.zero;

            switch (interpolationMode)
            {
                case InterpolationMode.Linear:

                    value.x = Mathf.Lerp(values[index].x, values[nextIndex].x, t);
                    value.y = Mathf.Lerp(values[index].y, values[nextIndex].y, t);
                    value.z = Mathf.Lerp(values[index].z, values[nextIndex].z, t);

                    break;

                case InterpolationMode.Curve:

                    if(closed)
                    {
                        nextNextIndex = (int)Mathf.Repeat(index + 2, maxArrayLength);
                        prevIndex = (int)Mathf.Repeat(index - 1, maxArrayLength);
                    }
                    else
                    {
                        nextNextIndex = Mathf.Min(index + 2, maxArrayLength);
                        prevIndex = Mathf.Max(index - 1, 0);
                    }

                    value.x = Cubic(values[prevIndex].x, values[index].x, values[nextIndex].x, values[nextNextIndex].x, t);
                    value.y = Cubic(values[prevIndex].y, values[index].y, values[nextIndex].y, values[nextNextIndex].y, t);
                    value.z = Cubic(values[prevIndex].z, values[index].z, values[nextIndex].z, values[nextNextIndex].z, t);

                    break;
            }

            return value;
        }

        /// <summary>
        /// Interpolates between from and to and follows the path specified by tangent in and tangent out.
        /// </summary>
        static public Vector3 Tween(Vector3 tangentIn, Vector3 from, Vector3 to, Vector3 tangentOut, float t, InterpolationMode interpolationMode)
        {
            Vector3 value = Vector3.zero;

            if(t == 0.0f) return from;
            if (t == 1.0f) return to;

            switch (interpolationMode)
            {
                case InterpolationMode.Linear:

                    value.x = Mathf.Lerp(from.x, to.x, t);
                    value.y = Mathf.Lerp(from.y, to.y, t);
                    value.z = Mathf.Lerp(from.z, to.z, t);

                    break;

                case InterpolationMode.Curve:
                    value.x = CubicTangent(tangentIn.x, from.x, to.x, tangentOut.x, t);
                    value.y = CubicTangent(tangentIn.y, from.y, to.y, tangentOut.y, t);
                    value.z = CubicTangent(tangentIn.z, from.z, to.z, tangentOut.z, t);

                    break;
            }

            return value;
        }

        static public Vector2 Tween(float normalizedIndex, Vector2[] values, bool closed, InterpolationMode interpolationMode)
        {
            if (values == null || values.Length == 0) return Vector3.zero;

            if (values.Length == 1)
            {
                return values[0];
            }

            int nextIndex;
            int index;
            int nextNextIndex;
            int prevIndex;

            int maxArrayLength = Mathf.Max(0, values.Length - 1);

            // reconstruct index as a floating point number
            float floatIndex = normalizedIndex * (float)(maxArrayLength);

            // capture fractional part of the index to use as the interpolation scalar
            float t = floatIndex - (int)floatIndex;
            //float t = (floatIndex % 1);

            // get the integral part of the float to use as the index
            index = Mathf.Max(0, (int)floatIndex);

            // get the next index by adding 1, but don't let it excede one less than the array length
            nextIndex = (closed) ? (int)Mathf.Repeat(index + 1, maxArrayLength) : Mathf.Min(index + 1, maxArrayLength);

            Vector2 value = Vector2.zero;

            switch (interpolationMode)
            {
                case InterpolationMode.Linear:

                    value.x = Mathf.Lerp(values[index].x, values[nextIndex].x, t);
                    value.y = Mathf.Lerp(values[index].y, values[nextIndex].y, t);

                    break;

                case InterpolationMode.Curve:

                    if (closed)
                    {
                        nextNextIndex = (int)Mathf.Repeat(index + 2, maxArrayLength);
                        prevIndex = (int)Mathf.Repeat(index - 1, maxArrayLength);
                    }
                    else
                    {
                        nextNextIndex = Mathf.Min(index + 2, maxArrayLength);
                        prevIndex = Mathf.Max(index - 1, 0);
                    }

                    value.x = Cubic(values[prevIndex].x, values[index].x, values[nextIndex].x, values[nextNextIndex].x, t);
                    value.y = Cubic(values[prevIndex].y, values[index].y, values[nextIndex].y, values[nextNextIndex].y, t);

                    break;
            }

            return value;
        }

        static public Color Tween(float normalizedIndex, Color[] values, bool closed, InterpolationMode interpolationType)
        {
            int nextIndex;
            int index;
            int nextNextIndex;
            int prevIndex;

            int maxArrayLength = Mathf.Max(0, values.Length - 1);

            // reconstruct index as a floating point number
            float floatIndex = normalizedIndex * (float)(maxArrayLength);

            // capture fractional part of the index to use as the interpolation scalar
            //float t = floatIndex - (int)floatIndex;
            float t = (floatIndex % 1);

            // get the integral part of the float to use as the index
            index = Mathf.Max(0, (int)floatIndex);

            // get the next index by adding 1, but don't let it excede one less than the array length
            nextIndex = (closed) ? (int)Mathf.Repeat(index + 1, maxArrayLength) : Mathf.Min(index + 1, maxArrayLength);

            Color value = Color.black;

            switch (interpolationType)
            {
                case InterpolationMode.Linear:

                    value = Color.Lerp(values[index], values[nextIndex], t);
                    break;

                case InterpolationMode.Curve:

                    if (closed)
                    {
                        nextNextIndex = (int)Mathf.Repeat(index + 2, maxArrayLength);
                        prevIndex = (int)Mathf.Repeat(index - 1, maxArrayLength);
                    }
                    else
                    {
                        nextNextIndex = Mathf.Min(index + 2, maxArrayLength);
                        prevIndex = Mathf.Max(index - 1, 0);
                    }

                    value.r = Cubic(values[prevIndex].r, values[index].r, values[nextIndex].r, values[nextNextIndex].r, t);
                    value.g = Cubic(values[prevIndex].g, values[index].g, values[nextIndex].g, values[nextNextIndex].g, t);
                    value.b = Cubic(values[prevIndex].b, values[index].b, values[nextIndex].b, values[nextNextIndex].b, t);

                    break;
            }

            return value;
        }
        
        static private float Cubic(float before, float from, float to, float after, float t)
        {
            float r;                // result
            float ts = t * t;       // t squared
            float tc = t * t * t;   // t cubed

            // our tangents
            float t1 = 0.5f * (to - before);
            float t2 = 0.5f * (after - from);
            
            // calculate new interpolated value
            r = from * (2.0f * tc - 3.0f * ts + 1.0f);
            r += to * (-2.0f * tc + 3.0f * ts);
            r += t1 * (tc - 2.0f * ts + t);
            r += t2 * (tc - ts);

            // 14 multiplies
            // 5 subtractions
            // 6 additions

            return r;
        }

        static private float CubicTangent(float tangentIn, float from, float to, float tangentOut, float t)
        {
            float r;                // result
            float ts = t * t;       // t squared
            float tc = t * t * t;   // t cubed

            // calculate new interpolated value
            r = from * (2.0f * tc - 3.0f * ts + 1.0f);
            r += to * (-2.0f * tc + 3.0f * ts);
            r += tangentIn * (tc - 2.0f * ts + t);
            r += tangentOut * (tc - ts);

            // 14 multiplies
            // 5 subtractions
            // 6 additions

            return r;
        }
    }
}