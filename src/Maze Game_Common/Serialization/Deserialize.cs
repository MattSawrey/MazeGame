using Maze_Game_Common.CommonConsole;
using Newtonsoft.Json;
using System.IO;

namespace Maze_Game_Common.SavingLoading
{
    public static class Deserialize
    {
        public static string LoadTextFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                CommonConsoleHelpers.WriteOutputAsDelayedCharArray("ERROR: configuration file does not exist. Please ensure file exists.", 10, true);
                return "";
            }
            try
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch
            {
                CommonConsoleHelpers.WriteOutputAsDelayedCharArray("ERROR: configuration file could not be read. Please ensure that the format of the configuration file is correct.", 10, true);
                return "";
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
                CommonConsoleHelpers.WriteOutputAsDelayedCharArray("ERROR: Failed to deserialize json string. Please ensure that the format of the configuration file is correct.", 10, true);
                return null;
            }
        }
    }
}
