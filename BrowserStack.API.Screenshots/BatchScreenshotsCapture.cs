﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchScreenshotsCapture.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   Captures screenshots for multiple urls and in multiple browsers in one batch call.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BrowserStack.API.Screenshots.Configuration;

    #endregion

    /// <summary>
    /// Captures screenshots for multiple urls and in multiple browsers in one batch call.
    /// </summary>
    /// <remarks>
    /// This class makes multiple calls to the BrowserStack screenshots API in order to
    /// start the necessary jobs to capture all the combinations of urls and browsers. For each screenshot
    /// taken it saves the corresponding file to a local path.
    /// </remarks>
    public sealed class BatchScreenshotsCapture
    {
        #region Constants and Fields

        /// <summary>
        /// Specifies whether to capture the thumbnails along with the actual screenshots.
        /// </summary>
        private readonly bool captureThumbnails;

        /// <summary>
        /// The screenshots API.
        /// </summary>
        private readonly ScreenshotsApi screenshotsApi;

        /// <summary>
        /// The session limit.
        /// </summary>
        private readonly int sessionLimit;

        /// <summary>
        /// The session lock.
        /// </summary>
        private readonly object sessionLock = new object();

        /// <summary>
        /// The current batch jobs to run.
        /// </summary>
        private List<BatchCaptureJobInfo> currentBatchJobsToRun;

        /// <summary>
        /// The URL filenames.
        /// </summary>
        private IDictionary<string, string> urlFilenames;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchScreenshotsCapture" /> class.
        /// </summary>
        /// <param name="sessionLimit">The session limit.</param>
        /// <param name="captureThumbnails">if set to <c>true</c> then the batch job will also save the thumbnails when saving the screenshots.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public BatchScreenshotsCapture(int sessionLimit, bool captureThumbnails, string username, string password)
        {
            this.sessionLimit = sessionLimit;
            this.captureThumbnails = captureThumbnails;

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                this.screenshotsApi = new ScreenshotsApi();
            }
            else
            {
                this.screenshotsApi = new ScreenshotsApi(username, password);
            }

            this.Jobs = new ObservableCollection<Job>();
            this.ScreenhotsCompleted = new ObservableCollection<Screenshot>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchScreenshotsCapture"/> class.
        /// </summary>
        /// <remarks>Use this constructor when you want to configure the batch screenshots capture through the application configuration file.</remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown if there are errors in the application configuration section.</exception>
        public BatchScreenshotsCapture()
        {
            var config = ConfigurationSectionManager.Configuration;
            
            this.sessionLimit = config.Batch.SessionLimit;
            this.captureThumbnails = config.Batch.CaptureThumbnails;

            this.screenshotsApi = new ScreenshotsApi();

            this.Jobs = new ObservableCollection<Job>();
            this.ScreenhotsCompleted = new ObservableCollection<Screenshot>();
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The job event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public delegate void JobEvent(object sender, JobEventArgs args);

        /// <summary>
        /// The job failure event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="JobFailureEventArgs"/> instance containing the event data.</param>
        public delegate void JobFailureEvent(object sender, JobFailureEventArgs args);

        /// <summary>
        /// The screenshot event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public delegate void ScreenshotEvent(object sender, ScreenshotEventArgs args);

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when a job is completed (either timed out or all screenshots saved).
        /// </summary>
        public event JobEvent JobCompleted;

        /// <summary>
        /// Occurs when a job is started in BrowserStack.
        /// </summary>
        public event JobEvent JobStarted;

        /// <summary>
        /// Occurs when a job failed to start.
        /// </summary>
        /// <remarks>Warning, this event is not raised for all jobs that failed, only for jobs that failed to start.</remarks>
        public event JobFailureEvent JobFailedToStart;

        /// <summary>
        /// Occurs when a job's status changes.
        /// </summary>
        public event JobEvent JobStateChanged;

        /// <summary>
        /// The screenshot completed.
        /// </summary>
        public event ScreenshotEvent ScreenshotCompleted;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the BrowserStack jobs that have been initiated.
        /// </summary>
        /// <remarks>Whenever a screenshot job is started it is added to this list and whenever it is finished it is replaced with the new values.</remarks>
        public ObservableCollection<Job> Jobs { get; private set; }

        /// <summary>
        /// Gets the screenhots completed so far.
        /// </summary>
        public ObservableCollection<Screenshot> ScreenhotsCompleted { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the job asynchronously.
        /// </summary>
        /// <param name="rootPath">The root path where to save the screenshots to.</param>
        /// <param name="usingTunnel">set to <c>true</c> if the BrowserStack jobs need to run under a tunnel. The tunnel must have been initiated externally.</param>
        /// <param name="batchCaptureJobs">Information about the jobs to run.</param>
        /// <returns>
        /// A <see cref="Task" /> to await for completion.
        /// </returns>
        public async Task StartJobAsync(string rootPath, bool usingTunnel, params BatchCaptureJobInfo[] batchCaptureJobs)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException("rootPath");
            }

            if (Directory.Exists(rootPath))
            {
                throw new ArgumentException("The root path where to save the screenshots to must not exist, it will be created by the batch job.", rootPath);
            }

            if (!batchCaptureJobs.Any())
            {
                throw new ArgumentException("At least one job must be defined.", "batchCaptureJobs");
            }

            // Create the root folder to save the screenshots to. If this succeed then probably the rest of the folder and file creation will also succeed.
            Directory.CreateDirectory(rootPath);
            this.currentBatchJobsToRun = this.SanitizeBatchJobs(batchCaptureJobs).ToList();
            this.urlFilenames = this.currentBatchJobsToRun.ToDictionary(x => x.Url, x => x.Filename);

            var runningSessions = 0;
            await Task.WhenAll(
                this.currentBatchJobsToRun.Select(
                    async jobToRun =>
                    {
                        var jobRun = false;
                        while (!jobRun)
                        {
                            var isSessionFree = false;
                            if (runningSessions < this.sessionLimit)
                            {
                                lock (this.sessionLock)
                                {
                                    if (runningSessions < this.sessionLimit)
                                    {
                                        runningSessions++;
                                        isSessionFree = true;
                                    }
                                }
                            }

                            if (isSessionFree)
                            {
                                Job job = null;

                                try
                                {
                                    job = await this.screenshotsApi.StartJobAsync(jobToRun.Url, jobToRun.JobInfo, usingTunnel, jobToRun.Browsers);
                                }
                                catch (Exception ex)
                                {
                                    // Release the session since this job has failed.
                                    lock (this.sessionLock)
                                    {
                                        jobRun = true;
                                        runningSessions--;
                                    }

                                    // Notify that the job has failed to start.
                                    this.OnJobFailedToStart(new JobFailureEventArgs(jobToRun.Url, jobToRun.JobInfo, jobToRun.Browsers, ex));
                                }

                                if (job != null)
                                {
                                    try
                                    {
                                        this.Jobs.Add(job);
                                        this.OnJobStarted(new JobEventArgs(job));

                                        var completedJob = await this.HandleScreenshotsJobAsync(job.Id, rootPath);
                                        this.Jobs[this.Jobs.IndexOf(this.Jobs.First(x => x.Id == job.Id))] = completedJob;
                                        this.OnJobCompleted(new JobEventArgs(completedJob));
                                    }
                                    finally
                                    {
                                        // Release the session since this job has completed.
                                        lock (this.sessionLock)
                                        {
                                            jobRun = true;
                                            runningSessions--;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    }));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:JobCompleted" /> event.
        /// </summary>
        /// <param name="args">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobCompleted(JobEventArgs args)
        {
            var handler = this.JobCompleted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobFailedToStart" /> event.
        /// </summary>
        /// <param name="args">The <see cref="JobFailureEventArgs"/> instance containing the event data.</param>
        private void OnJobFailedToStart(JobFailureEventArgs args)
        {
            JobFailureEvent handler = this.JobFailedToStart;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobStarted" /> event.
        /// </summary>
        /// <param name="args">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobStarted(JobEventArgs args)
        {
            var handler = this.JobStarted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobStateChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobStateChanged(JobEventArgs args)
        {
            var handler = this.JobStateChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ScreenshotCompleted" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ScreenshotEventArgs"/> instance containing the event data.</param>
        private void OnScreenshotCompleted(ScreenshotEventArgs args)
        {
            var handler = this.ScreenshotCompleted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Sanitizes the batch jobs since BrowserStack only supports 25 browsers per url.
        /// </summary>
        /// <param name="batchCaptureJobs">The batch capture jobs.</param>
        /// <returns>
        /// A list of sanitized batch jobs.
        /// </returns>
        private IEnumerable<BatchCaptureJobInfo> SanitizeBatchJobs(IEnumerable<BatchCaptureJobInfo> batchCaptureJobs)
        {
            foreach (var job in batchCaptureJobs)
            {
                if (job.Browsers.Length <= 25)
                {
                    yield return job;
                }
                else
                {
                    // Split browsers into groups of 25 items
                    var splitBrowsers = job.Browsers.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 25).Select(x => x.Select(v => v.Value));

                    foreach (var splitBrowser in splitBrowsers)
                    {
                        yield return new BatchCaptureJobInfo(job.Url, job.Filename, job.JobInfo, splitBrowser.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// The create screenshot directory.
        /// </summary>
        /// <param name="jobInfo">The job info.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="rootPath">The root path.</param>
        /// <returns>
        /// The <see cref="DirectoryInfo" />.
        /// </returns>
        private DirectoryInfo CreateScreenshotDirectory(Job.JobInfo jobInfo, Browser browser, string rootPath)
        {
            const string deviceFolderStructureTemplate = @"{0}\{1}\{2}\{3}";
            const string browserFolderStructureTemplate = @"{0}\{1}\{2}\{3}\{4}";

            string screenshotFolderName;

            if (string.IsNullOrEmpty(browser.Device))
            {
                screenshotFolderName = string.Format(
                    browserFolderStructureTemplate, 
                    browser.OS, 
                    browser.OSVersion, 
                    browser.BrowserName, 
                    browser.BrowserVersion, 
                    (browser.OS.ToLower().Contains("os x") ? jobInfo.OsxResolution.ToString() : jobInfo.WinResolution.ToString()).Replace("R_", string.Empty));
            }
            else
            {
                screenshotFolderName = string.Format(deviceFolderStructureTemplate, browser.OS, browser.OSVersion, browser.Device, jobInfo.Orientation);
            }

            return Directory.CreateDirectory(Path.Combine(rootPath, screenshotFolderName));
        }

        /// <summary>
        /// Handles a screenshots job asynchronously.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="rootPath">The root path.</param>
        /// <returns>
        /// A <see cref="Task{Job}" />.
        /// </returns>
        /// <remarks>
        /// This method periodically connects to the BrowserStack API to retrieve a job's status and saves the screenshot images
        /// for those screenshots that complete.
        /// </remarks>
        private async Task<Job> HandleScreenshotsJobAsync(string jobId, string rootPath)
        {
            var handledScreenshotsDictionary = new Dictionary<string, Screenshot>();
            var screenshotTasks = new List<Task>();

            Job job = null;

            while (true)
            {
                job = this.screenshotsApi.GetJobInfoAsync(jobId).Result;

                if (this.Jobs.First(x => x.Id == job.Id).State != job.State)
                {
                    this.OnJobStateChanged(new JobEventArgs(job));
                }

                ////// Serialize job info to drive
                ////var serializer = new DataContractSerializer(typeof(Job));
                ////using (var fs = File.Open(string.Format(@"{0}\Job_{1}.xml", this.rootFolderToSaveTo, job.Id), FileMode.Create))
                ////{
                ////    serializer.WriteObject(fs, job);
                ////}

                // While the job is processing, start tasks to capture the screenshots that have been generated so far
                // Note: Not really sure which of the states is valid for a job
                if (job.State == Job.States.Processing || job.State == Job.States.Queue)
                {
                    // Get the screenshots that need to be handled. These should not have been handled before.
                    var screenshotsToHandle =
                        job.Screenshots.Where(x => !handledScreenshotsDictionary.ContainsKey(x.Id)).Where(x => x.State == Screenshot.States.Done || x.State == Screenshot.States.TimedOut).ToList();

                    foreach (var screenshot in screenshotsToHandle)
                    {
                        // First add the screenshot to the list of already handled screenshots
                        handledScreenshotsDictionary.Add(screenshot.Id, screenshot);

                        // Then start a task for asynchronously saving the screenshot to a file
                        var screenshotTask = this.SaveScreenshotAsync(screenshot, job.Info, rootPath);
                        screenshotTask.Start();

                        // Finally add the screenshot to the list of tasks that need to be completed
                        screenshotTasks.Add(screenshotTask);
                    }
                }

                if (job.IsComplete)
                {
                    break;
                }

                // Wait for some time before reading the job status again
                Thread.Sleep(4000);
            }

            await Task.WhenAll(screenshotTasks.ToArray());

            return job;
        }

        /// <summary>
        /// The save screenshot async.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="jobInfo">The job info.</param>
        /// <param name="rootPath">The root path.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        private async Task SaveScreenshotAsync(Screenshot screenshot, Job.JobInfo jobInfo, string rootPath)
        {
            // Create the folder for the screenshot
            var directory = this.CreateScreenshotDirectory(jobInfo, screenshot.Browser, rootPath);
            var filename = string.Format("{0}.png", this.urlFilenames.ContainsKey(screenshot.Url) ? this.urlFilenames[screenshot.Url] : screenshot.Id);
            var screenshotFullPath = Path.Combine(directory.FullName, filename + ".png");
            var thumbnailFullPath = this.captureThumbnails ? Path.Combine(directory.FullName, filename + "_thumbnail.png") : null;

            if (screenshot.State == Screenshot.States.Done)
            {
                if (!File.Exists(screenshotFullPath))
                {
                    await this.screenshotsApi.SaveScreenshotToFileAsync(screenshot, screenshotFullPath, thumbnailFullPath);
                }
            }
            else
            {
                File.WriteAllText(Path.Combine(directory.FullName, filename + string.Format("_{0}.txt", screenshot.State)), string.Empty);
            }

            this.ScreenhotsCompleted.Add(screenshot);
            this.OnScreenshotCompleted(new ScreenshotEventArgs(screenshot));
        }

        #endregion
    }

    /// <summary>
    /// The job failure event args.
    /// </summary>
    public class JobFailureEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobFailureEventArgs" /> class.
        /// </summary>
        /// <param name="jobUrl">The job URL.</param>
        /// <param name="jobInfo">The job information.</param>
        /// <param name="browsers">The browsers.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        public JobFailureEventArgs(string jobUrl, Job.JobInfo jobInfo, IEnumerable<Browser> browsers, Exception exception)
        {
            this.Exception = exception;
            this.Browsers = browsers;
            this.JobInfo = jobInfo;
            this.JobUrl = jobUrl;
        }

        /// <summary>
        /// Gets the job URL.
        /// </summary>
        public string JobUrl { get; private set; }

        /// <summary>
        /// Gets the job information.
        /// </summary>
        public Job.JobInfo JobInfo { get; private set; }

        /// <summary>
        /// Gets the browsers that were used to initiate the job.
        /// </summary>
        public IEnumerable<Browser> Browsers { get; private set; }

        /// <summary>
        /// Gets the exception that caused the failure.
        /// </summary>
        public Exception Exception { get; private set; }
    }

    /// <summary>
    /// The screenshot event args.
    /// </summary>
    public class ScreenshotEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotEventArgs" /> class.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="thumbnailFilePath">The thumbnail file path.</param>
        public ScreenshotEventArgs(Screenshot screenshot, string imageFilePath = null, string thumbnailFilePath = null)
        {
            this.ThumbnailFilePath = thumbnailFilePath;
            this.ImageFilePath = imageFilePath;
            this.Screenshot = screenshot;
            this.State = screenshot.State;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the image file path.
        /// </summary>
        public string ImageFilePath { get; private set; }

        /// <summary>
        /// Gets the screenshot.
        /// </summary>
        public Screenshot Screenshot { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public Screenshot.States State { get; private set; }

        /// <summary>
        /// Gets the thumbnail file path.
        /// </summary>
        public string ThumbnailFilePath { get; private set; }

        #endregion
    }

    /// <summary>
    /// The job event args.
    /// </summary>
    public class JobEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JobEventArgs"/> class.
        /// </summary>
        /// <param name="job">
        /// The job.
        /// </param>
        public JobEventArgs(Job job)
        {
            this.Job = job;
            this.State = job.State;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the job.
        /// </summary>
        public Job Job { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public Job.States State { get; private set; }

        #endregion
    }
}