using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Emr_web.Common
{
    public static class SessionExtension
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        public static void SetObjectAsJsonLsit(this ISession session, string key, object[] value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static List<T> GetObjectFromJsonList<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(List<T>) : JsonConvert.DeserializeObject<List<T>>(value).ToList();
        }
    }
}
