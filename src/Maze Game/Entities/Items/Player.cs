using System;

namespace Maze_Game.Entities.Items
{
    public class Player : Item
    {
        public int NumberOfMovesMade { get; set; }

        public int CollectedTreasure { get; private set; }

        public void ResetTreasureAndMovesMade()
        {
            NumberOfMovesMade = 0;
            CollectedTreasure = 0;
        }

        public void AddTreasure(int treasureValue)
        {
            CollectedTreasure += treasureValue;
        }

        public void RemoveTreasure(int treasureValueToRemove, out int amountOfTreasureLost)
        {
            amountOfTreasureLost = treasureValueToRemove > CollectedTreasure ? CollectedTreasure :  treasureValueToRemove;

            CollectedTreasure -= treasureValueToRemove;

            if (CollectedTreasure < 0)
                CollectedTreasure = 0;
        }
    }
}
