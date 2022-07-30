using System;
using System.Collections.Generic;
using System.Threading;

namespace Maze.Game.Common
{
    public static class CommonConsoleHelpers
    {
        public static bool RequirePositiveInput(string checkValue)
        {
            Console.WriteLine();
            Console.WriteLine($"You entered: {checkValue}.");
            Console.WriteLine("Is that correct?");
            return RequirePositiveInput();
        }

        public static bool RequirePositiveInput()
        {
            Console.WriteLine("Please enter Y/N");
            ConsoleKey enteredKey;
            do
            {
                enteredKey = Console.ReadKey(true).Key;
            } while (enteredKey != ConsoleKey.Y && enteredKey != ConsoleKey.N);

            if (enteredKey == ConsoleKey.Y)
            {
                return true;
            }
            else if (enteredKey == ConsoleKey.N)
            {
                return false;
            }
            return false;
        }

        public static void WaitForUserToPressEnter(bool newLine = false)
        {
            if (newLine)
                Console.WriteLine();

            Console.WriteLine("Please Press Enter to continue.");
            do
            {
            } while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        }

        public static void ShakeConsole()
        {
            Random rand = new Random();

            var currentWindowHeight = Console.WindowHeight;
            var currentWindowWidth = Console.WindowWidth;

            for (int i = 0; i < 60; i++)
            {
                Console.WindowHeight = currentWindowHeight + rand.Next(-4, 4);
                Console.WindowWidth = currentWindowWidth + rand.Next(-4, 4);
            }
        }

        public static void WriteOutputAsDelayedCharArray(string text, int delaySpeed, bool newLine = false)
        {
            if (newLine)
                Console.WriteLine();

            char[] chars = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(delaySpeed);
            }
        }

        // Keeps the user in a confirmation loop until they confirm they are happy with their input value
        public static string GetUserEnteredValueWithCorrectionCheck(string nameOfValue)
        {
            string value;
            bool valueIsCorrect = false;
            do
            {
                WriteOutputAsDelayedCharArray($"Please enter {nameOfValue}: ", 20, true);
                Console.WriteLine();
                value = Console.ReadLine();
                if (value != "")
                {
                    valueIsCorrect = RequirePositiveInput(value);
                }
            } while (!valueIsCorrect);

            return value;
        }

        // TODO - Deal with sub commands
        public static string[] PresentAndProcessPlayerCommands(List<string> userCommands)
        {
            // Remove the sub-commands from the command list for checking purposes.
            // Sub-command errors are handled by the caller.
            List<string> commands = new List<string>();
            for (int i = 0; i < userCommands.Count; i++)
            {
                commands.Add(userCommands[i].Split(" ")[0]);
            }

            string enteredCommand = "";
            string primaryCommand = "";

            while (!commands.Contains(primaryCommand))
            {
                Console.WriteLine();
                WriteOutputAsDelayedCharArray("What would you like to do?", 10, true);
                Console.WriteLine();
                WriteOutputAsDelayedCharArray($"Commands:", 10, true);
                Console.WriteLine();
                foreach (var userCommand in userCommands)
                {
                    WriteOutputAsDelayedCharArray($"- {userCommand}", 2, true);
                }
                Console.WriteLine();
                Console.WriteLine();
                enteredCommand = Console.ReadLine().ToLower();
                string[] enteredCommands = enteredCommand.Split(' ');
                primaryCommand = enteredCommands[0];
                if (!commands.Contains(primaryCommand))
                {
                    WriteOutputAsDelayedCharArray($"{enteredCommand} is not a recognised command. Please review the command list and enter a recognised command.", 10, true);
                    DrawSeperationLine();
                }
                else
                {
                    return enteredCommands;
                }
            }
            return null;
        }

        public static void DrawSeperationLine()
        {
            WriteOutputAsDelayedCharArray("----------", 10, true);
        }
    }
}
