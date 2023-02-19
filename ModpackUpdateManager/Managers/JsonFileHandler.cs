using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModpackUpdateManager.Managers
{
    public static class JsonFileHandler
    {
        public static T DeserializeJsonFile<T>(string path, Action onJsonInvalid = null)
        {
            T result = default;

            string json = Utilities.ReadFile(path);

            if (!Utilities.TryParseJson<T>(json, out result))
            {
                onJsonInvalid?.Invoke();
            }

            return result;
        }

        public async static void SerializeJsonFile<T>(string path, T instance, bool append = false)
        {
            await Utilities.FileWriteAsync(path, JsonConvert.SerializeObject(instance, Formatting.Indented), append);
        }

        public static void WriteJsonToFile<T>(string path, T instance, bool append)
        {
            List<T> instanceList = JsonFileHandler.DeserializeJsonFile<List<T>>(path);

            if (instanceList == null)
            {
                instanceList = new List<T>();
            }

            instanceList.Add(instance);
            JsonFileHandler.SerializeJsonFile<List<T>>(path, instanceList, append);
        }
    }
}
