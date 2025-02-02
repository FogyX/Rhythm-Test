// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using System.IO;

namespace LDG.SoundReactor
{
    public class MidiReader : BinaryReader
    {
        #region Fields
        // default to Intel endian, which is little endian.
        private bool isLittleEndian = true;
        #endregion

        #region Constructors
        public MidiReader(Stream input) : base(input)
        {
        }

        public MidiReader(Stream input, bool isLittleEndian = true) : base(input)
        {
            this.isLittleEndian = isLittleEndian;
        }
        #endregion

        #region Overrides
        public byte[] ReadBytes(int count, bool resolveEndian)
        {
            byte[] bytes = base.ReadBytes(count);

            if (isLittleEndian != BitConverter.IsLittleEndian && resolveEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        public override float ReadSingle()
        {
            byte[] bytes = BitConverter.GetBytes(base.ReadSingle());

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToSingle(bytes, 0);
        }

        public override ushort ReadUInt16()
        {
            uint v = base.ReadUInt16();

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                v = (v << 8) | (v >> 8);
            }

            return (ushort)v;
        }

        public override short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            uint v = base.ReadUInt32();

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                v = ((v << 8) & 0xFF00FF00) | ((v >> 8) & 0xFF00FF);
                v = (v << 16) | (v >> 16);
            }

            return v;
        }

        public override int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        /// <summary>
        /// Reads variable length quantity
        /// 
        /// Reference:
        /// https://en.wikipedia.org/wiki/Variable-length_quantity
        /// </summary>
        /// <returns>
        /// The decoded integer
        /// </returns>
        public uint ReadVLQ()
        {
            uint v = 0;
            byte b;

            do
            {
                b = ReadByte();

                // 0x7F = 0111 1111
                v = (uint)((v << 7) + (b & 0x7F));
            }
            while ((b & 0x80) == 0x80);

            return v;
        }

        public int ReadVInt32()
        {
            return (int)ReadVLQ();
        }
        #endregion
    }
}