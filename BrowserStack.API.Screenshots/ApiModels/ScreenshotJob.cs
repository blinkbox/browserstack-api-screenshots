namespace BrowserStack.API.Screenshots.ApiModels
{
    internal class ScreenshotJob
    {
        #region Public Properties

        public ScreenshotInfo[] Screenshots { get; set; }
        public string callback_url { get; set; }

        public string id { get; set; }
        public string job_id { get; set; }

        public string mac_res { get; set; }
        public string orientation { get; set; }
        public string quality { get; set; }
        public string state { get; set; }
        public int wait_time { get; set; }
        public string win_res { get; set; }

        #endregion
    }
}