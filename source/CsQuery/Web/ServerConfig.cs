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
        /// Configure the "default default" settings
        /// </summary>
         static ServerConfig()
        {
            _Default = new ServerConfig
            {
                Timeout = TimeSpan.FromSeconds(10),
                UserAgent = null
            };
        }

        private static ServerConfig _Default;

        /// <summary>
        /// The default settings used when making remote requests
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
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// Apply these options to a web request
        /// </summary>
        /// <param name="request"></param>
        public static void Apply(ServerConfig options, CsqWebRequest request)
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



        public string UserAgent { get; set; }
        public TimeSpan Timeout { get; set; }
        
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
