namespace BrowserStack.API.Screenshots
{
    public class Browser
    {
        #region Public Properties

        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string Device { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Device))
            {
                return string.Format("{0} v{1} on {2} running {3}", this.OS, this.OSVersion, this.Device, this.BrowserName);
            }
            else
            {
                return string.Format("{0} v{1} on {2} {3}", this.BrowserName, this.BrowserVersion, this.OS, this.OSVersion);
            }
        }

        #endregion
    }
}