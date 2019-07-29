namespace Maze_Game
{
    public class Room
    {
        // There are 4 passages on each room
        public Passage[] passages = new Passage[4];

        public void GeneratePassages()
        {
            for (int i = 0; i < passages.Length; i++)
            {
                passages[i] = new Passage(false, (PassageDirections)i);
            }
        }
    }
}
