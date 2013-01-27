using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{
    /// <summary>
    /// Configuration opttions used when accessing remote resources
    /// </summary>
    public class ServerConfig
    {
         /// <summary>
         /// Configure the "default default" settings.
         /// </summary>

        static ServerConfig()
        {
            _Default = new ServerConfig
            {
                Timeout = TimeSpan.FromSeconds(10),
                UserAgent = "Mozilla/5.0 (compatible; CsQuery/1.3)"
            };
        }

        private static ServerConfig _Default;

        /// <summary>
        /// The default settings used when making remote requests.
        /// </summary>

        public static ServerConfig Default
        {
            get
            {
                return _Default;
            }
            
        }

        /// <summary>
        /// Merge any non-null options into a new options object.
        /// </summary>
        ///
        /// <param name="options">
        /// The options
        /// </param>
        ///
        /// <returns>
        /// A new ServerConfig object
        /// </returns>

        public static ServerConfig Merge(ServerConfig options) {
            ServerConfig config = ServerConfig.Default;
            if (options != null)
            {
                if (options.UserAgent != null)
                {
                    config.UserAgent = options.UserAgent;
                }
                if (options.Timeout != null)
                {
                    config.Timeout = options.Timeout;
                }
            }
            return config;
        }

        /// <summary>
        /// Apply these options to a web request.
        /// </summary>
        ///
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="request">
        /// The CsqWebRequest object to apply the options to.
        /// </param>

        public static void Apply(ServerConfig options, ICsqWebRequest request)
        {
            var opts = Merge(options);
            if (opts.Timeout != null)
            {
                request.Timeout = (int)Math.Floor(opts.Timeout.TotalMilliseconds);
            }
            if (opts.UserAgent != null)
            {
                request.UserAgent = opts.UserAgent;
            }
        }

        /// <summary>
        /// Gets or sets the user agent string that will be used to identify this service to the server
        /// </summary>

        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the timeout after which an incomplete request will be aborted
        /// </summary>

        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout (in seconds)  after which an incomplete request will be aborted
        /// </summary>

        public double TimeoutSeconds
        {

            get
            {
                return Timeout==null ? 0 :
                    Timeout.TotalSeconds;
            }
            set
            {
                Timeout = TimeSpan.FromSeconds(value);
            }
        }
    }
}
