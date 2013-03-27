/* This class has been written by
 * Corinna John (Hannover, Germany)
 * cj@binary-universe.net
 * 
 * You may do with this code whatever you like,
 * except selling it or claiming any rights/ownership.
 * 
 * Please send me a little feedback about what you're
 * using this code for and what changes you'd like to
 * see in later versions. (And please excuse my bad english.)
 * 
 * WARNING: This is experimental code.
 * Please do not expect "Release Quality".
 * */

using System;
using System.Runtime.InteropServices;

namespace AviFile {
    public class AudioStream : AviStream {
        /// <summary>the stream's format</summary>
        private Avi.PCMWAVEFORMAT waveFormat = new Avi.PCMWAVEFORMAT();

        /// <summary>Initialize an AudioStream for an existing stream</summary>
        /// <param name="aviFile">The file that contains the stream</param>
        /// <param name="aviStream">An IAVISTREAM from [aviFile]</param>
        public AudioStream(int aviFile, IntPtr aviStream) {
            this.aviFile = aviFile;
            this.aviStream = aviStream;
            int size = Marshal.SizeOf(this.waveFormat);
            Avi.AVIStreamReadFormat(aviStream, 0, ref this.waveFormat, ref size);
            Avi.AVISTREAMINFO streamInfo = this.GetStreamInfo(aviStream);
        }

        public int CountBitsPerSample {
            get { return this.waveFormat.wBitsPerSample; }
        }

        public int CountSamplesPerSecond {
            get { return this.waveFormat.nSamplesPerSec; }
        }

        public int CountChannels {
            get { return this.waveFormat.nChannels; }
        }

        /// <summary>Read the stream's header information</summary>
        /// <param name="aviStream">The IAVISTREAM to read from</param>
        /// <returns>AVISTREAMINFO</returns>
        private Avi.AVISTREAMINFO GetStreamInfo(IntPtr aviStream) {
            Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
            int result = Avi.AVIStreamInfo(aviStream, ref streamInfo, Marshal.SizeOf(streamInfo));
            if (result != 0) {
                throw new Exception("Exception in AVIStreamInfo: " + result.ToString());
            }
            return streamInfo;
        }

        /// <summary>Read the stream's header information</summary>
        /// <returns>AVISTREAMINFO</returns>
        public Avi.AVISTREAMINFO GetStreamInfo() {
            if (this.writeCompressed) {
                return this.GetStreamInfo(this.compressedStream);
            } else {
                return this.GetStreamInfo(this.aviStream);
            }
        }

        /// <summary>Read the stream's format information</summary>
        /// <returns>PCMWAVEFORMAT</returns>
        public Avi.PCMWAVEFORMAT GetFormat() {
            Avi.PCMWAVEFORMAT format = new Avi.PCMWAVEFORMAT();
            int size = Marshal.SizeOf(format);
            int result = Avi.AVIStreamReadFormat(this.aviStream, 0, ref format, ref size);
            return format;
        }

        /// <summary>Returns all data needed to copy the stream</summary>
        /// <remarks>Do not forget to call Marshal.FreeHGlobal and release the raw data pointer</remarks>
        /// <param name="streamInfo">Receives the header information</param>
        /// <param name="format">Receives the format</param>
        /// <param name="streamLength">Receives the length of the stream</param>
        /// <returns>Pointer to the wave data</returns>
        public IntPtr GetStreamData(ref Avi.AVISTREAMINFO streamInfo, ref Avi.PCMWAVEFORMAT format, ref int streamLength) {
            streamInfo = this.GetStreamInfo();
            format = this.GetFormat();
            //length in bytes = length in samples * length of a sample
            streamLength = Avi.AVIStreamLength(this.aviStream.ToInt32()) * streamInfo.dwSampleSize;
            IntPtr waveData = Marshal.AllocHGlobal(streamLength);
            int result = Avi.AVIStreamRead(this.aviStream, 0, streamLength, waveData, streamLength, 0, 0);
            if (result != 0) {
                throw new Exception("Exception in AVIStreamRead: " + result.ToString());
            }
            return waveData;
        }

        /// <summary>Copy the stream into a new file</summary>
        /// <param name="fileName">Name of the new file</param>
        public override void ExportStream(String fileName) {
            Avi.AVICOMPRESSOPTIONS_CLASS opts = new Avi.AVICOMPRESSOPTIONS_CLASS();
            opts.fccType = (UInt32) Avi.mmioStringToFOURCC("auds", 0);
            opts.fccHandler = (UInt32) Avi.mmioStringToFOURCC("CAUD", 0);
            opts.dwKeyFrameEvery = 0;
            opts.dwQuality = 0;
            opts.dwFlags = 0;
            opts.dwBytesPerSecond = 0;
            opts.lpFormat = new IntPtr(0);
            opts.cbFormat = 0;
            opts.lpParms = new IntPtr(0);
            opts.cbParms = 0;
            opts.dwInterleaveEvery = 0;
            Avi.AVISaveV(fileName, 0, 0, 1, ref this.aviStream, ref opts);
        }
    }
}