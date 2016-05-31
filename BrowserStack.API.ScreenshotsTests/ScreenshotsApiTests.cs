using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrowserStack.API.Screenshots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrowserStack.API.Screenshots.Configuration;
using System.Configuration;

namespace BrowserStack.API.Screenshots.Tests
{
    [TestClass()]
    public class ScreenshotsApiTests
    {
        [TestMethod()]
        public void HelloWorld()
        {
            Assert.IsTrue( true );
        }

        /// <remarks>MSTest and ReSharper responds with "Inconclusive: Test not run" on all async testmethods due to unknown factors</remarks>>
        [TestMethod()]
        public async void HelloWorldAsync()
        {
            bool doodle = await Task.Run(()=> true);
            Assert.IsTrue(doodle);
        }

        [TestMethod()]
        public void CustomConfigurationSectionIsPresent()
        {
            var section = (BrowserStackAPIScreenshotsSection)ConfigurationManager.GetSection("BrowserStackAPIScreenshotsSection");
            Assert.IsNotNull(section, "App.Config missing Configuration section BrowserStackAPIScreenshots");
            Assert.IsNotNull(section.Authentication, "App.Config missing Authentication Element in section BrowserStackAPIScreenshots");
            Assert.IsFalse(string.IsNullOrWhiteSpace(section.Authentication.Username), "Missing Username in App.Config BrowserStackAPIScreenshots Authentication");
            Assert.IsFalse(string.IsNullOrWhiteSpace(section.Authentication.Password), "Missing Password in App.Config BrowserStackAPIScreenshots Authentication");

        }

        [TestMethod()]
        public void RunJob()
        {
            var screenshotsAPI = new ScreenshotsApi(); // Config based Authentication
            //var screenshotsAPI = new ScreenshotsApi(Username, Password); // Alternative

            Assert.IsNotNull(screenshotsAPI);

            var browsers = screenshotsAPI.GetBrowsers();
            Assert.IsNotNull(browsers);

            var job = screenshotsAPI.StartJob("http://www.google.com", new Job.JobInfo() { Orientation = Job.Orientations.Landscape, Quality = Job.Qualities.Compressed, WaitTime = 10, WinResolution = Job.WinResolutions.R_1280x1024 }, false,
                    browsers.First(x => x.OS == "Windows" && x.OSVersion == "10" && x.BrowserName == "chrome" && x.BrowserVersion == "49.0"));

            while (!job.IsComplete)
            {
                Thread.Sleep(1000);
                
                job = screenshotsAPI.GetJobInfo(job.Id);
                Assert.IsNotNull(job);
            }

            Assert.IsTrue(job.IsComplete);
            Assert.IsTrue(job.State == Job.States.Done);

            if (job.State == Job.States.Done)
            {
                var screenshot = job.Screenshots.FirstOrDefault(x => x.State == Screenshot.States.Done);
                Assert.IsNotNull(screenshot);
                if (screenshot != null)
                {
                    if (!System.IO.Directory.Exists("Screenshots")) { System.IO.Directory.CreateDirectory("Screenshots"); }
                    screenshotsAPI.SaveScreenshotToFile(screenshot, "Screenshots", "screenshot", true);
                    screenshotsAPI.SaveThumbnailToFile(screenshot, "Screenshots", "screenshot-thumbnail", true);
                    Process.Start(System.IO.Path.GetFullPath("Screenshots/screenshot.png"));
                }
            }    
        }

        [TestMethod()]
        public async void RunJobAsync()
        {
            var screenshotsAPI = new ScreenshotsApi();
            var browsers = await screenshotsAPI.GetBrowsersAsync();

            var job = await screenshotsAPI.StartJobAsync("http://www.google.com", new Job.JobInfo(), false,
                    browsers.First(x => x.OS == "Windows" && x.OSVersion == "10" && x.BrowserName == "firefox" && x.BrowserVersion == "45.0"));

            while (!job.IsComplete)
            {
                Thread.Sleep(1000);
                job = screenshotsAPI.GetJobInfoAsync(job.Id).Result;
            }

            if (job.State == Job.States.Done)
            {
                var screenshot = job.Screenshots.FirstOrDefault(x => x.State == Screenshot.States.Done);
                Assert.IsNotNull(screenshot);
                if (screenshot != null)
                {
                    if (!System.IO.Directory.Exists("Screenshots")) { System.IO.Directory.CreateDirectory("Screenshots"); }
                    await screenshotsAPI.SaveScreenshotToFileAsync(screenshot, "Screenshots", "screenshot", true);
                    Process.Start(System.IO.Path.GetFullPath("Screenshots/screenshot.png"));   
                }
            }
        }

        [TestMethod()]
        public void RunBatch()
        {
            var screenshotsAPI = new ScreenshotsApi();

            var browsers = screenshotsAPI.GetBrowsers();
            Assert.IsNotNull(browsers);

            var batchCaptureJobForWindows = new BatchCaptureJobInfo(
                url: "http://www.google.com",
                filenameTemplate: "google",
                jobInfo: new Job.JobInfo() { WinResolution = Job.WinResolutions.R_1280x1024 },
                browsers: browsers.Where(x => x.OS == "Windows" && x.OSVersion == "10" && x.BrowserName == "firefox" && x.BrowserVersion == "45.0").ToArray());

            var batchCaptureJobForDevices = new BatchCaptureJobInfo(
                    url: "http://www.bing.com",
                    filenameTemplate: "bing",
                    jobInfo: new Job.JobInfo() { Orientation = Job.Orientations.Portrait },
                    browsers: browsers.Where(x => x.Device == "HTC One M8").ToArray());

            var capturer = new BatchScreenshotsCapture(1, false);

            if (System.IO.Directory.Exists("Screenshots")) { System.IO.Directory.Delete("Screenshots", true); }
            System.IO.Directory.CreateDirectory("Screenshots");

            capturer.ExecuteBatch("Screenshots-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture), false, batchCaptureJobForWindows, batchCaptureJobForDevices);
            Process.Start(System.IO.Path.GetFullPath("Screenshots"));
        }

        [TestMethod()]
        public async void RunBatchAsync()
        {
            var screenshotsAPI = new ScreenshotsApi();

            var browsers = await screenshotsAPI.GetBrowsersAsync();

            var batchCaptureJobForWindows = new BatchCaptureJobInfo(
                url: "http://www.google.com",
                filenameTemplate: "google",
                jobInfo: new Job.JobInfo() { WinResolution = Job.WinResolutions.R_1280x1024 },
                browsers: browsers.Where(x => x.OS == "Windows" && x.OSVersion == "10" && x.BrowserName == "firefox" && x.BrowserVersion == "45.0").ToArray());

            var batchCaptureJobForDevices = new BatchCaptureJobInfo(
                    url: "http://www.bing.com",
                    filenameTemplate: "bing",
                    jobInfo: new Job.JobInfo() { Orientation = Job.Orientations.Portrait },
                    browsers: browsers.Where(x => x.Device == "HTC One M8").ToArray());

            var capturer = new BatchScreenshotsCapture(1, false);

            if (System.IO.Directory.Exists("Screenshots")) { System.IO.Directory.Delete("Screenshots", true); }
            System.IO.Directory.CreateDirectory("Screenshots");

            await capturer.ExecuteBatchAsync("Screenshots-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture), false, batchCaptureJobForWindows, batchCaptureJobForDevices);
            Process.Start(System.IO.Path.GetFullPath("Screenshots"));
        }


    }
}