using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void WaitForUserToPressEnter()
        {
            Console.WriteLine();
            Console.WriteLine("Please Press Enter.");
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

        public static void WriteOutputAsDelayedCharArray(string text, int delaySpeed)
        {
            Console.WriteLine();
            char[] chars = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(delaySpeed);
            }
        }

        // Keeps the user in a confirmation loop until they confirm they are happy with their input value
        public static string GetValueWithCorrectionCheck(string nameOfValue)
        {
            string value;
            bool valueIsCorrect;
            do
            {
                Console.WriteLine();
                Console.WriteLine($"Please enter {nameOfValue}: ");
                value = Console.ReadLine();
                valueIsCorrect = RequirePositiveInput(value);
            } while (!valueIsCorrect);

            return value;
        }

        public static void PresentAndProcessPlayerCommands(List<string> commands)
        {
            string commandList = string.Join(", ", commands);
            WriteOutputAsDelayedCharArray($"Commands: {commandList}", 10);

            // Deal with the player entering an incorrect command.
        }
    }
}
