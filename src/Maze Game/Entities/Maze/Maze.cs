using System;
using System.Collections.Generic;

namespace Maze_Game
{
    public partial class Maze
    {
        public List<Room> Rooms { get; set; }

        public Maze()
        {
            Rooms = new List<Room>();
        }

        public void GenerateRooms(MazeConfiguration configuration)
        {
            Random rand = new Random();

            for (int i = 1; i <= configuration.NumberOfRooms; i++)
            {
                Room room = new Room();
                int numPassagesFromRoom = rand.Next(1, 5); // MaxValue is exclusive, so needs to be one greater than the actual number of passages allowed on a room.
                room.GeneratePassages(numPassagesFromRoom, i == configuration.NumberOfRooms);
                room.GenerateItems();
                room.GenerateEnemies();
                Rooms.Add(room);
            }
        }
    }
}
