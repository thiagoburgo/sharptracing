using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Util.Conversion;

namespace DrawEngine.Renderer.Util.IO
{
    public class EndianessBinaryWriter : BinaryWriter
    {
        protected EndianessBinaryWriter()
        { }
        public EndianessBinaryWriter(Stream output, ByteOrder byteOrder) : this(output, new UTF8Encoding(false, true), byteOrder) { }
        public EndianessBinaryWriter(Stream output, Encoding encoding, ByteOrder byteOrder)
            : base(output, encoding)
        {
            this.ByteOrder = byteOrder;
        }

        public ByteOrder ByteOrder { get; set; }
        public BitConverterEx CurrentBitConverter
        {
            get
            {
                BitConverterEx bitConverter = BitConverterEx.SystemEndian;
                switch (this.ByteOrder)
                {  
                    case ByteOrder.LittleEndian:
                        bitConverter = BitConverterEx.LittleEndian;
                        break;
                    case ByteOrder.BigEndian:
                        bitConverter = BitConverterEx.BigEndian;
                        break;
                }
                return bitConverter;
            }
        }
       
        public override void Write(int value)
        {
            base.Write(this.CurrentBitConverter.GetBytes(value));
        }
       
    }
}
