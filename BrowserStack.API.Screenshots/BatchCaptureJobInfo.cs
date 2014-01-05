namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text.RegularExpressions;

    #endregion

    public class BatchCaptureJobInfo
    {
        #region Constants and Fields

        private static readonly Regex illegalFileNameCharactersRegex;

        #endregion

        #region Constructors and Destructors

        static BatchCaptureJobInfo()
        {
            // Taken from http://stackoverflow.com/a/146162/533420
            var illegalFileNameCharacters = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            illegalFileNameCharactersRegex = new Regex(string.Format("[{0}]", Regex.Escape(illegalFileNameCharacters)), RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCaptureJobInfo"/> class.
        /// </summary>
        /// <param name="url">The URL to capture.</param>
        /// <param name="filenameTemplate">The filename template that will be used to capture the screenshot.</param>
        /// <param name="jobInfo">The job information.</param>
        /// <param name="browsers">The browsers.</param>
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

        public Browser[] Browsers { get; private set; }
        public Job.JobInfo JobInfo { get; private set; }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url { get; private set; }

        #endregion
    }
}