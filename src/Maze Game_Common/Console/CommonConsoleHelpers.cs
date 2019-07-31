using System;
using System.Collections.Generic;
using System.Threading;

namespace Maze_Game_Common.CommonConsole
{
    public static class CommonConsoleHelpers
    {
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

        public static bool RequirePositiveInput(string checkValue)
        {
            Console.WriteLine();
            Console.WriteLine($"You entered: {checkValue}.");
            Console.WriteLine("Is that correct?");
            return RequirePositiveInput();
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

        // TODO - rewrite to use number of seconds and shake intensity
        public static void ShakeConsole()
        {
            Random rand = new Random();

            var currentWindowHeight = Console.WindowHeight;
            var currentWindowWidth = Console.WindowWidth;

            for (int i = 0; i < 100; i++)
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
            bool valueIsCorrect;
            do
            {
                Console.WriteLine();
                WriteOutputAsDelayedCharArray($"Please enter {nameOfValue}: ", 20, true);
                Console.WriteLine();
                value = Console.ReadLine();
                valueIsCorrect = RequirePositiveInput(value);
            } while (!valueIsCorrect);

            return value;
        }

        // TODO - Deal with sub commands
        public static string[] PresentAndProcessPlayerCommands(List<string> commands)
        {
            string commandList = string.Join(", ", commands);

            // Remove the sub-commands from the command list for checking purposes.
            // Sub-command errors are handled by the caller.
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i] = commands[i].Split(" ")[0];
            }

            string enteredCommand = "";
            var primaryCommand = "";

            while (!commands.Contains(primaryCommand))
            {
                Console.WriteLine();
                WriteOutputAsDelayedCharArray("What would you like to do?", 10, true);
                Console.WriteLine();
                WriteOutputAsDelayedCharArray($"Commands: {commandList}", 10, true);
                Console.WriteLine();
                enteredCommand = Console.ReadLine().ToLower();
                var enteredCommands = enteredCommand.Split(' ');
                primaryCommand = enteredCommands[0];
                if (!commands.Contains(primaryCommand))
                {
                    WriteOutputAsDelayedCharArray($"{enteredCommand} is not a recognised command. Please review the command list and enter a recognised command.", 10, true);
                }
                else
                {
                    return enteredCommands;
                }
            }
            return null;
        }
    }
}
