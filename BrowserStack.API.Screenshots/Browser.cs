// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Browser.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   The browser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The browser.
    /// </summary>
    [Serializable]
    public class Browser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        public string BrowserName { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        public string BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the os.
        /// </summary>
        public string OS { get; set; }

        /// <summary>
        /// Gets or sets the os version.
        /// </summary>
        public string OSVersion { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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