using EW.Navigator.SCM.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace EW.Navigator.SCM.Services
{
    public class HttpRequestConfiguration : IHttpRequestConfiguration
    {
        public string Url { get; set; }
        public string StartSha { get; set; }
        public string RefName { get; set; }
        public int Limit { get; set; }
        public string Sha { get; set; }
    }
}
