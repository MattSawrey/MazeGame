using System;

namespace Maze_Game_Common.Randomisation
{
    // Singleton Random class that can be accessed from anywhere
    public class RandomProvider
    {
        private static Random _Instance;
        public static Random Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Random();
                }
                return _Instance;
            }
        }
    }
}
