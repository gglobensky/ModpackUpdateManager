using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

        public async static void SerializeJsonFile<T>(string path, T instance, bool append)
        {
            await Utilities.FileWriteAsync(path, JsonConvert.SerializeObject(instance, Formatting.Indented), append);
        }

        public static async Task CreateFile(string path)
        {
            await Utilities.FileWriteAsync(path, "", false);
        }
    }
}
