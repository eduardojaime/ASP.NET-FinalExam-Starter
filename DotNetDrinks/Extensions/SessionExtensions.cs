using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Import these two namespaces
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DotNetDrinks.Extensions
{
    // Static classes don't need to be instantiated
    // Memory ??? when is it accessible? before the application starts up
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
