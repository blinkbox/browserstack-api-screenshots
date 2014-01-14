namespace BrowserStack.API.Screenshots.ApiModels
{
    internal class BrowserInfo
    {
        #region Public Properties

        public string browser { get; set; }
        public string browser_version { get; set; }
        public string device { get; set; }
        public string os { get; set; }
        public string os_version { get; set; }

        #endregion
    }
}