using System.IO;
using UnityEngine;

namespace CityAuthority.SaveLoad
{
    // Thin disk read/write wrapper around JsonUtility so ScenarioSaveService
    // itself stays testable without touching the filesystem.
    public static class SaveFileIO
    {
        public static void Save(string path, ScenarioSaveData data)
        {
            File.WriteAllText(path, JsonUtility.ToJson(data, prettyPrint: true));
        }

        public static ScenarioSaveData Load(string path)
        {
            return JsonUtility.FromJson<ScenarioSaveData>(File.ReadAllText(path));
        }

        public static bool Exists(string path) => File.Exists(path);
    }
}
