// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System.Collections;
using System.Collections.Generic;

namespace LDG.Core.Collections.Generic
{
    public class ObjectPool <T> : IEnumerable<T>
    {
        public T this[int i]
        {
            get { return objectPool[i]; }
            set { objectPool[i] = value; }
        }

        public int Index { get{ return CyclicIndex(); } }
        public int Length {  get{ return (objectPool != null) ? objectPool.Length : 0; } }

        private int size = 0;
        private T[] objectPool;
        private int index;

        /// <summary>
        /// Size of the pool. Size cannot be less than 1.
        /// </summary>
        public ObjectPool(int size)
        {
            if(size < 1)
            {
                throw new System.Exception("Pool size cannot be less than 1");
            }

            this.size = size;

            objectPool = new T[size];
            index = 0;
        }

        public ObjectPool(int cacheSize, T initialValue) : this(cacheSize)
        {
            for (int i = 0; i < cacheSize; i++)
            {
                objectPool[i] = initialValue;
            }
        }

        /// <summary>
        /// Automatically increments by 1 then sets the object.
        /// </summary>
        public void Add(T v)
        {
            index += 1;

            objectPool[CyclicIndex()] = v;
        }

        /// <summary>
        /// Increments internal index by increment then sets the object. Passing in 0 would be the same as Set(v) and passing in a 1 would be the same as Add(v).
        /// </summary>
        public void Add(T v, int increment)
        {
            index += increment;

            objectPool[CyclicIndex()] = v;
        }

        /// <summary>
        /// Sets the current object
        /// </summary>
        public void Set(T v)
        {
            objectPool[CyclicIndex()] = v;
        }

        /// <summary>
        /// Sets object at index.
        /// </summary>
        public void Set(T v, int index)
        {
            objectPool[index] = v;
        }

        /// <summary>
        /// Sets object relative to the current internal index. This does not change the internal index.
        /// </summary>
        public void SetRelative(T v, int index)
        {
            objectPool[RelativeToAbsoluteIndex(index)] = v;
        }

        /// <summary>
        /// Gets the current object.
        /// </summary>
        public T Get()
        {
            return objectPool[CyclicIndex()];
        }

        /// <summary>
        /// Gets object at index.
        /// </summary>
        public T Get(int index)
        {
            return objectPool[index];
        }

        /// <summary>
        /// Gets an object relative to the internal index. This does not change the internal index.
        /// </summary>
        public T GetRelative(int index)
        {
            return objectPool[RelativeToAbsoluteIndex(index)];
        }

        /// <summary>
        /// Offsets internal index. To increment or decrement the internal index by 1, use the ++/-- overloads on the class.
        /// </summary>
        public void OffsetIndex(int offset)
        {
            index += offset;
        }

        /// <summary>
        /// Returns an internal in bounds index relative to the input parameter.
        /// </summary>
        public int RelativeToAbsoluteIndex(int index)
        {
            int mod = (this.index + index) % size;
            
            return (mod < 0) ? size + mod : mod;
        }

        public static ObjectPool<T> operator ++(ObjectPool<T> v)
        {
            v.index++;

            return v;
        }

        public static ObjectPool<T> operator --(ObjectPool<T> v)
        {
            v.index--;

            return v;
        }

        /// <summary>
        /// Last in first out
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < size; ++i)
                yield return objectPool[RelativeToAbsoluteIndex(-i)];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int CyclicIndex()
        {
            int mod = index % size;

            return (mod < 0) ? size + mod : mod;
        }
    }
}