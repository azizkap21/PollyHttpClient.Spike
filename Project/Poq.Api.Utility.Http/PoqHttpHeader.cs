using System;

namespace Poq.Api.Utility.Http
{
    [Serializable]
    public class PoqHttpHeader
    {
        public PoqHttpHeader()
        {
        }

        public PoqHttpHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public string Value;
        public string RawValue;
    }
}
