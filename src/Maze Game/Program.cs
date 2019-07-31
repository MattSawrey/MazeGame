using Maze_Game.Entities.Items;
using Maze_Game_Common.SavingLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Maze_Game.Maze;
using ConsoleHelpers = Maze_Game_Common.CommonConsole.CommonConsoleHelpers;

namespace Maze_Game
{
    class Program
    {
        // Gets the root directory of this project, regardless of where the project has moved too.
        private static readonly string projectDirectoryRoot = Directory.GetCurrentDirectory();
        public static string configurationFilePath = $"{projectDirectoryRoot}/config.json";

        public static MazeConfiguration config;
        // The two core entities that the game needs to track throughout it's lifecycle.
        public static Player player = new Player();
        public static Maze maze = new Maze(); // Need to allow the user to reset the maze.
        public static int currentRoomIndex = 0; // TODO - Randomise so that the player is deposited in a random room at the start of the maze.
        public static Random random;


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

        #region - Game Initialisation


        // Gets the user's name and reads the contents of the configuration file
        static void GetUserName()
        {
            Console.WriteLine("-- User Introduction --");
            Console.WriteLine();
            // Get player name
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Hi there! Welcome to Olde Worlde Phunne's new Maze Game.", 20);
            player.Name = ConsoleHelpers.GetUserEnteredValueWithCorrectionCheck("your name, brave adventurer");
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Thank you, {player.Name}...", 60);
            ConsoleHelpers.ShakeConsole();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"What was that?!", 60, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"...", 400, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Probably just a monster from the Maze.", 60, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"No need to worry {player.Name}!", 60, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"...", 400, true);
            Console.WriteLine();
            ConsoleHelpers.WaitForUserToPressEnter(true);

            //ConsoleHelpers.WriteOutputAsDelayedCharArray("Are you ready to enter the Maze?", 50);
            //Console.WriteLine();
            //var playerIsReady = ConsoleHelpers.RequirePositiveInput();
        }


        static void InitializeMaze()
        {
            Console.WriteLine("-- Maze Generation --");
            Console.WriteLine();

            // Read game config file
            Console.WriteLine("Reading the contents of the configuration file");
            config = Deserialize.DeserializeFromJson<Maze.MazeConfiguration>(configurationFilePath);
            // Catch config being null
            while (config == null)
            {
                Console.Write("Configuration file does not exist, or its contents cannot be read. Please ensure that the configuration file exists and it is in the correct .json format.");
                Console.WriteLine("Press enter to attempt to load configuration files again");
                ConsoleHelpers.WaitForUserToPressEnter();
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

            // Display details of Maze
            ConsoleHelpers.WriteOutputAsDelayedCharArray("These are the details of your Maze:", 10, true);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Maze Name: {config.MazeName}", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Rooms: {config.NumberOfRooms}", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Maze Seed: {seedNumber}", 20, true);
        }

        #endregion

        #region - In-Game Loop

        static void PlayGame()
        {
            Console.WriteLine("-- Maze --");
            Console.WriteLine();

            ConsoleHelpers.WriteOutputAsDelayedCharArray("Welcome to the Dungeon. We've got fun and games!", 10);
            ConsoleHelpers.WriteOutputAsDelayedCharArray("...", 200, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Oh, sorry. Hello there.", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{player.Name}, you find yourself in a dark room in the middle of a Maze with no idea how to escape!", 20, true);
            Console.WriteLine();

            bool hasAccessedExitPassage = false;

            do
            {
                hasAccessedExitPassage = ProcessPlayerCommand();
            } while (!hasAccessedExitPassage);

            // Exit point of the game
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Well done, {player.Name}. You've found the exit passage!", 20);

            ConsoleHelpers.WaitForUserToPressEnter(true);
        }

        #endregion

        //static void PresentPlayerCommandOptions()
        //{
        //    ConsoleHelpers.WriteOutputAsDelayedCharArray("Commands: checkpassages, takepassage {n, s, e, w}, checkitems, resetmaze", 20, true);
        //    Console.WriteLine();
        //}

        static bool ProcessPlayerCommand()
        {
            var commands = ConsoleHelpers.PresentAndProcessPlayerCommands(new List<string> { "checkpassages", "takepassage {n, s, e, w}", "checkitems", "resetmaze" });
            var primaryCommand = commands[0];
            switch (primaryCommand)
            {
                case "checkpassages": CheckPassages(); break;
                case "takepassage":
                    var commandModifier = commands[1];
                    // Check that the passage direction is legitimate.
                    if (!new string[]{"n", "s", "e", "w"}.Contains(commandModifier))
                    {
                        ConsoleHelpers.WriteOutputAsDelayedCharArray("passage direction not recognised. Please enter n, s, e or w as a passage direction.", 20, true);
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
                    return TakePassage(passageDirection);
                case "checkitems": break;
                case "resetmaze":
                    ConsoleHelpers.RequirePositiveInput();
                    break;
                default: break;
            }
            return false;
        }

        static bool TakePassage(PassageDirections passageDirection)
        {
            // Check that this passage direction exists
            if (!maze.Rooms[currentRoomIndex].passages.Any(x => x.passageDirection == passageDirection))
            {
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempt to take the {passageDirection.ToString()} passage. But it isn't there! You turn back into the room.", 10);
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
                    currentRoomIndex = maze.Rooms.IndexOf(passage.passageTo);
                    ConsoleHelpers.WriteOutputAsDelayedCharArray($"You take the {passageDirection.ToString()} passage and enter a new room.", 10);
                }
            }

            return false;
        }

        static void ResetMaze()
        {

        }

        static void CheckPassages()
        {
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You look around and see....", 20, true);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{maze.Rooms[currentRoomIndex].passages.Length} passages.", 10, true);

            foreach (var passage in maze.Rooms[currentRoomIndex].passages)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"A passage to the {passage.passageDirection.ToString()}", 20, true);
        }

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
    }
}
