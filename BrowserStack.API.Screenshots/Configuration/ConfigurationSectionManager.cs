namespace BrowserStack.API.Screenshots.Configuration
{
    using System;
    using System.Configuration;

    internal static class ConfigurationSectionManager
    {
        private static volatile BrowserStackAPIScreenshotsSection configuration;
        private static readonly object syncRoot = new Object();

        public static BrowserStackAPIScreenshotsSection Configuration
        {
            get
            {
                // Implementing multithreaded singleton
                if (configuration == null)
                {
                    lock (syncRoot)
                    {
                        if (configuration == null)
                        {
                            configuration = (BrowserStackAPIScreenshotsSection)ConfigurationManager.GetSection("BrowserStackAPIScreenshotsSection");
                        }
                    }
                }

                return configuration;
            }
        }
    }
}
