namespace BrowserStack.API.Screenshots.Configuration
{
    using System;
    using System.Configuration;

    public class BatchElement: ConfigurationElement
    {
        [ConfigurationProperty("session-limit", DefaultValue = 4, IsRequired = false)]
        public int SessionLimit
        {
            get { return Convert.ToInt32(this["session-limit"]); }
            set { this["session-limit"] = value; }
        }

        [ConfigurationProperty("capture-thumbnails", DefaultValue = false, IsRequired = false)]
        public bool CaptureThumbnails
        {
            get { return Convert.ToBoolean(this["capture-thumbnails"]); }
            set { this["capture-thumbnails"] = value; }
        }
    }
}