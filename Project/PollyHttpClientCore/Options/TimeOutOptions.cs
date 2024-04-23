using System;

namespace PollyHttpClientCore.Options
{
    public class TimeOutOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}