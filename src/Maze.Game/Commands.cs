namespace Maze.Game
{
    public static class Commands
    {
        public static Command CheckPassages = new Command("CheckPassages", "cp");
        public static Command TakePassage = new Command("takepassage", "tp");
        public static Command CheckItems = new Command("checkitems", "ci");
        public static Command CollectItem = new Command("collectitem", "co");
        public static Command HitItem = new Command("hititem", "hi");
        public static Command DefuseItem = new Command("defuseitem", "di");
        public static Command DropcCoin = new Command("dropcoin", "dc");
        public static Command ResetMaze = new Command("resetmaze", "rm");
    }

    public class Command
    { 
        public string Name { get; set; }
        public string ShortName { get; set; }

        public Command(string name, string shortName)
        { 
            Name = name;
            ShortName = shortName;
        }
    }
}
