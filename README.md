# browserstack-api-screenshots

This is a .NET client to BrowserStack's Screenshots REST API. You can see more about the API documentation at http://www.browserstack.com/screenshots/api.

This class library includes two different components. A low level REST client for wrapping calls to BrowserStack's Screenshots API and a higher level component (that uses the lower level one) that performs batch screenshot capture jobs and saves the results in a local folder.

## REST client usage

To use the library, first you need to have an account at BrowserStack. Starting a screenshot capture job through the REST API requires *at least* a Team Plus subscription to the Screenshots + Responsive product, though for the rest of the calls a simple subscription is sufficient.

You can start a screenshots job in BrowserStack like this:

	var screenshotsAPI = new ScreenshotsApi("username", "password");
	
	var browsers = await screenshotsAPI.GetBrowsersAsync();
	
	var job = await screenshotsAPI.StartJobAsync("http://www.google.com", new Job.JobInfo(), false, browsers.First(x => x.BrowserName == "ie" && x.BrowserVersion == "9.0"));

After starting the job you can periodically check it's status like this:

	while(!job.IsComplete) 
	{
		Thread.Sleep(1000);
		job = await screenshotsAPI.GetJobInfoAsync(job.Id);
	}

Finally, you can decide to save a screenshot to a local folder like this:

	if (job.State == Job.States.Done)
	{
		var screenshot = job.Screenshots.FirstOrDefault(x => x.State == Screenshot.States.Done);
		if (screenshot != null) 
		{
			await screenshotsApi.SaveScreenshotToFileAsync(screenshot, "screenshot.png", "screenshot_thumbnail.png");
		}
	}

## Batch capture usage

If you wish to capture a large amount of screenshots for more than one urls then using the low level REST client can become tedious. This is where the BatchScreenshotsCapture class can be of help by passing into it a list of BatchCaptureJobInfo. First let's see how to configure two BatchCaptureJobInfo classes:

	var batchCaptureJobForWindows = new BatchCaptureJobInfo(
			url: "http://www.google.com", 
			filenameTemplate: "google",
			jobInfo: new Job.JobInfo() { WinResolution = Job.WinResolutions.R_1024x768 },
			browsers: browsers.Where(x => x.OS == "Windows").ToArray());
			
	var batchCaptureJobForDevices = new BatchCaptureJobInfo(
			url: "http://www.bing.com", 
			filenameTemplate: "bing",
			jobInfo: new Job.JobInfo() { Orientation = Job.Orientations.Portrait }, 
			browsers: browsers.Where(x => !string.IsNullOrEmpty(x.Device)).ToArray());

Starting the batch capture job is easy:

	var capturer = new BatchScreenshotsCapture(sessionLimit: 1, captureThumbnails: false, "username", "password");
	
	await ExecuteBatchAsync("C:\Screenshots\" + DateTime.Now.ToString(), false, batchCaptureJobForWindows, batchCaptureJobForDevices);

Running the above command will start as many BrowserStack screenshot jobs necessary to capture the all screenshots for google on windows platforms and all screenshots for bing on all devices. After the method executes the screenshots will be saved in the local folder specified following a structure like so:

* Windows
	* XP
		* chrome
			* 31.0
				* 1024x768
					* google.png
			* ...
		* ie
			* 7.0 
				* ...
		* ...
	* 7
		* chrome
			* ...
		* ...
* android
	* 2.2
		* LG_Optimus_3D
			* Portrait
				* bing.png
		* ...
	* 4.1
		* Google_Nexus_7
			* ...
		*...
	* ...
* ios
	* ...