namespace Maze_Game.Entities.Items
{
    public class Player : Item
    {
        public string Name { get; set; }

        public int NumberOfMovesMade { get; set; }

        public int CollectedTreasure { get; set; }
    }
}
