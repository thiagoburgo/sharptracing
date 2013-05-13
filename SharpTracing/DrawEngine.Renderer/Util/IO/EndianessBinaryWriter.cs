using System.IO;
using System.Text;
using Util.Conversion;

namespace DrawEngine.Renderer.Util.IO {
    public class EndianessBinaryWriter : BinaryWriter {
        protected EndianessBinaryWriter() : base() {}

        public EndianessBinaryWriter(Stream output, ByteOrder byteOrder)
            : this(output, new UTF8Encoding(false, true), byteOrder) {}

        public EndianessBinaryWriter(Stream output, Encoding encoding, ByteOrder byteOrder) : base(output, encoding) {
            this.ByteOrder = byteOrder;
        }

        public ByteOrder ByteOrder { get; set; }

        public BitConverterEx CurrentBitConverter {
            get {
                BitConverterEx bitConverter = BitConverterEx.SystemEndian;
                switch (this.ByteOrder) {
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

        public override void Write(bool value) {
            base.Write(value);
        }

        public override void Write(byte value) {
            base.Write(value);
        }

        public override void Write(byte[] buffer) {
            base.Write(buffer);
        }

        public override void Write(byte[] buffer, int index, int count) {
            base.Write(buffer, index, count);
        }

        public override void Write(char ch) {
            base.Write(ch);
        }

        public override void Write(char[] chars) {
            base.Write(chars);
        }

        public override void Write(char[] chars, int index, int count) {
            base.Write(chars, index, count);
        }

        public override void Write(decimal value) {
            base.Write(value);
        }

        public override void Write(double value) {
            base.Write(value);
        }

        public override void Write(float value) {
            base.Write(value);
        }

        public override void Write(int value) {
            base.Write(this.CurrentBitConverter.GetBytes(value));
        }

        public override void Write(long value) {
            base.Write(value);
        }

        public override void Write(sbyte value) {
            base.Write(value);
        }

        public override void Write(short value) {
            base.Write(value);
        }

        public override void Write(string value) {
            base.Write(value);
        }

        public override void Write(uint value) {
            base.Write(value);
        }

        public override void Write(ulong value) {
            base.Write(value);
        }

        public override void Write(ushort value) {
            base.Write(value);
        }
    }
}