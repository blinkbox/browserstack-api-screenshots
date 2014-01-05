namespace BrowserStack.API.Screenshots.Configuration
{
    using System;
    using System.Configuration;

    public class BatchElement: ConfigurationElement
    {
        [ConfigurationProperty("sessionLimit", DefaultValue = 4, IsRequired = false)]
        public int SessionLimit
        {
            get { return Convert.ToInt32(this["sessionLimit"]); }
            set { this["sessionLimit"] = value; }
        }

        [ConfigurationProperty("captureThumbnails", DefaultValue = false, IsRequired = false)]
        public bool CaptureThumbnails
        {
            get { return Convert.ToBoolean(this["captureThumbnails"]); }
            set { this["captureThumbnails"] = value; }
        }
    }
}