namespace BrowserStack.API.Screenshots.ApiModels
{
    internal class ScreenshotInfo : BrowserInfo
    {
        #region Public Properties

        public string created_at { get; set; }
        public string id { get; set; }
        public string image_url { get; set; }
        public string state { get; set; }

        public string thumb_url { get; set; }
        public string url { get; set; }

        #endregion
    }
}