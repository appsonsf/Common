using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleKeyValueService
{
    [DataContract]
    [Serializable]
    public struct DataItem
    {
        public DataItem(string value)
        {
            Value = value;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public string Value;
        public DateTimeOffset Timestamp;
    }
}
