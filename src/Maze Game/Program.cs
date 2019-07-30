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
        // Gets the root directory of this project, regardless of where the project has moved too.
        private static readonly string projectDirectoryRoot = Directory.GetCurrentDirectory().Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\"));
        public static string configurationFilePath = $"{projectDirectoryRoot}/config.json";

        // The two core entities that the game needs to track throughout it's lifecycle.
        public static Player player = new Player();
        public static Maze maze = new Maze(); // Need to allow the user to reset the maze.

        static void Main(string[] args)
        {
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
            Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory().Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));

            Console.ResetColor();

            Console.WriteLine("Hello user! Welcome to Olde Worlde Phunne's new Maze Game.");

            string name = ConsoleHelpers.GetValueWithCorrectionCheck("your name");

            ConsoleHelpers.ShakeConsole();

            Console.WriteLine();

            Thread.Sleep(1000);

            ConsoleHelpers.WriteOutputAsDelayedCharArray($"Thank you, {name}", 100);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Are you ready to enter the bog of eternal stench?", 50);

            var playerIsReady = ConsoleHelpers.RequirePositiveInput();

            Thread.Sleep(1000);

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

            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("These are the details of your Maze:", 100);
            Console.WriteLine();
            Console.WriteLine($"Maze Name: {config.MazeName}");
            Console.WriteLine($"No. Rooms: {config.NumberOfRooms}");
            Console.WriteLine($"Maze Difficulty: {config.DifficultyLevel}");

            maze.GenerateRooms(config);

            DebugGeneratedMaze();

            ConsoleHelpers.WaitForUserToPressEnter();

            ConsoleHelpers.WriteOutputAsDelayedCharArray("Would you like to enter the Maze?", 20);
            ConsoleHelpers.RequirePositiveInput("Begin Maze");
        }

        static void PlayGame()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Welcome to the Dungeon! We've got fun and games...", 40);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Oh, sorry. Hello there.", 20);
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You're in a dark room in the middle of a Maze with no idea how to escape. What do you want to do?", 20);
            Console.WriteLine();

            PresentPlayerCommandOptions();
            ProcessPlayerCommand();
        }

        static void PresentPlayerCommandOptions()
        {
            ConsoleHelpers.WriteOutputAsDelayedCharArray("Commands: CheckPassages, CheckItems, ResetMaze", 20);
            Console.WriteLine();
        }

        static void ProcessPlayerCommand()
        {
            var input = Console.ReadLine();
            switch (input)
            {
                case "CheckPassages": CheckPassages(); break;
                case "TryPassage": break;
                case "CheckItems": break;
                case "ResetMaze":
                    ConsoleHelpers.RequirePositiveInput();
                    break;
                default: break;
            }
        }

        static void ResetMaze()
        {

        }

        static void CheckPassages()
        {
            Console.WriteLine();
            ConsoleHelpers.WriteOutputAsDelayedCharArray("You look around and see....", 20);
            ConsoleHelpers.WriteOutputAsDelayedCharArray($"{maze.Rooms[0].passages.Length} passages.", 40);

            foreach (var passage in maze.Rooms[0].passages)
                ConsoleHelpers.WriteOutputAsDelayedCharArray($"A passage to the {passage.passageDirection.ToString()}", 60);
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
