using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PollyHttpClientCore
{
    public interface IHttpClientCore
    {       
            /// <summary>
            /// Send an HTTP request as an asynchronous operation.. 
            /// Returns the result as (awaitable Task of) <seealso cref="HttpResponseMessage"/>.
            /// </summary>
            /// <param name="request">The HTTP request message to send</param>
            /// <returns>An (awaitable Task of) <seealso cref="HttpResponseMessage"/>.</returns>
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        
    }
}

