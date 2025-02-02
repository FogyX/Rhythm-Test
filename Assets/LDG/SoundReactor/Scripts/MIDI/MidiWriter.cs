// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LDG.SoundReactor
{
    public class MidiWriter : BinaryWriter
    {
        #region Fields
        // default to Intel endian, which is little endian.
        private bool isLittleEndian = true;

        private const uint MAX_VLQ = uint.MaxValue >> 4;

        private Queue chunkPosQueue = new Queue();
        #endregion

        #region Constructors
        /// <summary>
        /// Writes file in native indian order. If indian order is not specified, then little endian is the default.
        /// </summary>
        /// <param name="input">Input stream.</param>
        public MidiWriter(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Writes file in native indian order. If indian order is not specified, then little endian is used by default.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <param name="isLittleEndian">Specifiy endianness.</param>
        public MidiWriter(Stream input, bool isLittleEndian = true) : base(input)
        {
            this.isLittleEndian = isLittleEndian;
        }

        public MidiWriter()
        {
            chunkPosQueue = new Queue();
        }
        #endregion

        public void ResetChunk()
        {
            chunkPosQueue.Clear();
        }

        /// <summary>
        /// Starts a chunk and writes chunk info either inside or outside the chunk.
        /// </summary>
        /// <param name="writeChunk">User defined handler that takes care of writing the chunk.</param>
        /// <param name="id">4 char chunk ID.</param>
        /// <param name="includeChunkDefinition">True will include the chunk info inside the chunk.</param>
        public void PushChunk(Action<MidiWriter, string> writeChunk, string id, bool includeChunkDefinition)
        {
            if (!includeChunkDefinition)
            {
                writeChunk(this, id);
                //Debug.Log($"Pushed: Chunk Id: {id}, Chunk Data Begin: {base.BaseStream.Position}, Include Id: {includeIdInChunk.ToString()}");
            }

            chunkPosQueue.Enqueue(base.BaseStream.Position);

            if (includeChunkDefinition)
            {
                //Debug.Log($"Pushed: Chunk Id: {id}, Chunk Data Begin: {base.BaseStream.Position}, Include Id: {includeIdInChunk.ToString()}");
                writeChunk(this, id);
            }
        }

        /// <summary>
        /// Ends a chunk and writes the size of the chunk into the chunk info.
        /// </summary>
        /// <param name="writeChunkSize">User defined handler that takes care of writing the size of the chunk into the chunk info.</param>
        public void PopChunk(Action<MidiWriter, long> writeChunkSize)
        {
            // get position of the start of the chunk (the position can either include, or not include the chunk id and size)
            long position = (long)chunkPosQueue.Dequeue();

            // calculate the size of the chunk
            long size = base.BaseStream.Position - position;

            // set stream position to the start of the chunk
            base.BaseStream.Position = position;

            // write the size of the chunk
            writeChunkSize(this, size);

            // restore stream position
            base.BaseStream.Position = position + size;

            //Debug.Log($"Popped: Chunk Size: {size}, Chunk Data End: {base.BaseStream.Position}");
        }

        public void WriteBytes(byte[] bytes, bool resolveEndian)
        {
            // if the system is Big Endian, then reverse the bytes
            if (isLittleEndian != BitConverter.IsLittleEndian && resolveEndian)
            {
                Array.Reverse(bytes);
            }

            base.Write(bytes);

            // if the array was reversed, restore the original order so that it's correct after writing
            if (isLittleEndian != BitConverter.IsLittleEndian && resolveEndian)
            {
                Array.Reverse(bytes);
            }
        }

        public void WriteBytes(byte[] bytes, int count, bool resolveEndian)
        {
            Array.Resize(ref bytes, count);

            if (isLittleEndian != BitConverter.IsLittleEndian && resolveEndian)
            {
                Array.Reverse(bytes);
            }

            base.Write(bytes);

            if (isLittleEndian != BitConverter.IsLittleEndian && resolveEndian)
            {
                Array.Reverse(bytes);
            }
        }

        /// <summary>
        /// Writes variable length quanity
        /// 
        /// Reference:
        /// https://en.wikipedia.org/wiki/Variable-length_quantity
        /// </summary>
        public void WriteVLQ(uint v)
        {
            if (v > MAX_VLQ)
            {
                Debug.Log(string.Format("WriteVLQ Error: {0} cannot be greater than {1}.", v, MAX_VLQ));
                throw new Exception(string.Format("WriteVLQ Error: {0} cannot be greater than {1}.", v, MAX_VLQ));
            }

            uint buffer;
            buffer = v & 0x7F;

            // build variable length byte buffer.
            while ((v >>= 7) > 0)
            {
                buffer <<= 8;

                // 0x7F = 0111 1111
                // 0x80 = 1000 0000
                buffer |= ((v & 0x7F) | 0x80);
            }

            // now write the buffer
            while(true)
            {
                // write the least significant byte
                base.Write((byte)buffer);

                // 0x80 = 1000 0000
                if ((buffer & 0x80) == 0x80)
                    buffer >>= 8;
                else
                    break;
            }
        }

        public void WriteVLQ(int v)
        {
            WriteVLQ((uint)v);
        }

        #region Overrides
        public override void Write(ushort value)
        {
            int v = value;

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                v = (v << 8) | ((v >> 8) & 0xFF);
            }

            base.Write((ushort)v);
        }

        public override void Write(short value)
        {
            Write((ushort)value);
        }

        public override void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            base.Write(BitConverter.ToSingle(bytes, 0));
        }

        public override void Write(uint value)
        {
            uint v = value;

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                v = ((v << 8) & 0xFF00FF00) | ((v >> 8) & 0xFF00FF);
                v = (v << 16) | ((v >> 16) & 0xFFFF);
            }

            base.Write(v);
        }

        public override void Write(int v)
        {
            Write((uint)v);
        }
        #endregion
    }
}