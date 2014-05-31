// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBatchScreenshotsCapture.cs" company="">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   Provides an interface for capturing screenshots for multiple URLs and in multiple browsers in one batch call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for capturing screenshots for multiple URLs and in multiple browsers in one batch call.
    /// </summary>
    public interface IBatchScreenshotsCapture
    {
        #region Public Events

        /// <summary>
        /// The job completed.
        /// </summary>
        event BatchScreenshotsCapture.JobEvent JobCompleted;

        /// <summary>
        /// The job failed to start.
        /// </summary>
        event BatchScreenshotsCapture.JobFailureEvent JobFailedToStart;

        /// <summary>
        /// The job started.
        /// </summary>
        event BatchScreenshotsCapture.JobEvent JobStarted;

        /// <summary>
        /// The job state changed.
        /// </summary>
        event BatchScreenshotsCapture.JobEvent JobStateChanged;

        /// <summary>
        /// The screenshot completed.
        /// </summary>
        event BatchScreenshotsCapture.ScreenshotEvent ScreenshotCompleted;

        /// <summary>
        /// The screenshot failed.
        /// </summary>
        event BatchScreenshotsCapture.ScreenshotFailedEvent ScreenshotFailed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        ObservableCollection<Job> Jobs { get; }

        /// <summary>
        /// Gets the screenhots completed.
        /// </summary>
        ObservableCollection<Screenshot> ScreenhotsCompleted { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The execute batch async.
        /// </summary>
        /// <param name="rootPath">
        /// The root path.
        /// </param>
        /// <param name="usingTunnel">
        /// The using tunnel.
        /// </param>
        /// <param name="batchCaptureJobs">
        /// The batch capture jobs.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ExecuteBatchAsync(string rootPath, bool usingTunnel, params BatchCaptureJobInfo[] batchCaptureJobs);

        #endregion
    }
}