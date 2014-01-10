// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TunnelsCollection.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   The tunnels collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BrowserStack.API.Screenshots.Configuration
{
    #region Using Directives

    using System;
    using System.Configuration;

    #endregion

    /// <summary>
    /// The tunnels collection.
    /// </summary>
    public class TunnelsCollection : ConfigurationElementCollection
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the java path.
        /// </summary>
        [ConfigurationProperty("java-path", DefaultValue = "")]
        public string JavaPath
        {
            get
            {
                return this["java-path"].ToString();
            }

            set
            {
                this["java-path"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tunnel jar full path.
        /// </summary>
        [ConfigurationProperty("tunnel-jar-fullpath", DefaultValue = "")]
        public string TunnelJarFullPath
        {
            get
            {
                return this["tunnel-jar-fullpath"].ToString();
            }

            set
            {
                this["tunnel-jar-fullpath"] = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element name.
        /// </summary>
        protected override string ElementName
        {
            get
            {
                return "Tunnel";
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TunnelElement"/>.
        /// </returns>
        public TunnelElement this[int index]
        {
            get
            {
                return (TunnelElement)this.BaseGet(index);
            }

            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create new element.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TunnelElement();
        }

        /// <summary>
        /// The get element key.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TunnelElement)element).Host;
        }

        #endregion
    }

    /// <summary>
    /// The tunnel element.
    /// </summary>
    public class TunnelElement : ConfigurationElement
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return this["host"].ToString();
            }

            set
            {
                this["host"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is secure.
        /// </summary>
        [ConfigurationProperty("is-secure", IsRequired = false, DefaultValue = false)]
        public bool IsSecure
        {
            get
            {
                return Convert.ToBoolean(this["is-secure"]);
            }

            set
            {
                this["is-secure"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 80)]
        public int Port
        {
            get
            {
                return Convert.ToInt32(this["port"]);
            }

            set
            {
                this["port"] = value;
            }
        }

        #endregion
    }
}