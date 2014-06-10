// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScreenshotsApi.cs" company="">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   Provides an interface around a REST client for connecting to Browserstack's screenshots API. See http://www.browserstack.com/screenshots/api.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface around a REST client for connecting to Browserstack's screenshots API. See http://www.browserstack.com/screenshots/api.
    /// </summary>
    public interface IScreenshotsApi
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether authenticate for get browsers.
        /// </summary>
        bool AuthenticateForGetBrowsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authenticate for get job info.
        /// </summary>
        bool AuthenticateForGetJobInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authenticate for get screenshot images.
        /// </summary>
        bool AuthenticateForGetScreenshotImages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authenticate for start job.
        /// </summary>
        bool AuthenticateForStartJob { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a list of the supported browsers from BrowserStack. See http://www.browserstack.com/list-of-browsers-and-platforms?product=screenshots.
        /// </summary>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        Task<IEnumerable<Browser>> GetBrowsersAsync();

        /// <summary>
        /// Retrieves a job's information.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        Task<Job> GetJobInfoAsync(string jobId);

        /// <summary>
        /// Saves a screenshot to a local path.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="path">The path where the image will be saved to.</param>
        /// <param name="filename">
        /// The name of the file where the image will be saved to. The extension will be added automatically according to the image path. 
        /// If no filename is supplied then the original filename from the BrowserStack url will be used.
        /// </param>
        /// <param name="overwrite">If set to true then the file will be overwritten, otherwise no attempt will be made to retrieve the file.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        Task SaveScreenshotToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false);

        /// <summary>
        /// Saves a screenshot's thumbnail to a local path.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="path">The path where the image will be saved to.</param>
        /// <param name="filename">
        /// The name of the file where the image will be saved to. The extension will be added automatically according to the image path. 
        /// If no filename is supplied then the original filename from the BrowserStack url will be used.
        /// </param>
        /// <param name="overwrite">If set to true then the file will be overwritten, otherwise no attempt will be made to retrieve the file.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        Task SaveThumbnailToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false);

        /// <summary>
        /// Starts a BrowserStack screenshot job asynchronously.
        /// </summary>
        /// <param name="url">The url for which screenshots are required.</param>
        /// <param name="jobInfo">The job information that will be used to start the job.</param>
        /// <param name="usingTunnel">set to <c>true</c> if the BrowserStack jobs need to run under a tunnel. The tunnel must have been initiated externally.</param>
        /// <param name="browsers">The browsers that will be used to start the job.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        /// <exception cref="ApplicationException">Thrown when the call to the BrowserStack API results in an http code other than 200 (OK).</exception>
        Task<Job> StartJobAsync(string url, Job.JobInfo jobInfo, bool usingTunnel = false, params Browser[] browsers);

        #endregion
    }
}