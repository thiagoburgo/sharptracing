using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace DrawEngine.Renderer.Util.IO
{
    public enum Endianess
    {
        LittleEndian,
        BigEndian
    }
    public class EndianessBinaryReader : BinaryReader
    {
        public EndianessBinaryReader(Stream input)
            : this(input, Endianess.LittleEndian)
        {
        }
        public EndianessBinaryReader(Stream input, Endianess endianess) : this(input, new UTF8Encoding(false, true), endianess)
        {
        }
        public EndianessBinaryReader(Stream input, Encoding encoding, Endianess endianess) : base(input, encoding)
        {
            this.Endianess = endianess;
        }
        public Endianess Endianess { get; set; }
        
        
        public override short ReadInt16()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    short r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadInt16();
                }
            }
        }
        public override ushort ReadUInt16()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    ushort r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadUInt16();
                }
            }
        }
        public override int ReadInt32()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    int r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadInt32();
                }
            }
        }
        public override uint ReadUInt32()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    uint r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadUInt32();
                }
            }
        }
        public override double ReadDouble()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    double r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadDouble();
                }
            }
        }
        public override float ReadSingle()
        {
            unsafe
            {
                if (this.Endianess == Endianess.BigEndian)
                {
                    float r = 0;
                    byte* p = (byte*)&r;
                    p[3] = base.ReadByte();
                    p[2] = base.ReadByte();
                    p[1] = base.ReadByte();
                    p[0] = base.ReadByte();

                    return r;
                }
                else
                {
                    return base.ReadSingle();
                }
            }
        }
    }
}
