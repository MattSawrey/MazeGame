using Maze_Game.Entities.Items;
using Maze_Game_Common.SavingLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using static Maze_Game.Maze;
using ConsoleHelpers = Maze_Game_Common.CommonConsole.CommonConsoleHelpers;

namespace Maze_Game
{
    class Program
    {
        // Gets the root directory of this project, regardless of where the project has moved too.
        public static readonly string configurationFilePath = @".\Config.json";

        public static MazeConfiguration config;
        // The two core entities that the game needs to track throughout it's lifecycle.
        public static Player player = new Player();
        public static Maze maze = new Maze(); // Need to allow the user to reset the maze.
        public static int currentRoomIndex;
        public static Random random; // The single instance of the random class that is passed to all areas of the project that require randomisation. This is seeded from a value in the configuration file.

        static void Main(string[] args)
        {
            GameLoop();
            return;
        }

        static void GameLoop()
        {
            bool restartGame = true;
            while (restartGame)
            {
                player.ResetTreasureAndMovesMade();
                Console.ResetColor();
                Console.Clear();
                GetUserName();
                Console.Clear();
                InitializeMaze();
                Console.Clear();
                PlayGame();
                Console.Clear();
                restartGame = PresentResults();
            }
            return;
        }

        #region - Introduction and User Name

        // Gets the user's name and reads the contents of the configuration file
        static void GetUserName()
        {
            Console.WriteLine("-- User Introduction --");
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Hi there! Welcome to Olde Worlde Phunne's new Maze Game.", 20);
            Console.WriteLine();
            player.Name = ConsoleHelpers.GetUserEnteredValueWithCorrectionCheck("your name, brave adventurer");
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Thank you, {player.Name}...", 40);
            ConsoleHelpers.ShakeConsole();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"What was that?!", 40, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($".....", 200, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Probably just a monster from the Maze.", 40, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"No need to worry {player.Name}!", 60, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($".....", 200, true);
            Console.WriteLine();
            ConsoleHelpers.WaitForUserToPressEnter(true);
        }

        #endregion

        #region - Intialisation

        static void InitializeMaze()
        {
            Console.WriteLine("-- Maze Generation --");
            Console.WriteLine();

            // Read game config file
            Console.WriteLine("Reading the contents of the configuration file.");
            config = Deserialize.DeserializeFromJson<Maze.MazeConfiguration>(configurationFilePath);
            // Catch config being null
            while (config == null)
            {
                ConsoleHelpers.WriteOutputAsDelayedCharArray("Please fix the configuration file error and try again.", 10, true);
                Console.WriteLine();
                ConsoleHelpers.WaitForUserToPressEnter(true);
                config = Deserialize.DeserializeFromJson<Maze.MazeConfiguration>(configurationFilePath);
            }

            // Use config values to generate maze
            SeedMaze(config.MazeSeed);

            // TODO - Change this to what would you like to do? 3 Commands. Debug Generated Maze. Reseed Maze. StartGame.

            string[] playerCommands = null;
            while (playerCommands == null || playerCommands[0] != "StartGame")
            {
                playerCommands = ConsoleHelpers.PresentAndProcessPlayerCommands(new List<string> { "debugmaze", "reseedmaze {No.}", "startgame" });
                switch (playerCommands[0])
                {
                    case "debugmaze":
                        DebugGeneratedMaze();
                        break;
                    case "reseedmaze":
                        int newSeed;
                        if (playerCommands.Length > 1)
                        {
                            bool couldParse = int.TryParse(playerCommands[1], out newSeed);
                            if (couldParse)
                            {
                                SeedMaze(newSeed);
                            }
                            else
                            {
                                ConsoleHelpers.WriteOutputAsDelayedCharArray("Unable to parse reseed number. Please enter a whole, 32-bit integer.", 20);
                            }
                        }
                        else
                        {
                            ConsoleHelpers.WriteOutputAsDelayedCharArray("No reseed number specified. Please enter a reseed number.", 20);
                        }
                        break;
                    case "startgame":
                        return;
                }
            }
        }

        static void SeedMaze(int seedNumber)
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Seeding Maze", 20);
            random = new Random(seedNumber);
            maze = new Maze();
            maze.GenerateRooms(config, random);
            maze.ConnectRooms(random);
            currentRoomIndex = random.Next(0, maze.Rooms.Count - 1); // The player can be deposited in any room, apart from the final one that contains the exit passage.

            // Display details of Maze
            ConsoleHelpers.WriteOutputAsDelayedCharArray("These are the details of your Maze:", 10, true);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Maze Name: {config.MazeName}", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Rooms: {config.NumberOfRooms}", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Maze Seed: {seedNumber}", 20, true);
        }

        static void DebugGeneratedMaze()
        {
            for (int i = 0; i < maze.Rooms.Count; i++)
            {
                Console.WriteLine();
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"Room {i}.", 10, true);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Passages: {maze.Rooms[i].passages.Length}", 10, true);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Items: {maze.Rooms[i].Treasures.Count}", 10, true);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Threats: {maze.Rooms[i].Threats.Count}", 10, true);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"Room has final exit: {maze.Rooms[i].passages.Any(x => x.isExit)}", 10, true);
            }
        }

        #endregion

        #region - In-Game Loop

        static void PlayGame()
        {
            DisplayMazeIntro();

            bool hasAccessedExitPassage = false;

            do
            {
                hasAccessedExitPassage = ProcessPlayerCommandUntilMazeExit();
                ConsoleHelpers.DrawSeperationLine();
            } while (!hasAccessedExitPassage);

            // Exit point of the game
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Well done, {player.Name}. You've found the exit passage!", 20);

            ConsoleHelpers.WaitForUserToPressEnter(true);
        }

        static bool ProcessPlayerCommandUntilMazeExit()
        {
            var commands = ConsoleHelpers.PresentAndProcessPlayerCommands(new List<string> { "checkpassages", "takepassage {n, s, e, w}", "checkitems", "collectitem {item name}", "hititem {item name}", "defuseitem {item name}", "dropcoin", "resetmaze" });
            var primaryCommand = commands[0];
            switch (primaryCommand)
            {
                case "checkpassages": CheckPassages(); player.NumberOfMovesMade++; break;
                case "takepassage":
                    player.NumberOfMovesMade++;
                    if (commands.Length > 1)
                    {
                        var commandModifier = commands[1];
                        // Check that the passage direction is legitimate.
                        if (!new string[] { "n", "s", "e", "w" }.Contains(commandModifier))
                        {
                            ConsoleHelpers.WriteOutputAsDelayedCharArray("passage direction not recognised. Please enter 'n', 's', 'e' or 'w' as a passage direction.", 20, true);
                            break;
                        }

                        PassageDirections passageDirection = PassageDirections.North;
                        switch (commandModifier)
                        {
                            case "n":
                                passageDirection = PassageDirections.North;
                                break;
                            case "s":
                                passageDirection = PassageDirections.South;
                                break;
                            case "e":
                                passageDirection = PassageDirections.East;
                                break;
                            case "w":
                                passageDirection = PassageDirections.West;
                                break;
                        }

                        if (maze.Rooms[currentRoomIndex].Threats.Any())
                        {
                            ConsoleHelpers.ShakeConsole();
                            ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempted to take the {passageDirection.ToString()} passage, but something grabs your arm and pulls you back into the room!", 20, true);

                            // Player loses treasure
                            int amountOfTreasureLost;
                            player.RemoveTreasure(maze.Rooms[currentRoomIndex].Threats[new Random().Next(0, maze.Rooms[currentRoomIndex].Threats.Count - 1)].TreasureTheftValue, out amountOfTreasureLost);

                            ConsoleHelpers.WriteOutputAsDelayedCharArray($"It attacked you and you lost {amountOfTreasureLost} treasure!", 20, true);
                            return false;
                        }
                        else
                        {
                            return TakePassage(passageDirection);
                        }
                    }
                    else
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray("No direction specified. Please enter a direction with takepassage command.", 20, true);
                    }
                    break;
                case "checkitems":
                    CheckItems();
                    player.NumberOfMovesMade++;
                    break;
                case "collectitem":
                    if (commands.Length > 1)
                    {
                        var subCommandList = commands.ToList();
                        subCommandList.RemoveAt(0);
                        var subCommand = string.Join(' ', subCommandList);
                        CollectItem(subCommand);
                        player.NumberOfMovesMade++;
                    }
                    else
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray("No item specified. Please enter an item name with collectitem command.", 20, true);
                    }
                    break;
                case "hititem":
                    if (commands.Length > 1)
                    {
                        var subCommandList = commands.ToList();
                        subCommandList.RemoveAt(0);
                        var subCommand = string.Join(' ', subCommandList);
                        HitItemWithHammer(subCommand);
                        player.NumberOfMovesMade++;
                    }
                    else
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray("No item specified. Please enter an item name with hititem command.", 20, true);
                    }
                    break;
                case "defuseitem":
                    if (commands.Length > 1)
                    {
                        var subCommandList = commands.ToList();
                        subCommandList.RemoveAt(0);
                        var subCommand = string.Join(' ', subCommandList);
                        DefuseItem(subCommand);
                        player.NumberOfMovesMade++;
                    }
                    else
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray("No item specified. Please enter an item name with defuseitem command.", 20, true);
                    }
                    break;
                case "dropcoin":
                    DropCoin();
                    break;
                case "resetmaze":
                    ResetMaze();
                    break;
                default: break;
            }
            return false;
        }

        static void CheckPassages()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You look around and see....", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{maze.Rooms[currentRoomIndex].passages.Length} " + (maze.Rooms[currentRoomIndex].passages.Length == 1 ? "passage." : "passages."), 10, true);

            foreach (var passage in maze.Rooms[currentRoomIndex].passages)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"A passage to the {passage.passageDirection.ToString()}" + " {" + passage.passageDirection.ToString().Substring(0, 1).ToLower() + "}", 20, true);
        }

        static bool TakePassage(PassageDirections passageDirection)
        {
            // Check that this passage direction exists
            if (!maze.Rooms[currentRoomIndex].passages.Any(x => x.passageDirection == passageDirection))
            {
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempt to take the {passageDirection.ToString()} passage. But it isn't there! You may not go that way and turn back into the room.", 10);
                return false;
            }

            var passage = maze.Rooms[currentRoomIndex].passages.First(x => x.passageDirection == passageDirection);
            if (passage != null)
            {
                // Exit point of the in-game loop
                if (passage.isExit)
                {
                    return true;
                }
                else
                {
                    // Take Passage
                    string newRoomHintText = "no different in this room. You must be back in the same room!";
                    if (maze.Rooms.IndexOf(passage.passageTo) > currentRoomIndex)
                    {
                        newRoomHintText = "a little bit fresher in this room. You must be getting closer to the exit!";
                    }
                    else if (maze.Rooms.IndexOf(passage.passageTo) > currentRoomIndex)
                    {
                        newRoomHintText = "a little more foul in this room. You must be going deeper into the Maze!";
                    }

                    currentRoomIndex = maze.Rooms.IndexOf(passage.passageTo);
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You take the {passageDirection.ToString()} passage and enter a new room.", 10);
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"The air smells {newRoomHintText}", 10, true);

                    if (maze.Rooms[currentRoomIndex].Treasures.Any(x => x.Name == "Dropped Coin"))
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"You can see a coin that you dropped in the middle of this room. You must have been here before!", 10, true);
                    }
                }
            }

            return false;
        }

        static void CheckItems()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You look around and see....", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{maze.Rooms[currentRoomIndex].TotalItemCount} " + (maze.Rooms[currentRoomIndex].TotalItemCount == 1 ? "item" : "items"), 10, true);

            foreach (var treasure in maze.Rooms[currentRoomIndex].Treasures)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"{treasure.Name}", 20, true);

            foreach (var threat in maze.Rooms[currentRoomIndex].Threats)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"{threat.Name}", 20, true);
        }

        static void CollectItem(string itemName)
        {
            for (int i = 0; i < maze.Rooms[currentRoomIndex].Treasures.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Treasures[i].Name.ToLower() == itemName)
                {
                    // Collect treasure, remove it from the list and return
                    player.AddTreasure(maze.Rooms[currentRoomIndex].Treasures[i].TreasureValue);
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You collected the {itemName} and gained {maze.Rooms[currentRoomIndex].Treasures[i].TreasureValue} treasure", 20, true);
                    maze.Rooms[currentRoomIndex].Treasures.RemoveAt(i);
                    return;
                }
            }

            for (int i = 0; i < maze.Rooms[currentRoomIndex].Threats.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Threats[i].Name.ToLower() == itemName)
                {
                    // Player loses treasure
                    int amountOfTreasureLost;
                    player.RemoveTreasure(maze.Rooms[currentRoomIndex].Threats[i].TreasureTheftValue, out amountOfTreasureLost);
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempted to collect the {itemName}.", 20, true);
                    ConsoleHelpers.ShakeConsole();
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"The {itemName} attacked you instead and you lost {amountOfTreasureLost} treasure!", 20, true);
                    return;
                }
            }

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"You couldn't find a {itemName} in the room.", 20, true);
        }

        static void HitItemWithHammer(string itemName)
        {
            for (int i = 0; i < maze.Rooms[currentRoomIndex].Treasures.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Treasures[i].Name.ToLower() == itemName)
                {
                    // Collect treasure, remove it from the list and return
                    ConsoleHelpers.ShakeConsole();
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You hit the {itemName} with a hammer. It smashed and is no longer collectable!", 20, true);
                    maze.Rooms[currentRoomIndex].Treasures.RemoveAt(i);
                    return;
                }
            }

            for (int i = 0; i < maze.Rooms[currentRoomIndex].Threats.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Threats[i].Name.ToLower() == itemName)
                {
                    ConsoleHelpers.ShakeConsole();
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You hit the {itemName} with a hammer.", 20, true);
                    if (maze.Rooms[currentRoomIndex].Threats[i].Solution.ToLower() == "hammer")
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"It worked and the {itemName} ran away!", 20, true);
                        maze.Rooms[currentRoomIndex].Threats.RemoveAt(i);
                    }
                    else
                    {
                        int amountOfTreasureLost;
                        player.RemoveTreasure(maze.Rooms[currentRoomIndex].Threats[i].TreasureTheftValue, out amountOfTreasureLost);
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"The {itemName} attacked you instead and you lost {amountOfTreasureLost} treasure!", 20, true);
                    }
                    return;
                }
            }

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"You couldn't find a {itemName} in the room to hit.", 20, true);
        }

        static void DefuseItem(string itemName)
        {
            for (int i = 0; i < maze.Rooms[currentRoomIndex].Treasures.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Treasures[i].Name.ToLower() == itemName)
                {
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You can't defuse a {itemName}! You've wasted a turn!", 20, true);
                    return;
                }
            }

            for (int i = 0; i < maze.Rooms[currentRoomIndex].Threats.Count; i++)
            {
                if (maze.Rooms[currentRoomIndex].Threats[i].Name.ToLower() == itemName)
                {
                    ConsoleHelpers.ShakeConsole();
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempt to defuse the {itemName}.", 20, true);
                    if (maze.Rooms[currentRoomIndex].Threats[i].Solution.ToLower() == "defuse")
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"It worked and the {itemName} is no longer a threat!", 20, true);
                        maze.Rooms[currentRoomIndex].Threats.RemoveAt(i);
                    }
                    else
                    {
                        int amountOfTreasureLost;
                        player.RemoveTreasure(maze.Rooms[currentRoomIndex].Threats[i].TreasureTheftValue, out amountOfTreasureLost);
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"The {itemName} attacked you instead and you lost {amountOfTreasureLost} treasure!", 20, true);
                    }
                    return;
                }
            }

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"You couldn't find a {itemName} in the room to defuse.", 20, true);
        }

        static void DropCoin()
        {
            if (player.CollectedTreasure == 0)
            {
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"You don't have any coins to drop!", 20, true);
                return;
            }

            if (maze.Rooms[currentRoomIndex].Treasures.Any(x => x.Name == "Dropped Coin"))
            {
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"You already dropped a coin in this room. Use dropped coins to mark rooms you've already visited.", 20, true);
                return;
            }

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"You dropped 1 coin in the center of the room. Hopefully this will help to identify the room if you return.", 20, true);
            player.RemoveTreasure(1);
            maze.Rooms[currentRoomIndex].AddDroppedCoin();
        }

        static void ResetMaze()
        {
            bool reset = ConsoleHelpers.RequirePositiveInput("resetmaze");

            if (!reset)
            {
                return;
            }
            Console.Clear();
            SeedMaze(config.MazeSeed);
            player.ResetTreasureAndMovesMade();
            currentRoomIndex = 0;
            Console.WriteLine();
            ConsoleHelpers.WaitForUserToPressEnter(true);
            Console.Clear();
            DisplayMazeIntro();
        }

        static void DisplayMazeIntro()
        {
            Console.WriteLine("-- Maze --");
            Console.WriteLine();

            ConsoleHelpers.WriteOutputAsDelayedCharArray("Welcome to the Dungeon. We've got fun and games!", 10);
            ConsoleHelpers.WriteOutputAsDelayedCharArray("...", 200, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Oh, sorry. Hello there.", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{player.Name}, you find yourself in a dark room in the middle of a Maze with no idea how to escape!", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Your aim is to leave the Maze with the most treasure you can.", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"But be carefull! Threats lurk in the Maze and will stop you progressing between rooms if you don't deal with them first.", 20, true);
            Console.WriteLine();
        }

        #endregion

        #region - End Results

        static bool PresentResults()
        {
            Console.WriteLine("-- Results --");
            Console.WriteLine();

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Congratulations on making it out of the maze {player.Name}!", 20);
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Here are your stats:", 20, true);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Number of moves made: {player.NumberOfMovesMade}", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Amount of Treasure Collected: {player.CollectedTreasure}", 20, true);

            string[] playerCommands = ConsoleHelpers.PresentAndProcessPlayerCommands(new List<string>() { "restartgame", "endgame" });
            switch (playerCommands[0])
            {
                case "restartgame":
                    return true;
                case "endgame":
                    return false;
            }
            return false;
        }

        #endregion
    }
}
