using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace PBDASHBOARD.BackgroundTaks
{
    public class UrlHelper

    {
        public string HostUrl { get; set; }
        public string Path { get; set; }
        public string BaseUrl;
        public string AbsoluteUri { get; set; } // final Uri

        public UrlHelper()
        {
        }

        /// <summary>
        /// initialises the parameters and build the Uri.
        /// </summary>
        /// <param name="hostUrl"></param>
        /// <param name="path"></param>
        /// <param name="queryParameters"></param>
        public UrlHelper(string hostUrl, string path, Dictionary<string, string> queryParameters)
        {
            QueryParameters = queryParameters;
            HostUrl = hostUrl;
            Path = path;
            AbsoluteUri = AddQueryString(hostUrl + path, QueryParameters);
        }

        /// <summary>
        /// Append the given key to the uri
        /// </summary>
        /// <param name="uri"> the base uri</param>
        /// <param name="param">The name of Query key</param>
        /// <param name="value">value of the Query key</param>
        /// <returns>the result uri</returns>
        public string AddQueryString(string uri, string param, string value)
        {
            return QueryHelpers.AddQueryString(uri, param, value);
        }

        /// <summary>
        /// Append the elements of key,val pair in the dictionary to the uri
        /// </summary>
        /// <param name="uri">base uri (host + path)</param>
        /// <param name="parameterDictionary">dictionary with query key,value to append to the uri</param>
        /// <returns>the result Uri</returns>
        public string AddQueryString(string uri, IDictionary<string, string> parameterDictionary)
        {
            return QueryHelpers.AddQueryString(uri, parameterDictionary);
        }

        public string getBaseUrl()
        {
            return BaseUrl = HostUrl + Path;
        }

        /// <summary>
        /// Add Query parameters in a dictionary. Can be used to build url using AddQueryString() method.
        /// </summary>
        public IDictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
    }
}