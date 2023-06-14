namespace MinotaurLabyrinth
{
    // The minotaur labyrinth game. Tracks the progression of a single round of gameplay.
    public class LabyrinthGame
    {
        // The map being used by the game.
        public Map Map { get; }

        // The player playing the game.
        public Hero Hero { get; }

        // Looks up what room type the player is currently in.
        public Room CurrentRoom => Map.GetRoomAtLocation(Hero.Location);
        // Initializes a new game round with a specific map and player.
        public LabyrinthGame(Size mapSize, int seed)
        {
            RandomNumberGenerator.SetSeed(seed);
            (Map, Hero) = LabyrinthCreator.InitializeMap(mapSize);
        }

        // Runs the game one turn at a time.
        public void Run()
        {
            Display.ScreenUpdate(Hero, Map);
            // This is the game loop. Each turn runs through this while loop once.
            while (!Hero.IsVictorious && Hero.IsAlive)
            {
                //add new logic to check here got poison or not.
                if (Hero.IsPoisoned && Hero.StillCanPlaySteps < 0)
                {
                    //handle the callback that the room set it.
                    Hero.HandleCallback();
                    ConsoleHelper.WriteLine($"{Hero.CauseOfDeath}\nYou lost.", ConsoleColor.Red);
                    return;
                }

                ICommand command = GetCommand();
                Console.Clear();
                if (Hero.IsAlive) // Player did not quit the game
                {
                    command.Execute(Hero, Map);
                    Location currentLocation = Hero.Location;
                    CurrentRoom.Activate(Hero, Map);
                    // If the room interaction moves the player
                    // Activate the room the player has been moved to
                    while (currentLocation != Hero.Location)
                    {
                        currentLocation = Hero.Location;
                        CurrentRoom.Activate(Hero, Map);
                    }

                    if (Hero.IsPoisoned)
                    {
                        Hero.StillCanPlaySteps--; //current command already decrease 1, so need to check StillCanPlaySteps < 0
                    }
                }
                Display.ScreenUpdate(Hero, Map);
            }
            if (Hero.IsVictorious)
                ConsoleHelper.WriteLine("You have claimed the magic sword, and you have escaped with your life!\nYou win!", ConsoleColor.DarkGreen);
            else
                ConsoleHelper.WriteLine($"{Hero.CauseOfDeath}\nYou lost.", ConsoleColor.Red);
        }

        // Gets an 'ICommand' object that represents the player's desires.
        private ICommand GetCommand()
        {
            ICommand? command = null;
            do
            {
                ConsoleHelper.Write("What do you want to do? ", ConsoleColor.Black);
                Console.ForegroundColor = ConsoleColor.Cyan;
                string? input = Console.ReadLine();

                if (input == null)
                {
                    ConsoleHelper.WriteLine("Unable to parse your instruction. Try again.", ConsoleColor.Red);
                    continue; // If input is null, continue with the next iteration of the loop.
                }

                input = input.ToLower();
                command = Hero.CommandList.GetCommand(input);

                if (command == null)
                {
                    // If the input is not found in the command list, we have no clue what the command was. Try again.
                    ConsoleHelper.WriteLine($"I did not understand '{input}'.", ConsoleColor.Red);
                }
            } while (command == null);
            return command;
        }
    }
}
