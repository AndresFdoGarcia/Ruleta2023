using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Business.Core.Util
{
    public class EnvironmentFinder
    {
        public static readonly string PRODUCTION = "Prod";       
        public static readonly string LOCAL = "Local";

        public static readonly string PRODUCTION_FILE = "appsettings.Prod.json";       
        public static readonly string LOCAL_FILE = "appsettings.Local.json";

        public static string GetEnvironmentName(string environment)
        {
            if (string.IsNullOrEmpty(environment)) return LOCAL;
            switch (environment.ToLower())
            {
                case "production":
                    return PRODUCTION;                
                case "local":
                    return LOCAL;
                default:
                    return LOCAL;
            }
        }

        public static string GetConfigurationFileName(string environment)
        {
            if (string.IsNullOrEmpty(environment)) return LOCAL_FILE;
            switch (environment.ToLower())
            {
                case "production":
                    return PRODUCTION_FILE;                
                case "local":
                    return LOCAL_FILE;
                default:
                    return LOCAL_FILE;
            }
        }
    }
}
