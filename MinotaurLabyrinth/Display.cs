namespace MinotaurLabyrinth
{
    static class Display
    {
        public static void ScreenUpdate(Hero hero, Map map)
        {
            ConsoleHelper.WriteLine("--------------------------------------------------------------------------------", ConsoleColor.Gray);
            ConsoleHelper.WriteLine("Map", ConsoleColor.White);
            DisplayMap(hero, map);
            Console.WriteLine();
            ConsoleHelper.WriteLine("Commands:", ConsoleColor.White);
            ConsoleHelper.Write($"{hero.CommandList}", ConsoleColor.White);
            Console.WriteLine();
            DisplayStatus(hero, map);
            ConsoleHelper.WriteLine("--------------------------------------------------------------------------------", ConsoleColor.Gray);
            Console.WriteLine();
        }

        // Displays the status to the player, including what room they are in and asks each sense to display
        // itself if it is currently relevant.
        public static void DisplayStatus(Hero hero, Map map)
        {
            bool somethingSensed = DisplaySenses(hero, map);
            if (!somethingSensed)
                ConsoleHelper.WriteLine("You sense nothing of interest nearby.", ConsoleColor.Gray);
            if (hero.HasSword)
                ConsoleHelper.WriteLine("You are currently carrying the sword! Make haste for the exit!", ConsoleColor.DarkYellow);
        }

        // Asks each sense to display itself if relevant. Returns true if something is sensed and false otherwise.
        private static bool DisplaySenses(Hero hero, Map map)
        {
            int sensedRooms = 0;
            var sensableLocs = map.GetSensableLocations(hero); // All the locations the player can sense based on their sense range
            (int px, int py) = hero.Location;
            foreach (var loc in sensableLocs)
            {
                var room = map.GetRoomAtLocation(loc);
                (int lx, int ly) = loc;
                int distanceFromPlayer = Math.Abs(lx - px) + Math.Abs(ly - py);
                if (room.DisplaySense(hero, distanceFromPlayer)) ++sensedRooms;
            }
            return sensedRooms > 0;
        }

        static public void DisplayMap(Hero hero, Map map)
        {
            map.Display(hero.Location, hero.HasMap, map.DebugMode);
        }
    }

    public static class ConsoleHelper
    {
        // Changes to the specified color and then displays the text on its own line.
        public static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        public static void Write(DisplayDetails details)
        {
            Console.ForegroundColor = details.Color;
            Console.Write(details.Text);
        }

        // Changes to the specified color and then displays the text without moving to the next line.
        public static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
    }
}
