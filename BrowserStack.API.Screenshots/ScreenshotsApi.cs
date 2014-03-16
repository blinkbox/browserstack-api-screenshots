// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenshotsApi.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   Provides a REST client for connecting to Browserstack's screenshots API. See http://www.browserstack.com/screenshots/api.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using BrowserStack.API.Screenshots.ApiModels;
    using BrowserStack.API.Screenshots.Configuration;

    using Newtonsoft.Json;
    using System.Diagnostics.Contracts;

    #endregion

    /// <summary>
    /// Provides a REST client for connecting to Browserstack's screenshots API. See http://www.browserstack.com/screenshots/api.
    /// </summary>
    public sealed class ScreenshotsApi
    {
        #region Constants and Fields

        /// <summary>
        /// The screenshots rest API base url.
        /// </summary>
        private const string screenshotsRestAPIBaseUrl = "http://www.browserstack.com/screenshots/";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotsApi"/> class.
        /// </summary>
        /// <remarks>Use this constructor when you want to configure the screenshots API through the application configuration file.</remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Thrown if there are errors in the application configuration section.</exception>
        public ScreenshotsApi()
        {
            var config = ConfigurationSectionManager.Configuration;

            this.Username = config.Authentication.Username;
            this.Password = config.Authentication.Password;
            this.AuthenticateForGetBrowsers = config.Authentication.AuthenticateForGetBrowsers;
            this.AuthenticateForGetJobInfo = config.Authentication.AuthenticateForGetJobInfo;
            this.AuthenticateForGetScreenshotImages = config.Authentication.AuthenticateForGetScreenshotImages;
            this.AuthenticateForStartJob = config.Authentication.AuthenticateForStartJob;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotsApi"/> class.
        /// </summary>
        /// <param name="username">
        /// The username used to connect to BrowserStack.
        /// </param>
        /// <param name="password">
        /// The password used to connect to BrowserStack.
        /// </param>
        public ScreenshotsApi(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.AuthenticateForGetBrowsers = false;
            this.AuthenticateForGetJobInfo = false;
            this.AuthenticateForGetScreenshotImages = false;
            this.AuthenticateForStartJob = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to authenticate when getting the list of browsers.
        /// </summary>
        public bool AuthenticateForGetBrowsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to authenticate when getting a job's information.
        /// </summary>
        public bool AuthenticateForGetJobInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to authenticate when getting a screenshot image.
        /// </summary>
        public bool AuthenticateForGetScreenshotImages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to authenticate when starting a new job.
        /// </summary>
        public bool AuthenticateForStartJob { get; set; }

        /// <summary>
        /// Gets the password used to connect to BrowserStack.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the username used to connect to BrowserStack.
        /// </summary>
        public string Username { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a list of the supported browsers from BrowserStack. See http://www.browserstack.com/list-of-browsers-and-platforms?product=screenshots.
        /// </summary>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public async Task<IEnumerable<Browser>> GetBrowsersAsync()
        {
            using (var httpClient = new HttpClient())
            {
                // This call does not require authentication
                var response = await httpClient.GetAsync(screenshotsRestAPIBaseUrl + "browsers");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                var browsers = await JsonConvert.DeserializeObjectAsync<IEnumerable<BrowserInfo>>(data);

                return browsers.Select(x => MapInfoToBrowser(x));
            }
        }

        /// <summary>
        /// Retrieves a job's information.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public async Task<Job> GetJobInfoAsync(string jobId)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, screenshotsRestAPIBaseUrl + jobId + ".json");
                if (this.AuthenticateForGetJobInfo)
                {
                    request.Headers.Authorization = this.GetAuthenticationHeader();
                }

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                var screenshotJob = await JsonConvert.DeserializeObjectAsync<ScreenshotJob>(data);

                return MapToJob(screenshotJob);
            }
        }

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
        public async Task SaveScreenshotToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false)
        {
            Contract.Requires(screenshot != null);
            Contract.Requires(path != null);

            using (var httpClient = new HttpClient())
            {
                var fullPath = string.IsNullOrEmpty(filename)
                    ? Path.Combine(path, Path.GetFileName(new Uri(screenshot.ImageUrl).AbsolutePath))
                    : Path.Combine(path, filename + Path.GetExtension(new Uri(screenshot.ImageUrl).AbsolutePath));

                // If there's no reason to perform the call then don't
                if (!overwrite && File.Exists(fullPath))
                {
                    return;
                }
                else
                {
                    // This calls does not require authentication and they actually FAIL if you add it
                    var imageResponse = await httpClient.GetAsync(screenshot.ImageUrl);
                    imageResponse.EnsureSuccessStatusCode();

                    await this.ReadAsFileAsync(imageResponse.Content, fullPath, true);
                }
            }
        }

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
        public async Task SaveThumbnailToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false)
        {
            Contract.Requires(screenshot != null);
            Contract.Requires(path != null);

            using (var httpClient = new HttpClient())
            {
                var fullPath = string.IsNullOrEmpty(filename)
                    ? Path.Combine(path, Path.GetFileName(new Uri(screenshot.ThumbnailUrl).AbsolutePath))
                    : Path.Combine(path, filename + Path.GetExtension(new Uri(screenshot.ThumbnailUrl).AbsolutePath));
                
                // If there's no reason to perform the call then don't
                if (!overwrite && File.Exists(fullPath))
                {
                    return;
                }
                else
                {
                    var imageResponse = await httpClient.GetAsync(screenshot.ThumbnailUrl);
                    imageResponse.EnsureSuccessStatusCode();

                    await this.ReadAsFileAsync(imageResponse.Content, fullPath, true);
                }
            }
        }

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
        public async Task<Job> StartJobAsync(string url, Job.JobInfo jobInfo, bool usingTunnel = false, params Browser[] browsers)
        {
            var jobRequest = new ScreenshotJobRequest()
            {
                url = url, 
                callback_url = jobInfo.CallbackUrl, 
                orientation = jobInfo.Orientation.HasValue ? jobInfo.Orientation.ToString().ToLower() : null, 
                quality = jobInfo.Quality.HasValue ? jobInfo.Quality.ToString().ToLower() : null, 
                wait_time = jobInfo.WaitTime, 
                mac_res = jobInfo.OsxResolution.HasValue ? jobInfo.OsxResolution.ToString().ToLower().Replace("r_", string.Empty) : null, 
                win_res = jobInfo.WinResolution.HasValue ? jobInfo.WinResolution.ToString().ToLower().Replace("r_", string.Empty) : null, 
                tunnel = usingTunnel, 
                browsers = browsers.Select(x => new BrowserInfo() { browser = x.BrowserName, browser_version = x.BrowserVersion, device = x.Device, os = x.OS, os_version = x.OSVersion, }).ToArray()
            };

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, screenshotsRestAPIBaseUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(jobRequest), Encoding.Default, "application/json")
                };
                if (this.AuthenticateForStartJob)
                {
                    request.Headers.Authorization = this.GetAuthenticationHeader();
                }

                var response = await httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(
                        string.Format("Error while starting the job.\nResponse status is {0} ({1}).\nResponse is: {2}", response.ReasonPhrase, response.StatusCode, responseString));
                }

                var screenshotJob = await JsonConvert.DeserializeObjectAsync<ScreenshotJob>(responseString);

                return MapToJob(screenshotJob);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The map info to browser.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        /// The <see cref="Browser" />.
        /// </returns>
        private static Browser MapInfoToBrowser(BrowserInfo x)
        {
            return new Browser() { BrowserName = x.browser, BrowserVersion = x.browser_version, Device = x.device, OS = x.os, OSVersion = x.os_version };
        }

        /// <summary>
        /// The map to job.
        /// </summary>
        /// <param name="screenshotJob">
        /// The screenshot job.
        /// </param>
        /// <returns>
        /// The <see cref="Job"/>.
        /// </returns>
        private static Job MapToJob(ScreenshotJob screenshotJob)
        {
            return new Job()
            {
                Id = string.IsNullOrEmpty(screenshotJob.job_id) ? screenshotJob.id : screenshotJob.job_id, 
                State = !string.IsNullOrEmpty(screenshotJob.state) ? (Job.States)Enum.Parse(typeof(Job.States), screenshotJob.state.Replace("-", string.Empty), true) : Job.States.Pending, 
                Info =
                    new Job.JobInfo()
                    {
                        CallbackUrl = screenshotJob.callback_url, 
                        Orientation = !string.IsNullOrEmpty(screenshotJob.orientation) ? (Job.Orientations?)Enum.Parse(typeof(Job.Orientations), screenshotJob.orientation, true) : null, 
                        Quality = !string.IsNullOrEmpty(screenshotJob.quality) ? (Job.Qualities?)Enum.Parse(typeof(Job.Qualities), screenshotJob.quality, true) : null, 
                        WaitTime = screenshotJob.wait_time, 
                        OsxResolution = !string.IsNullOrEmpty(screenshotJob.mac_res) ? (Job.OSXResolutions?)Enum.Parse(typeof(Job.OSXResolutions), "r_" + screenshotJob.mac_res, true) : null, 
                        WinResolution = !string.IsNullOrEmpty(screenshotJob.win_res) ? (Job.WinResolutions?)Enum.Parse(typeof(Job.WinResolutions), "r_" + screenshotJob.win_res, true) : null, 
                    }, 
                Screenshots =
                    screenshotJob.Screenshots.Select(
                        x =>
                            new API.Screenshots.Screenshot()
                            {
                                Id = x.id, 
                                State = !string.IsNullOrEmpty(x.state) ? (Screenshot.States)Enum.Parse(typeof(Screenshot.States), x.state.Replace("-", string.Empty), true) : Screenshot.States.Pending, 
                                Url = x.url, 
                                Browser = MapInfoToBrowser(x), 
                                ThumbnailUrl = x.thumb_url, 
                                ImageUrl = x.image_url, 
                                CreatedAt = !string.IsNullOrEmpty(x.created_at) ? (DateTimeOffset?)new DateTimeOffset(DateTime.Parse(x.created_at.Replace(" UTC", string.Empty))) : null, 
                            }), 
            };
        }

        /// <summary>
        /// The get authentication header.
        /// </summary>
        /// <returns>
        /// The <see cref="AuthenticationHeaderValue"/>.
        /// </returns>
        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Username + ":" + this.Password)));
        }

        /// <summary>
        /// The read as file async.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="overwrite">The overwrite.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        /// <remarks>Taken from <see href="http://blogs.msdn.com/b/henrikn/archive/2012/02/17/downloading-a-google-map-to-local-file.aspx" />.</remarks>
        private async Task ReadAsFileAsync(HttpContent content, string filename, bool overwrite)
        {
            var pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                await content.CopyToAsync(fileStream).ContinueWith((copyTask) => { fileStream.Close(); });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                throw;
            }
        }

        #endregion
    }
}