namespace BrowserStack.API.Screenshots.ApiModels
{
    internal class ScreenshotJobRequest
    {
        #region Public Properties

        public BrowserInfo[] browsers { get; set; }
        public string callback_url { get; set; }
        public string mac_res { get; set; }
        public string orientation { get; set; }
        public string quality { get; set; }
        public bool tunnel { get; set; }
        public string url { get; set; }
        public int wait_time { get; set; }
        public string win_res { get; set; }

        #endregion
    }
}