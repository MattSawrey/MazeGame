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

        public void GenerateRooms(MazeConfiguration configuration, Random rand)
        {
            for (int i = 1; i <= configuration.NumberOfRooms; i++)
            {
                Room room = new Room();
                // minimum of 1 passage from starting room and ending room. Min of 2 from all others
                int numPassagesFromRoom = rand.Next(2, 5); // MaxValue is exclusive, so needs to be one greater than the actual number of passages allowed on a room.
                room.GeneratePassages(numPassagesFromRoom, i == configuration.NumberOfRooms, rand);
                room.GenerateItems();
                room.GenerateEnemies();
                Rooms.Add(room);
            }
        }

        public void ConnectRooms(Random rand)
        {
            // Perform an initial pass and make sure that each room connects to the next one (apart from the last room), so that there is a possible route through the game.
            for (int r = 0; r < Rooms.Count; r++)
            {
                // if we're not dealing with the exit passage
                if (r < Rooms.Count - 1) // Not at the final room
                {
                    Rooms[r].passages[0].passageTo = Rooms[r + 1];
                }
            }

            // Then go through and randomise the connection of each other passage, but within a range. Passages can only lead to rooms that are 2 behind or two in-front of themselves in the rooms collection.
            for (int r = 0; r < Rooms.Count; r++)
            {
                for (int p = 1; p < Rooms[r].passages.Length; p++)
                {
                    // if we're not dealing with the exit passage
                    if (Rooms[r].passages[p].isExit)
                    {
                        Rooms[r].passages[p].passageTo = null;
                    }
                    else
                    {
                        int randomConnectingRoomIndex = rand.Next(Math.Clamp(r - 2, 0, Rooms.Count), Math.Clamp(r + 2, 0, Rooms.Count));
                        Rooms[r].passages[p].passageTo = Rooms[randomConnectingRoomIndex];
                    }
                }
            }
        }
    }
}
