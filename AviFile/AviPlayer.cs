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

#region Using directives

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace AviFile {
    public class AviPlayer {
        private readonly Control ctlFrameIndexFeedback;
        private Bitmap currentBitmap;
        private int currentFrameIndex;
        private bool isRunning;
        private int millisecondsPerFrame;
        private readonly PictureBox picDisplay;
        private readonly VideoStream videoStream;

        /// <summary>Create a new AVI Player</summary>
        /// <param name="videoStream">Video stream to play</param>
        /// <param name="picDisplay">PictureBox to display the video</param>
        /// <param name="ctlFrameIndexFeedback">Optional Label to show the current frame index</param>
        public AviPlayer(VideoStream videoStream, PictureBox picDisplay, Control ctlFrameIndexFeedback) {
            this.videoStream = videoStream;
            this.picDisplay = picDisplay;
            this.ctlFrameIndexFeedback = ctlFrameIndexFeedback;
            this.isRunning = false;
        }

        /// <summary>Returns the current playback status</summary>
        public bool IsRunning {
            get { return this.isRunning; }
        }

        public event EventHandler Stopped;

        /// <summary>Start the video playback</summary>
        public void Start() {
            this.isRunning = true;
            this.millisecondsPerFrame = (int) (1000 / this.videoStream.FrameRate);
            Thread thread = new Thread(new ThreadStart(this.Run));
            thread.Start();
        }

        /// <summary>Extract and display the frames</summary>
        private void Run() {
            this.videoStream.GetFrameOpen();
            for (this.currentFrameIndex = 0;
                 (this.currentFrameIndex < this.videoStream.CountFrames) && this.isRunning;
                 this.currentFrameIndex++) {
                //show frame
                this.currentBitmap = this.videoStream.GetBitmap(this.currentFrameIndex);
                this.picDisplay.Invoke(new SimpleDelegate(this.SetDisplayPicture));
                this.picDisplay.Invoke(new SimpleDelegate(this.picDisplay.Refresh));
                //show position
                if (this.ctlFrameIndexFeedback != null) {
                    this.ctlFrameIndexFeedback.Invoke(new SimpleDelegate(this.SetLabelText));
                }
                //wait for the next frame
                Thread.Sleep(this.millisecondsPerFrame);
            }
            this.videoStream.GetFrameClose();
            this.isRunning = false;
            if (this.Stopped != null) {
                this.Stopped(this, EventArgs.Empty);
            }
        }

        /// <summary>Change the visible frame</summary>
        private void SetDisplayPicture() {
            this.picDisplay.Image = this.currentBitmap;
        }

        /// <summary>Change the frame index feedback</summary>
        private void SetLabelText() {
            this.ctlFrameIndexFeedback.Text = this.currentFrameIndex.ToString();
        }

        /// <summary>Stop the video playback</summary>
        public void Stop() {
            this.isRunning = false;
        }

        #region Nested type: SimpleDelegate

        private delegate void SimpleDelegate();

        #endregion
    }
}