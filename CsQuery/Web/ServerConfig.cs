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
        public ServerConfig()
        {
            Timeout = 10000;
            UserAgent = null;
        }
        public static ServerConfig Merge(ServerConfig options) {
            ServerConfig config = new ServerConfig();
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
        public string UserAgent { get; set; }
        public int? Timeout { get; set; }
    }
}
