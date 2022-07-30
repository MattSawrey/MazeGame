using Maze.Game.Entities;
using System;
using Xunit;

namespace Maze.Game.Tests.Unit_Tests
{
    public class MazeTests
    {
        [Fact]
        public void NumberOfRoomsIsCorrect()
        {
            Entities.Maze maze = new Entities.Maze();
            MazeConfiguration mazeConfiguration = new MazeConfiguration();
            Random random = new Random();
            mazeConfiguration.NumberOfRooms = 4;
            maze.GenerateRooms(mazeConfiguration, random);

            Assert.Equal(mazeConfiguration.NumberOfRooms, maze.Rooms.Count);
        }

        [Fact]
        public void AddingPlayerTreasureValueIsCorrect()
        {
            Player player = new Player();
            player.AddTreasure(100);

            Assert.Equal(100, player.CollectedTreasure);
        }

    }
}
