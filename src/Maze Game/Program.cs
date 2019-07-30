using Maze_Game.Entities.Items;
using Maze_Game_Common.SavingLoading;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ConsoleHelpers = Maze_Game_Common.CommonConsole.CommonConsoleHelpers;

namespace Maze_Game
{
    class Program
    {
        // TODO - add configuration files for various bodies of text that can change up the wording on different playthroughs.

        // Gets the root directory of this project, regardless of where the project has moved too.
        private static readonly string projectDirectoryRoot = Directory.GetCurrentDirectory().Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\"));
        public static string configurationFilePath = $"{projectDirectoryRoot}/config.json";

        // The two core entities that the game needs to track throughout it's lifecycle.
        public static Player player = new Player();
        public static Maze maze = new Maze(); // Need to allow the user to reset the maze.
        public static int currentRoomIndex = 0; // TODO - Randomise so that the player is deposited in a random room at the start of the maze.
        public static Random random;


        static void Main(string[] args)
        {
            //Console.WriteLine("Congratulations, you escaped the maze alive!");
            //Console.WriteLine();
            //Console.WriteLine("Here are your game stats:");
            //Console.WriteLine("Number of moves made: 42");
            //Console.WriteLine("Total treasure collected: 200");
            //Console.WriteLine();
            //Console.WriteLine("Would you like to play again?");
            //Console.WriteLine("Y/N");
            GameLoop();
        }

        static void GameLoop()
        {
            InitialiseGame();
            Console.Clear();
            PlayGame();
        }

        // Gets the user's name and reads the contents of the configuration file
        static void InitialiseGame()
        {
            Console.ResetColor();

            Console.WriteLine("Hello user! Welcome to Olde Worlde Phunne's new Maze Game.");

            string name = ConsoleHelpers.GetValueWithCorrectionCheck("your name, brave adventurer");

            ConsoleHelpers.ShakeConsole();

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Thank you, {name}...", 100);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Are you ready to enter the bog of eternal stench?", 50);
            Console.WriteLine();

            var playerIsReady = ConsoleHelpers.RequirePositiveInput();

            Console.Clear();
            Console.WriteLine("Reading the contents of the configuration file");

            Maze.MazeConfiguration config = Deserialize.DeserializeFromJson<Maze.MazeConfiguration>(configurationFilePath);

            // Catch config being null
            while (config == null)
            {
                Console.Write("Configuration file does not exist, or its contents cannot be read. Please ensure that the configuration file exists and it is in the correct .json format.");

                Console.WriteLine("Press enter to attempt to load configuration files again");
                ConsoleHelpers.WaitForUserToPressEnter();

                config = Deserialize.DeserializeFromJson<Maze.MazeConfiguration>(configurationFilePath);
            }

            ConsoleHelpers.WriteOutputAsDelayedCharArray("These are the details of your Maze:", 100);
            Console.WriteLine();
            Console.WriteLine($"Maze Name: {config.MazeName}");
            Console.WriteLine($"No. Rooms: {config.NumberOfRooms}");
            Console.WriteLine($"Maze Difficulty: {config.DifficultyLevel}");

            // Use config values to generate maze
            random = new Random(config.MazeSeed);
            maze.GenerateRooms(config, random);
            maze.ConnectRooms(random);

            DebugGeneratedMaze();

            ConsoleHelpers.WriteOutputAsDelayedCharArray("Would you like to enter the Maze?", 20);
            ConsoleHelpers.RequirePositiveInput("Begin Maze");
        }

        static void PlayGame()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Welcome to the Dungeon! We've got fun and games...", 10);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Oh, sorry. Hello there.", 20);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You're in a dark room in the middle of a Maze with no idea how to escape. What do you want to do?", 20);
            Console.WriteLine();

            bool hasAccessedExitPassage = false;

            do
            {
                PresentPlayerCommandOptions();
                hasAccessedExitPassage = ProcessPlayerCommand();
            } while (!hasAccessedExitPassage);

            // Exit point of the game
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Congrats, {player.Name}. You've emerged from the Maze back into the real world!", 20);
        }

        static void PresentPlayerCommandOptions()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Commands: CheckPassages, CheckItems, ResetMaze, TakePassage {N, S, E, W}", 20);
            Console.WriteLine();
        }

        static bool ProcessPlayerCommand()
        {
            var input = Console.ReadLine();
            var commands = input.Split(' ');
            var primaryCommand = commands[0];
            switch (primaryCommand)
            {
                case "CheckPassages": CheckPassages(); break;
                case "TryPassage": break;
                case "CheckItems": break;
                case "ResetMaze":
                    ConsoleHelpers.RequirePositiveInput();
                    break;
                case "TakePassage":
                    var commandModifier = commands[1];
                    PassageDirections passageDirection = PassageDirections.North;
                    switch (commandModifier)
                    {
                        case "N":
                            passageDirection = PassageDirections.North;
                            break;
                        case "S":
                            passageDirection = PassageDirections.South;
                            break;
                        case "E":
                            passageDirection = PassageDirections.East;
                            break;
                        case "W":
                            passageDirection = PassageDirections.West;
                            break;
                    }

                    var passage = maze.Rooms[currentRoomIndex].passages.First(x => x.passageDirection == passageDirection);
                    if (passage != null)
                    {
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
                    else
                    {
                        // Say there isn't a passage
                        ConsoleHelpers.WriteOutputAsDelayedCharArray($"You attempt to take the {passageDirection.ToString()} passage. But it isn't there! You turn back into the room.", 10);
                    }
                    break;
                default: break;
            }
            return false;
        }

        static void TakePassage()
        {

        }

        static void ResetMaze()
        {

        }

        static void CheckPassages()
        {
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You look around and see....", 20);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{maze.Rooms[currentRoomIndex].passages.Length} passages.", 10);

            foreach (var passage in maze.Rooms[currentRoomIndex].passages)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"A passage to the {passage.passageDirection.ToString()}", 20);
        }

        static void ProcessInputAction()
        {

        }

        static void PlayGameOutput()
        {

        }

        static void ReceivePlayerInput()
        {

        }

        static void DebugGeneratedMaze()
        {
            for (int i = 0; i < maze.Rooms.Count; i++)
            {
                Console.WriteLine();
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"Room {i}.", 10);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Passages: {maze.Rooms[i].passages.Length}", 10);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Items: {maze.Rooms[i].Treasures.Count}", 10);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"No. Threats: {maze.Rooms[i].Threats.Count}", 10);
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"Room has final exit: {maze.Rooms[i].passages.Any(x => x.isExit)}", 10);
            }
        }
    }
}
