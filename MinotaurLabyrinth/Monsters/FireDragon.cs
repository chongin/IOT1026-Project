using System;
namespace MinotaurLabyrinth
{
    public class FireDragon : Monster, IDestoryable
    {
        private int _fireableDistance = 4; //because 4 is the smallest size of this labyrinth game.
        private Location _location;
        int _fireCount = 0;
        private ConsoleColor _displayColor;
        public FireDragon(Location location, ConsoleColor color = ConsoleColor.Blue)
        {
            _location = location;
            _displayColor = color;
        }

        /// <summary>
        /// when activate, the dragon will copy itself to a random room(not active, but can use a destroyed room())
        /// if there are no room can burn
        /// </summary>
        /// <param name="hero">The hero encountering the minotaur.</param>
        /// <param name="map">The current game map.</param>
        public override void Activate(Hero hero, Map map)
        {
            var currentRoom = map.GetRoomAtLocation(_location);
            currentRoom.Destory("Fire");

            //Check the hero already surround by fire.
            if (IsSurroundByFire(hero, map))
            {
                Console.WriteLine("You're surrounded by flames and there's no escape");
                hero.Kill("Burn to died");
            }

            Location? protentialLocation =  GetProtentialLocationForMonster(map);
            if (protentialLocation == null)
            {
                ConsoleHelper.WriteLine("I smell you will be a delicous sausage, I will burn you.", _displayColor);
            }
            else
            {
                currentRoom.RemoveMonster();
                var protentialRoom = map.GetRoomAtLocation(protentialLocation);
                protentialRoom.AddMonster(this);
                _location = protentialLocation;

                var protentialChildLocation =  GetProtentialLocationForMonster(map);
                if (protentialChildLocation == null)
                {
                    Console.WriteLine("There is no available location to put one more dragon.");
                }
                else
                {
                    FireDragon newDragon = new FireDragon(protentialChildLocation, ConsoleColor.Green);
                    var protentialChildRoom = map.GetRoomAtLocation(protentialChildLocation);
                    protentialChildRoom.AddMonster(newDragon);
                    map.AddDestoryables(newDragon);
                    ConsoleHelper.WriteLine("I call another dragon to kill you, you will die soon,", _displayColor);
                }
            }
        }

        //I want to design a feature that the monster can make a fire in the empty room, 
        //so that the hereo cannot enter into these rooms
        public void DestoryRoom(Hero hero, Map map)
        {
            List<Location> fireableLocations = GetFireableLocations(hero, map, _fireableDistance);
            List<Room> protentialToBeDestoryedRooms = new List<Room>();
            foreach (var location in fireableLocations)
            {
                var room = map.GetRoomAtLocation(location);
                if (room.CanBeDestoryed())
                {
                    protentialToBeDestoryedRooms.Add(room);
                }
                else
                {
                    Console.WriteLine($"This room is not available to destory row, cloumn: [{location.Row}, {location.Column}, type: {room.Type}, state: {room.State} Active:{room.IsActive}, Monster:{room._monster}]");
                }
            }

            if (protentialToBeDestoryedRooms.Count == 0)
            {
                ConsoleHelper.WriteLine("No rooms can be destoryed, you're waitting to burn.", ConsoleColor.DarkGreen);
            }
            else
            {
                int index = RandomNumberGenerator.Next(0, protentialToBeDestoryedRooms.Count);
                protentialToBeDestoryedRooms[index].Destory("Fire");

                if (IsSurroundByFire(hero, map))
                {
                    ConsoleHelper.WriteLine("You're surrounded by flames and there's no escape", ConsoleColor.DarkGreen);
                    hero.Kill("You're burn by flames");
                }
                else
                {
                    _fireCount += 1;
                    if (_fireCount % 3 == 0)
                    {
                        //when accumlate enought fire, then can upgrade its level. 
                        LevelUp();
                    }
                }
            }

        }

        //decrease the fireable distance to make the fire more close to hero.
        public void LevelUp() 
        {
            if (_fireableDistance > 1)
            {
                _fireableDistance -= 1;
            }
            //_fireableDistance = Math.Clamp(_fireableDistance - 1, 1, _fireableDistance - 1);
        }

        /// <summary>
        /// Displays sensory information about the minotaur based on the hero's distance from it.
        /// </summary>
        /// <param name="hero">The hero sensing the minotaur.</param>
        /// <param name="heroDistance">The distance between the hero and the minotaur.</param>
        /// <returns>Returns true if a message was displayed; otherwise, false.</returns>
        public override bool DisplaySense(Hero hero, int heroDistance)
        {
            if (heroDistance == 1)
            {
                ConsoleHelper.WriteLine("You hear a sound of glass breaking. The smasher is nearby!", _displayColor);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Displays the current state of the minotaur.
        /// </summary>
        /// <returns>Returns a DisplayDetails object containing the minotaur's display information.</returns>
        public override DisplayDetails Display(bool isOccupyInADestoryedRoom = false)
        {
            if (isOccupyInADestoryedRoom)
                return new DisplayDetails("<D>", _displayColor);
            else
                return new DisplayDetails("[D]", _displayColor);
        }

        public List<Location> GetFireableLocations(Hero hero, Map map, int distance)
        {
            List<Location> locations = new List<Location>();
            for (int row = 0; row < map.Rows; ++row)
            {
                for (int col = 0; col < map.Columns; ++col)
                {
                    if (row == hero.Location.Row && col == hero.Location.Column)
                    {// skip hero current location
                        continue;
                    }
                    int interval = Math.Abs(row - hero.Location.Row) + Math.Abs(col - hero.Location.Column);
                    if (interval <= distance)
                    {
                        locations.Add(new Location(row, col));
                    }
                }
            }
            return locations;
        }

        public Location?  GetProtentialLocationForMonster(Map map)
        {
            List<Location> protentialLocations = new();
            for (int row = 0; row < map.Rows; ++row)
            {
                for (int col = 0; col < map.Columns; ++col)
                {
                    if (row == _location.Row && col == _location.Column)
                    {// skip dragon current location
                        continue;
                    }
                    var location = new Location(row, col);
                    var room = map.GetRoomAtLocation(location);
                    if (room.CanOccupyByMonistor())
                    {
                        protentialLocations.Add(location);
                    }
                }
            }

            if (protentialLocations.Count == 0)
                return null;
            else
            { 
                int index = RandomNumberGenerator.Next(0, protentialLocations.Count);
                return protentialLocations[index];
            }
        }

        private bool IsSurroundByFire(Hero hero, Map map)
        {
            bool isSurroundFires = true;
            List<Room> adjacentRooms = map.GetAdjacentRooms(hero.Location);
            foreach (var room in adjacentRooms)
            {
                if (room.State != RoomState.Destoryed && room.Type != RoomType.Wall)
                {
                    isSurroundFires = false;
                    break;
                }
            }

            return isSurroundFires;
        }
    }
}

