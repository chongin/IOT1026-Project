namespace MinotaurLabyrinth
{
    static class Program
    {
        static void Main()
        {
            ConsoleHelper.Write("Do you want to play a small, medium, or large game? ", ConsoleColor.White);

            // Default game setting in the event user does not input a proper size.
            Size mapSize = Console.ReadLine() switch
            {
                "small" => Size.Small,
                "large" => Size.Large,
                _ => Size.Medium // Make a medium game if input is "medium" or anything else
            };

            // A random seed (Guid.NewGuid().GetHashCode()) is used for each game.
            // Consider adding a way the user can choose a game seed (this is useful for testing as well).
            // If the user doesn't enter a seed, you can use the random seed
            LabyrinthGame game = new(mapSize, Guid.NewGuid().GetHashCode());
            game.Run();
        }
    }
}
