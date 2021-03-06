﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchCaptureJobInfo.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   The batch capture job info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    /// The batch capture job info.
    /// </summary>
    public class BatchCaptureJobInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The illegal file name characters regex.
        /// </summary>
        private static readonly Regex illegalFileNameCharactersRegex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="BatchCaptureJobInfo"/> class.
        /// </summary>
        static BatchCaptureJobInfo()
        {
            // Taken from http://stackoverflow.com/a/146162/533420
            var illegalFileNameCharacters = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + ":";
            illegalFileNameCharactersRegex = new Regex(string.Format("[{0}]", Regex.Escape(illegalFileNameCharacters)), RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCaptureJobInfo"/> class.
        /// </summary>
        /// <param name="url">
        /// The URL to capture.
        /// </param>
        /// <param name="filenameTemplate">
        /// The filename template that will be used to capture the screenshot.
        /// </param>
        /// <param name="jobInfo">
        /// The job information.
        /// </param>
        /// <param name="browsers">
        /// The browsers.
        /// </param>
        public BatchCaptureJobInfo(string url, string filenameTemplate, Job.JobInfo jobInfo, params Browser[] browsers)
        {
            Contract.Requires(!string.IsNullOrEmpty(url));
            Contract.Requires(!string.IsNullOrEmpty(filenameTemplate));
            Contract.Requires(!illegalFileNameCharactersRegex.IsMatch(filenameTemplate), "The filename contains illegal characters.");
            Contract.Requires(jobInfo != null);
            Contract.Requires(browsers != null);
            Contract.Requires(browsers.Length > 1);

            this.Url = url;
            this.Filename = filenameTemplate;
            this.JobInfo = jobInfo;
            this.Browsers = browsers;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the browsers.
        /// </summary>
        public Browser[] Browsers { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the job info.
        /// </summary>
        public Job.JobInfo JobInfo { get; private set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url { get; private set; }

        #endregion
    }
}