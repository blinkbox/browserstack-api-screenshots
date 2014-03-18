using System;
namespace BrowserStack.API.Screenshots
{
    /// <summary>
    /// Provides an interface for capturing screenshots for multiple URLs and in multiple browsers in one batch call.
    /// </summary>
    public interface IBatchScreenshotsCapture
    {
        System.Threading.Tasks.Task ExecuteBatchAsync(string rootPath, bool usingTunnel, params BatchCaptureJobInfo[] batchCaptureJobs);
        event BatchScreenshotsCapture.JobEvent JobCompleted;
        event BatchScreenshotsCapture.JobFailureEvent JobFailedToStart;
        System.Collections.ObjectModel.ObservableCollection<Job> Jobs { get; }
        event BatchScreenshotsCapture.JobEvent JobStarted;
        event BatchScreenshotsCapture.JobEvent JobStateChanged;
        System.Collections.ObjectModel.ObservableCollection<Screenshot> ScreenhotsCompleted { get; }
        event BatchScreenshotsCapture.ScreenshotEvent ScreenshotCompleted;
        event BatchScreenshotsCapture.ScreenshotFailedEvent ScreenshotFailed;
    }
}
