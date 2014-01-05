namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents a screenshot capture job in BrowserStack.
    /// </summary>
    public class Job
    {
        #region Enums

        /// <summary>
        /// The possible orientations of a mobile device.
        /// </summary>
        public enum Orientations
        {
            /// <summary>
            /// Portrait orientation.
            /// </summary>
            Portrait,

            /// <summary>
            /// Landscape orientation.
            /// </summary>
            Landscape
        }

        /// <summary>
        /// The possible screenshot qualities.
        /// </summary>
        public enum Qualities
        {
            /// <summary>
            /// Original quality, the screenshot is captured as is.
            /// </summary>
            Original,

            /// <summary>
            /// Compressed quality, the screenshot is compressed after being captured.
            /// </summary>
            Compressed,
        }

        /// <summary>
        /// The possible screen resolutions for OSX operating systems.
        /// </summary>
        public enum OSXResolutions
        {
            /// <summary>
            /// 1024x768 resolution.
            /// </summary>
            R_1024x768,

            /// <summary>
            /// 1280x968 resolution.
            /// </summary>
            R_1280x960,

            R_1280x1024,

            R_1600x1200,

            R_1920x1080,
        }

        /// <summary>
        /// The possible screen resolutions for Windows operating systems.
        /// </summary>
        public enum WinResolutions
        {
            R_1024x768,

            R_1280x1024,
        }

        /// <summary>
        /// The possible states of a screenshot.
        /// </summary>
        /// <remarks>This list is neither exhaustive, neither 100% correct, inconsistencies can be found when calling the API.</remarks>
        public enum States
        {
            /// <summary>
            /// The job has not yet been started.
            /// </summary>
            Pending = 0,

            /// <summary>
            /// The job has been queued for execution.
            /// </summary>
            Queue,

            /// <summary>
            /// The job has timed out.
            /// </summary>
            TimedOut,

            /// <summary>
            /// The job was successfully completed.
            /// </summary>
            Done,

            /// <summary>
            /// The job is currently processing.
            /// </summary>
            Processing,
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a unique identifier of the job.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the job information that was used to start the job.
        /// </summary>
        public JobInfo Info { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the job has completed.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return this.State == States.Done || this.State == States.TimedOut;
            }
        }

        /// <summary>
        /// Gets the screenshots that belong to the job.
        /// </summary>
        public IEnumerable<Screenshot> Screenshots { get; internal set; }

        /// <summary>
        /// Gets or sets the state of the job.
        /// </summary>
        public States State { get; set; }

        #endregion

        /// <summary>
        /// Describes the configuration of the BrowserStack screenshot job
        /// </summary>
        public class JobInfo
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the callback URL where results of the job will be posted to.
            /// </summary>
            public string CallbackUrl { get; set; }

            /// <summary>
            /// Gets or sets the resolution that will be used to capture screenshots on Mac browsers.
            /// </summary>
            public OSXResolutions? OsxResolution { get; set; }

            /// <summary>
            /// Gets or sets the orientation to be used for mobile browsers.
            /// </summary>
            public Orientations? Orientation { get; set; }
            
            /// <summary>
            /// Gets or sets the quality to be used when capturing screenshots.
            /// </summary>
            public Qualities? Quality { get; set; }

            /// <summary>
            /// Gets or sets the wait time (in seconds).
            /// </summary>
            public int WaitTime { get; set; }

            /// <summary>
            /// Gets or sets the resolution that will be used to capture screenshots on Windows browsers.
            /// </summary>
            public WinResolutions? WinResolution { get; set; }

            #endregion
        }
    }
}