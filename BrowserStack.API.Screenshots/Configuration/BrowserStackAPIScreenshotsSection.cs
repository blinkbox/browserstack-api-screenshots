// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserStackAPIScreenshotsSection.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   The BrowserStack api screenshots section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BrowserStack.API.Screenshots.Configuration
{
    #region Using Directives

    using System.Configuration;

    #endregion

    /// <summary>
    /// The BrowserStack api screenshots section.
    /// </summary>
    public class BrowserStackAPIScreenshotsSection : ConfigurationSection
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        [ConfigurationProperty("Authentication", IsRequired = false)]
        public AuthenticationElement Authentication
        {
            get
            {
                return base["Authentication"] as AuthenticationElement;
            }

            set
            {
                this["Authentication"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the batch.
        /// </summary>
        [ConfigurationProperty("Batch", IsRequired = false)]
        public BatchElement Batch
        {
            get
            {
                return base["Batch"] as BatchElement;
            }

            set
            {
                this["Batch"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tunnels.
        /// </summary>
        [ConfigurationProperty("Tunnels", IsRequired = false)]
        [ConfigurationCollection(typeof(TunnelsCollection), AddItemName = "Template")]
        public TunnelsCollection Tunnels
        {
            get
            {
                return base["Tunnels"] as TunnelsCollection;
            }

            set
            {
                this["Tunnels"] = value;
            }
        }

        #endregion
    }
}