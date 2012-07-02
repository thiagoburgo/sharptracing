// Axiom3DSLoader - Converts a 3DStudio file to an XML format that can be
// recognized by the RealmForge GDK. This software is a part of the
// RealmForge GDK project, produced by XeonxStudios. For more information
// visit http://www.xeonxstudios.com/
// Copyright (C) 2004 Xeonx Studios
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System.Diagnostics;
using System.IO;

namespace DrawEngine.Renderer.Importers
{
    /// <summary>
    /// Reads data from a sequence of 3DS data segments (aka 3DS data "chunks").
    /// </summary>
    public class DataReader3DS
    {
        #region Fields
        protected const ushort HeaderSize = 6; // = (size of 'tag') + (size of 'size')
        protected BinaryReader currentSegment; //contains the current data segment
        protected DataReader3DS currentSubSegment; //contains the current data subsegment
        protected BinaryReader dataReader; //used to read data from the input System.IO.Stream
        protected ulong myStreamOffset; //the true position in the stream relative to start of sourceData
        protected ulong size; //specifies the size of the data subsegment
        protected byte[] sourceData; //array of source data
        protected MemoryStream sourceStream; //stream of source data
        protected ushort tag; //specifies what is contained in the data subsegment
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Creates a DataReader3DS object.
        /// </summary>
        /// <param name="inputData">Contains array of data bytes.</param>
        public DataReader3DS(byte[] inputData)
        {
            this.sourceData = inputData;
            this.myStreamOffset = 0;
            this.sourceStream = new MemoryStream(this.sourceData);
            this.dataReader = new BinaryReader(this.sourceStream);
            this.tag = this.dataReader.ReadUInt16();
            this.size = (ulong)this.dataReader.ReadInt32();
            this.currentSubSegment = null;
            this.currentSegment = this.dataReader;
        }
        /// <summary>
        /// Internal constructor that creates a DataReader3DS object.
        /// </summary>
        /// <param name="inputData">Contains array of data bytes.</param>
        /// <param name="startingPoint">Specified starting location.</param>
        /// <param name="length">Specified length.</param>
        protected DataReader3DS(byte[] inputData, int startingPoint, int length)
        {
            this.sourceData = inputData;
            this.myStreamOffset = (ulong)startingPoint;
            this.sourceStream = new MemoryStream(this.sourceData, startingPoint, length);
            this.dataReader = new BinaryReader(this.sourceStream);
            this.tag = this.dataReader.ReadUInt16();
            this.size = (ulong)this.dataReader.ReadInt32();
            this.currentSubSegment = null;
            this.currentSegment = this.dataReader;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the tag denoting the type of data in the current data segment.
        /// </summary>
        public ushort Tag
        {
            get { return this.tag; }
        }
        /// <summary>
        /// Returns the size of the current data reading segment.
        /// </summary>
        public ulong Size
        {
            get { return this.size; }
        }
        /// <summary>
        /// Returns the BinaryReader being used to read 3DS data from the stream.
        /// </summary>
        public BinaryReader DataReader
        {
            get { return this.dataReader; }
        }
        /*// <summary>
        /// Returns a reader that is looking at the current segment of 3DS data.
        /// </summary>
        public DataReader3DS CurrentSegment
        {
        get {
        return currentSegment;
        }
        }*/
        /// <summary>
        /// Returns a reader that is looking at the current subsegment of 3DS data.
        /// </summary>
        public DataReader3DS CurrentSubSegment
        {
            get { return this.currentSubSegment; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Moves to the next subsegment of data in the data stream.
        /// </summary>
        /// <returns>Returns the next subsegment of data in the data stream.</returns>
        public DataReader3DS GetNextSubSegment()
        {
            if(this.currentSegment.BaseStream.Position > 3700000){
                Debug.WriteLine("Breaking...");
            }
            if((ulong)this.currentSegment.BaseStream.Position < (this.size - HeaderSize)){
                if(this.currentSubSegment != null){
                    this.currentSegment.BaseStream.Position += (long)this.currentSubSegment.Size;
                    this.currentSubSegment = null;
                }
                //currentSubSegment = new DataReader3DS(sourceData,
                // (int)currentSegment.BaseStream.Position,
                // (int)(currentSegment.BaseStream.Length - currentSegment.BaseStream.Position));
                if((this.currentSegment.BaseStream.Length - this.currentSegment.BaseStream.Position) > 0){
                    this.currentSubSegment = new DataReader3DS(this.sourceData,
                                                               (int)
                                                               (this.myStreamOffset
                                                                + (ulong)this.currentSegment.BaseStream.Position),
                                                               (int)
                                                               (this.currentSegment.BaseStream.Length
                                                                - this.currentSegment.BaseStream.Position));
                }
            } else{
                this.currentSubSegment = null;
            }
            return this.currentSubSegment;
        }
        /// <summary>
        /// Reads a System.UInt16 from binary data in 3DS data stream.
        /// Assumes that the .NET implementation preserves correct endian-ness.
        /// </summary>
        /// <returns>Returns the next binary System.UInt16.</returns>
        public ushort GetUShort()
        {
            return this.currentSegment.ReadUInt16();
        }
        /// <summary>
        /// Reads a System.Byte from binary data in 3DS data stream.
        /// </summary>
        /// <returns>Returns the next binary System.Byte.</returns>
        public byte GetByte()
        {
            return this.currentSegment.ReadByte();
        }
        /// <summary>
        /// Reads a System.Single from binary data in 3DS data stream.
        /// Assumes that the .NET implementation preserves correct endian-ness.
        /// </summary>
        /// <returns>Returns the next binary System.Single.</returns>
        public float GetFloat()
        {
            return this.currentSegment.ReadSingle();
        }
        /// <summary>
        /// Reads a System.String from binary data in 3DS data stream.
        /// </summary>
        /// <returns>Returns the next binary System.String</returns>
        public string GetString()
        {
            string result = "";
            byte streamByte;
            do{
                streamByte = this.currentSegment.ReadByte();
                if(streamByte != '\0'){
                    result += ((char)streamByte).ToString();
                }
            } while(streamByte != '\0');
            return result;
        }
        #endregion
    }
}