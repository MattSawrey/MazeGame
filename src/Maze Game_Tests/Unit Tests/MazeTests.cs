using Maze_Game;
using Maze_Game.Entities.Items;
using System;
using Xunit;
using static Maze_Game.Maze;

namespace Maze_Game_Tests.Unit_Tests
{
    public class MazeTests
    {
        [Fact]
        public void Number_Of_Rooms_Is_Correct()
        {
            Maze maze = new Maze();
            MazeConfiguration mazeConfiguration = new MazeConfiguration();
            Random random = new Random();
            mazeConfiguration.NumberOfRooms = 4;
            maze.GenerateRooms(mazeConfiguration, random);

            Assert.Equal(mazeConfiguration.NumberOfRooms, maze.Rooms.Count);
        }

        [Fact]
        public void Adding_Player_Treasure_Value_Is_Correct()
        {
            Player player = new Player();
            player.AddTreasure(100);

            Assert.Equal(100, player.CollectedTreasure);
        }

    }
}
