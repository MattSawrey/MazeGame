using Newtonsoft.Json;
using System;
using System.IO;

namespace Maze_Game_Common.SavingLoading
{
    public static class Deserialize
    {
        public static string LoadTextFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new Exception("Attempting to read file that doesn't exist.");
            }
            catch
            {
                Console.WriteLine("configuration file does not exist. Please ensure file exists.");
                return "";
            }

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static T DeserializeFromJson<T>(string filePath) where T : class
        {
            string json = LoadTextFromFile(filePath);

            if (json == "")
                return null;

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                throw new Exception("Failed to deserialize json string. Please check the format of the string");
            }
        }
    }
}
