namespace MinotaurLabyrinth
{
    // An interface to represent one of many commands in the game. Each new command should
    // implement this interface.
    public interface ICommand
    {
        void Execute(Hero hero, Map map);
    }

    public interface ISecretCommand : ICommand { }

    // Represents a secret command that is not visible to the player.
    public class SecretCommand : ISecretCommand
    {
        public void Execute(Hero hero, Map map)
        {
            Console.WriteLine("This is a secret command.");
        }
    }

    // Represents a movement command, along with a specific direction to move.
    public abstract class BaseMoveCommand : ICommand
    {
        // The direction to move.
        public abstract Direction Direction { get; }

        // Causes the player's position to be updated with a new position, shifted in the intended direction,
        // but only if the destination stays on the map. Otherwise, nothing happens.
        public void Execute(Hero hero, Map map)
        {
            Location currentLocation = hero.Location;
            Location newLocation = Direction switch
            {
                Direction.North => new Location(currentLocation.Row - 1, currentLocation.Column),
                Direction.South => new Location(currentLocation.Row + 1, currentLocation.Column),
                Direction.West => new Location(currentLocation.Row, currentLocation.Column - 1),
                Direction.East => new Location(currentLocation.Row, currentLocation.Column + 1),
                _ => throw new ArgumentException("Invalid Direction command.")
            };

            if (map.IsOnMap(newLocation))
            {
                Room room = map.GetRoomAtLocation(newLocation);
                if (room.State == RoomState.Destoryed)
                {
                    ConsoleHelper.WriteLine($"This room[{newLocation.Row},{newLocation.Column}] already destoryed, cannot enter into.", ConsoleColor.Red);
                }
                else
                {
                    hero.Location = newLocation;
                }
            }
            else
                ConsoleHelper.WriteLine("There is a wall there.", ConsoleColor.Red);
        }
    }

    public class MoveNorthCommand : BaseMoveCommand
    {
        public override Direction Direction => Direction.North;
    }

    public class MoveSouthCommand : BaseMoveCommand
    {
        public override Direction Direction => Direction.South;
    }

    public class MoveWestCommand : BaseMoveCommand
    {
        public override Direction Direction => Direction.West;
    }

    public class MoveEastCommand : BaseMoveCommand
    {
        public override Direction Direction => Direction.East;
    }

    // A command that represents a request to pick up the sword.
    public class GetSwordCommand : ICommand
    {
        // Retrieves the sword if the player is in the room with the sword. Otherwise, nothing happens.
        public void Execute(Hero hero, Map map)
        {
            if (map.GetRoomTypeAtLocation(hero.Location) == RoomType.Sword)
            {
                if (hero.HasSword)
                {
                    ConsoleHelper.WriteLine("You've already picked up the sword from this room.", ConsoleColor.Red);
                }
                hero.HasSword = true;
            }
            else
            {
                ConsoleHelper.WriteLine("The sword is not in this room. There was no effect.", ConsoleColor.Red);
            }
        }
    }

    // Displays all the room occurrences - for debugging and testing
    public class DebugMapCommand : ICommand
    {
        public void Execute(Hero hero, Map map)
        {
            map.DebugMode = !map.DebugMode;
        }
    }

    public class QuitCommand : ICommand
    {
        public void Execute(Hero hero, Map map)
        {
            hero.Kill("You abandoned your quest.");
        }
    }
}
