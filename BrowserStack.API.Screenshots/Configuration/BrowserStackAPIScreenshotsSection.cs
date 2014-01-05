namespace BrowserStack.API.Screenshots.Configuration
{
    #region Using Directives

    using System.Configuration;

    #endregion

    /// <summary>
    /// The browser stack api screenshots section.
    /// </summary>
    public class BrowserStackAPIScreenshotsSection : ConfigurationSection
    {
        #region Public Properties

        /// <summary>
        /// Gets the authentication.
        /// </summary>
        [ConfigurationProperty("Authentication", IsRequired = false)]
        public AuthenticationElement Authentication
        {
            get
            {
                return base["Authentication"] as AuthenticationElement;
            }
        }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        [ConfigurationProperty("Batch", IsRequired = false)]
        public BatchElement Batch
        {
            get
            {
                return base["Batch"] as BatchElement;
            }
        }

        #endregion
    }
}