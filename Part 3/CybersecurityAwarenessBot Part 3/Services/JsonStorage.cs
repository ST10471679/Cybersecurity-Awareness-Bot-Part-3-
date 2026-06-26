using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CybersecurityAwarenessBotPart3.Services
{
    public static class JsonStorage
    {
        public static void Save<T>(string path, List<T> data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        public static List<T> Load<T>(string path)
        {
            if (!File.Exists(path))
                return new List<T>();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}