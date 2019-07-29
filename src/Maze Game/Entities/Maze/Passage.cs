namespace Maze_Game
{
    public class Passage
    {
        public bool isExit { get; set; }
        public PassageDirections passageDirection { get; set; }

        public Passage(bool isExit, PassageDirections passageDirection)
        {
            this.isExit = isExit;
            this.passageDirection = passageDirection;
        }
    }
}
