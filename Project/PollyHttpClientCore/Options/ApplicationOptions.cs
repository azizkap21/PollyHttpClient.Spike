using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace PollyHttpClientCore.Options
{
    public class ApplicationOptions 
    {
        public PolicyOptions Policies { get; set; }

        public HttpClientOptions  HttpClientOptions { get; set; }
    }
}
