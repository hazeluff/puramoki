using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUtils {
    public static T GetJsonValue<T>(this JToken jToken, string key, T defaultValue = default) {
        dynamic value = jToken[key];
        if (value == null)
            return defaultValue;
        if (value is JObject)
            return JsonConvert.DeserializeObject<T>(value.ToString());
        return (T)value;
    }
}
