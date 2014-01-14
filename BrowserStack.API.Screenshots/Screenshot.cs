// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Screenshot.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   Represents a screenshot taken through BrowserStack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// Represents a screenshot taken through BrowserStack.
    /// </summary>
    [Serializable]
    public class Screenshot
    {
        #region Enums

        /// <summary>
        /// The possible states of a screenshot.
        /// </summary>
        /// <remarks>This list is neither exhaustive, neither 100% correct, inconsistencies can be found when calling the API.</remarks>
        public enum States
        {
            /// <summary>
            /// The screenshot has not yet been started.
            /// </summary>
            Pending = 0, 

            /// <summary>
            /// The screenshot generation failed because of a timeout.
            /// </summary>
            TimedOut, 

            /// <summary>
            /// The screenshot was successfully taken.
            /// </summary>
            Done, 

            /// <summary>
            /// The screenshot is currently being processed.
            /// </summary>
            Processing, 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the browser information that was used to take the screenshot.
        /// </summary>
        public Browser Browser { get; internal set; }

        /// <summary>
        /// Gets the date and time the screenshot was created.
        /// </summary>
        public DateTimeOffset? CreatedAt { get; internal set; }

        /// <summary>
        /// Gets a unique identifier of the screenshot.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the screenshot image URL in Browserstack.
        /// </summary>
        public string ImageUrl { get; internal set; }

        /// <summary>
        /// Gets the current state of the screenshot.
        /// </summary>
        public States State { get; internal set; }

        /// <summary>
        /// Gets the screenshot thumbnail URL in Browserstack.
        /// </summary>
        public string ThumbnailUrl { get; internal set; }

        /// <summary>
        /// Gets the original URL that was used to take the screenshot.
        /// </summary>
        public string Url { get; internal set; }

        #endregion
    }
}