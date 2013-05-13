using System;

namespace AviFile {
    public abstract class AviStream {
        protected int aviFile;
        protected IntPtr aviStream;
        protected IntPtr compressedStream;
        protected bool writeCompressed;

        /// <summary>Pointer to the unmanaged AVI file</summary>
        internal int FilePointer {
            get { return this.aviFile; }
        }

        /// <summary>Pointer to the unmanaged AVI Stream</summary>
        internal virtual IntPtr StreamPointer {
            get { return this.aviStream; }
        }

        /// <summary>Flag: The stream is compressed/uncompressed</summary>
        internal bool WriteCompressed {
            get { return this.writeCompressed; }
        }

        /// <summary>Close the stream</summary>
        public virtual void Close() {
            if (this.writeCompressed) {
                Avi.AVIStreamRelease(this.compressedStream);
            }
            Avi.AVIStreamRelease(this.StreamPointer);
        }

        /// <summary>Export the stream into a new file</summary>
        /// <param name="fileName"></param>
        public abstract void ExportStream(String fileName);
    }
}