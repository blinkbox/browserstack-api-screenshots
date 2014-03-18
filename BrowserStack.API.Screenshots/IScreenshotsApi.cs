using System;
namespace BrowserStack.API.Screenshots
{
    /// <summary>
    /// Provides an interface around a REST client for connecting to Browserstack's screenshots API. See http://www.browserstack.com/screenshots/api.
    /// </summary>
    public interface IScreenshotsApi
    {
        bool AuthenticateForGetBrowsers { get; set; }
        bool AuthenticateForGetJobInfo { get; set; }
        bool AuthenticateForGetScreenshotImages { get; set; }
        bool AuthenticateForStartJob { get; set; }
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Browser>> GetBrowsersAsync();
        System.Threading.Tasks.Task<Job> GetJobInfoAsync(string jobId);
        System.Threading.Tasks.Task SaveScreenshotToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false);
        System.Threading.Tasks.Task SaveThumbnailToFileAsync(Screenshot screenshot, string path, string filename = null, bool overwrite = false);
        System.Threading.Tasks.Task<Job> StartJobAsync(string url, Job.JobInfo jobInfo, bool usingTunnel = false, params Browser[] browsers);
    }
}
