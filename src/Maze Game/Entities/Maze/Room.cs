using Maze_Game_Common.Extension_Methods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze_Game
{
    public class Room
    {
        // There are between 1 and 4 passages on each room. This is a predefined, immutale amount, so the passages collection can be a plain array.
        public Passage[] passages;

        // Numbers of treasures and threats in each room are mutable over the course of the game, therefore the List object is more appropriate than an array.
        public List<Treasure> Treasures { get; set; }

        public List<Threat> Threats { get; set; }

        public Room()
        {
            Threats = new List<Threat>();
            Treasures = new List<Treasure>();
        }

        public void GeneratePassages(int numPassages, bool hasExitPassage, Random rand)
        {
            passages = new Passage[numPassages];

            // Randomise the directions, but each direction can only be used once.

            // TODO - randomise passage directions
            var directionsList = Enum.GetValues(typeof(PassageDirections)).Cast<PassageDirections>().ToList();
            directionsList.Shuffle(rand);

            for (int i = 0; i < passages.Length; i++)
            {
                passages[i] = new Passage(false, directionsList[i]);
            }

            if (hasExitPassage)
            {
                int exitPassage = new Random().Next(0, passages.Length);
                passages[exitPassage].isExit = true;
            }
        }

        public void GenerateItems()
        {

        }

        public void GenerateEnemies()
        {

        }
    }
}
