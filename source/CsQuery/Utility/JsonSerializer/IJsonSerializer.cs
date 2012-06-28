using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        object Deserialize(string objectToDeserializer, Type type);
        T Deserialize<T>(string objectToDeserializer);
    }
}
