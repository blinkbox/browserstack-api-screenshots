namespace BrowserStack.API.Screenshots.Configuration
{
    using System;
    using System.Configuration;

    public class AuthenticationElement : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get { return this["username"].ToString(); }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return this["password"].ToString(); }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("authenticateForGetBrowsers", DefaultValue = "false")]
        public bool AuthenticateForGetBrowsers
        {
            get { return Convert.ToBoolean(this["authenticateForGetBrowsers"]); }
            set { this["authenticateForGetBrowsers"] = value; }
        }
            
        [ConfigurationProperty("authenticateForGetJobInfo", DefaultValue = "true")]
        public bool AuthenticateForGetJobInfo
        {
            get { return Convert.ToBoolean(this["authenticateForGetJobInfo"]); }
            set { this["authenticateForGetJobInfo"] = value; }
        }

        [ConfigurationProperty("authenticateForGetScreenshotImages", DefaultValue = "false")]
        public bool AuthenticateForGetScreenshotImages
        {
            get { return Convert.ToBoolean(this["authenticateForGetScreenshotImages"]); }
            set { this["authenticateForGetScreenshotImages"] = value; }
        }

        [ConfigurationProperty("authenticateForStartJob", DefaultValue = "true")]
        public bool AuthenticateForStartJob
        {
            get { return Convert.ToBoolean(this["authenticateForStartJob"]); }
            set { this["authenticateForStartJob"] = value; }
        }
    }
}