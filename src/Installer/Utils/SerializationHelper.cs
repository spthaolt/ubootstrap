using System.Collections;
using Newtonsoft.Json;

namespace Bootstrap.Installer.Utils
{
    public static class SerializationHelper
    {
        public static T JsonToDictionary<T>(this string json) where T : IDictionary
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string DictionaryToJson(this IDictionary dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }
    }
}

