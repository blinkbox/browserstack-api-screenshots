// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserStackTunnel.cs" company="blinkbox Entertainment Ltd">
//   Copyright © 2014 blinkbox Entertainment Ltd
// </copyright>
// <summary>
//   The browser stack tunnel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrowserStack.API.Screenshots
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using BrowserStack.API.Screenshots.Configuration;

    #endregion

    /// <summary>
    /// The browser stack tunnel.
    /// </summary>
    public class BrowserStackTunnel : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The browser stack key.
        /// </summary>
        private readonly string browserStackKey;

        /// <summary>
        /// The browser stack tunnel jar file.
        /// </summary>
        private readonly string browserStackTunnelJarFile;

        /// <summary>
        /// The java compiler path.
        /// </summary>
        private readonly string javaCompilerPath;

        /// <summary>
        /// The java tunnel.
        /// </summary>
        private Process javaTunnel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackTunnel"/> class.
        /// </summary>
        /// <remarks>Use this constructor when you want to configure the tunnel through the application configuration file.</remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// The path to the tunnel jar file has not been set up in the configuration file
        /// or
        /// The BrowserStack key has not been defined in the authentication section in the configuration file
        /// or
        /// The jar file could not be found
        /// </exception>
        public BrowserStackTunnel()
        {
            var config = ConfigurationSectionManager.Configuration;
            
            this.browserStackKey = config.Authentication.Password;
            this.browserStackTunnelJarFile = config.Tunnels.TunnelJarFullPath;
            this.javaCompilerPath = config.Tunnels.JavaPath;

            if (string.IsNullOrEmpty(this.browserStackTunnelJarFile))
            {
                throw new ConfigurationErrorsException("You need to specify the jar file in the configuration file.");
            }

            if (string.IsNullOrEmpty(this.browserStackKey))
            {
                throw new ConfigurationErrorsException("To run the tunnel you need to have set up the BrowserStack key in the authentication section.");
            }

            if (!File.Exists(this.browserStackTunnelJarFile))
            {
                throw new ConfigurationErrorsException("Could not find tunnel jar file at " + this.browserStackTunnelJarFile);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackTunnel" /> class.
        /// </summary>
        /// <param name="javaCompilerPath">The java compiler path.</param>
        /// <param name="browserStackTunnelJarFile">The browser stack tunnel jar file.</param>
        /// <param name="browserStackKey">The browser stack key.</param>
        /// <exception cref="System.ArgumentNullException">
        /// browserStackTunnelJarFile
        /// or
        /// browserStackKey
        /// </exception>
        /// <exception cref="System.ArgumentException">Could not find tunnel jar file at  + browserStackTunnelJarFile;browserStackTunnelJarFile</exception>
        /// <exception cref="ArgumentNullException">Thrown when any of the <paramref name="browserStackTunnelJarFile" /> or <paramref name="browserStackKey" /> are null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="browserStackTunnelJarFile" /> does not exist.</exception>
        public BrowserStackTunnel(string javaCompilerPath, string browserStackTunnelJarFile, string browserStackKey)
        {
            if (string.IsNullOrEmpty(browserStackTunnelJarFile))
            {
                throw new ArgumentNullException("browserStackTunnelJarFile");
            }

            if (string.IsNullOrEmpty(browserStackKey))
            {
                throw new ArgumentNullException("browserStackKey");
            }

            if (!File.Exists(browserStackTunnelJarFile))
            {
                throw new ArgumentException("Could not find tunnel jar file at " + browserStackTunnelJarFile, "browserStackTunnelJarFile");
            }

            this.browserStackKey = browserStackKey;
            this.browserStackTunnelJarFile = browserStackTunnelJarFile;
            this.javaCompilerPath = javaCompilerPath;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Starts the tunnel.
        /// </summary>
        /// <param name="hosts">The hosts for which the tunnel will be set up.</param>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="hosts"/> is null or does not contain any items.</exception>
        /// <exception cref="System.ApplicationException">Thrown when an error occurrs during the initialization of the tunnel.</exception>
        public void Start(IEnumerable<TunnelUrlConfig> hosts)
        {
            if (hosts == null || !hosts.Any())
            {
                throw new ArgumentException("Please provide at least one host to set up the tunnel.", "hosts");
            }

            this.StartTunnel(hosts);
        }

        /// <summary>
        /// Starts the tunnel.
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Please provide at least one host to set up the tunnel.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurrs during the initialization of the tunnel.</exception>
        /// <remarks>
        /// This method uses the configuration file to read the hosts configuration.
        /// </remarks>
        public void Start()
        {
            var config = ConfigurationSectionManager.Configuration;

            if (config.Tunnels == null || config.Tunnels.Count == 0)
            {
                throw new ConfigurationErrorsException("Please provide at least one host to set up the tunnel.");
            }

            this.StartTunnel(config.Tunnels.Cast<HostElement>().Select(x => new TunnelUrlConfig() { Host = x.Name, Port = x.Port, IsSecure = x.IsSecure} ));
        }

        /// <summary>
        /// Stops the tunnel.
        /// </summary>
        public void Stop()
        {
            if (this.javaTunnel != null)
            {
                this.javaTunnel.Kill();
                this.javaTunnel.Dispose();
                this.javaTunnel = null;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Starts the tunnel.
        /// </summary>
        /// <param name="hosts">The hosts.</param>
        private void StartTunnel(IEnumerable<TunnelUrlConfig> hosts)
        {
            this.javaTunnel = new Process();
            this.javaTunnel.StartInfo.FileName = Path.Combine(this.javaCompilerPath, "java.exe");
            this.javaTunnel.StartInfo.Arguments = string.Format("-jar \"{0}\" {1}", this.browserStackTunnelJarFile, this.browserStackKey);
            foreach (var tunnelUrlConfig in hosts)
            {
                this.javaTunnel.StartInfo.Arguments += string.Format(" {0},{1},{2}", tunnelUrlConfig.Host, tunnelUrlConfig.Port, tunnelUrlConfig.IsSecure ? "1" : "0");
            }

            this.javaTunnel.StartInfo.UseShellExecute = false;
            this.javaTunnel.StartInfo.RedirectStandardOutput = true;
            this.javaTunnel.StartInfo.RedirectStandardError = true;
            this.javaTunnel.Start();

            // This is quite brittle, but I don't know how else to wait so as to know if the tunnel has been set up
            // Maybe through the BrowserStack API?
            var stdOutput = string.Empty;
            while (!this.javaTunnel.StandardOutput.EndOfStream)
            {
                var output = this.javaTunnel.StandardOutput.ReadLine() ?? string.Empty;
                stdOutput += output;
                if (output.Contains("Ctrl-C") && output.EndsWith("exit"))
                {
                    break;
                }
            }

            if (this.javaTunnel.HasExited)
            {
                var stdErrorOutput = this.javaTunnel.StandardError.ReadToEnd();
                throw new ApplicationException(
                    string.Format(
                        "Could not setup tunnel.\nCommand executed: {0} {1}\nJava Tunnel Output:{2}\nJava Tunnel Error Output:{3}",
                        this.javaTunnel.StartInfo.FileName,
                        this.javaTunnel.StartInfo.Arguments,
                        stdOutput,
                        stdErrorOutput));
            }
        }


        #endregion

        /// <summary>
        /// The tunnel url config.
        /// </summary>
        public class TunnelUrlConfig
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the host.
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is secure.
            /// </summary>
            public bool IsSecure { get; set; }

            /// <summary>
            /// Gets or sets the port.
            /// </summary>
            public int Port { get; set; }

            #endregion
        }
    }
}