using System;
using System.Reflection;

namespace MinotaurLabyrinth
{
    public class FireDragon : Monster, ISmasher
    {
        public const int MIN_FIREABLE_DISTANCE = 2;
        public const int MAX_FIREABLE_DISTANCE = 5;

        private int _fireableDistance { get; set; } //MyExperience: here is use reflection, so it can use GetProperty method in test case
        private Location _location;
        private int _fireCount = 0;
        private ConsoleColor _displayColor;
        public FireDragon(Location location, ConsoleColor color = ConsoleColor.Cyan)
        {
            _location = location;
            _displayColor = color;
            _fireableDistance = MAX_FIREABLE_DISTANCE;
        }

        /// <summary>
        /// when activate, the dragon will copy itself to a random room(not active, but can use a destroyed room())
        /// if there are no room can burn
        /// </summary>
        /// <param name="hero">The hero encountering the minotaur.</param>
        /// <param name="map">The current game map.</param>
        public override void Activate(Hero hero, Map map)
        {
            Room currentRoom = DestoryDragonItSelfRoom(map);

            //Check the hero already surround by fire.
            if (IsSurroundByFire(hero, map))
            {
                HandleHeroWasSurroundByFire(hero, map);
                return;
            }

            InteractWithHeroInRoom(currentRoom, hero, map);
        }

        //I want to design a feature that the monster can make a fire in the empty room, 
        //so that the hereo cannot enter into these rooms
        public void DestoryRoom(Hero hero, Map map)
        {
            var protentialCanBeDestoryedRooms = GetProtentialCanBeDestoryedRooms(hero, map);

            if (protentialCanBeDestoryedRooms.Count == 0)
            {
                HandleNoProtentialRoomCanBeDestoryed();
            }
            else
            {
                HandleProtentialRoomsCanBeDestoryed(hero, map, protentialCanBeDestoryedRooms);
            }
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
                ConsoleHelper.WriteLine("You seem to hear the roar of the dragon!", _displayColor);
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

        protected virtual List<Location> GetFireableLocations(Hero hero, Map map, int distance)
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

        protected virtual Location? GetProtentialLocationForMonster(Map map)
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
                    if (room.CanOccupyByMonster())
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

        protected virtual bool IsSurroundByFire(Hero hero, Map map)
        {
            bool isSurroundFire = true;
            List<Room> adjacentRooms = map.GetAdjacentRooms(hero.Location);
            foreach (var room in adjacentRooms)
            {
                if (!room.IsDestoryed() && room.Type != RoomType.Wall)
                {
                    isSurroundFire = false;
                    break;
                }
            }

            return isSurroundFire;
        }

        //decrease the fireable distance to make the fire more close to hero.
        protected virtual void LevelUp()
        {
            if (_fireableDistance > MIN_FIREABLE_DISTANCE)
            {
                _fireableDistance -= 1;
            }
            //_fireableDistance = Math.Clamp(_fireableDistance - 1, 1, _fireableDistance - 1);
        }

        protected virtual List<Room> GetProtentialCanBeDestoryedRooms(Hero hero, Map map)
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
                    //Console.WriteLine($"This room is not available to destory row, cloumn: [{location.Row}, {location.Column}, type: {room.Type}, state: {room.State} Active:{room.IsActive}, Monster:{room._monster}]");
                }
            }

            return protentialToBeDestoryedRooms;
        }

        protected virtual void HandleNoProtentialRoomCanBeDestoryed()
        {
            ConsoleHelper.WriteLine("A dragon just waits quietly for you to be burnt.", _displayColor);
        }

        protected virtual void HandleProtentialRoomsCanBeDestoryed(Hero hero, Map map, List<Room> protentialToBeDestoryedRooms)
        {
            Room room = GetOneProtentialRoomCanBeDestoryed(protentialToBeDestoryedRooms);
            room.DestoryBy("Fire");
            ConsoleHelper.WriteLine("The labyrinth shook a little bit, as if something had collapsed", _displayColor);

            if (IsSurroundByFire(hero, map))
            {
                HandleHeroWasSurroundByFire(hero, map);
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

        protected virtual Room GetOneProtentialRoomCanBeDestoryed(List<Room> protentialToBeDestoryedRooms)
        {
            int index = RandomNumberGenerator.Next(0, protentialToBeDestoryedRooms.Count);
            return protentialToBeDestoryedRooms[index];
        }


        protected virtual void HandleHeroWasSurroundByFire(Hero hero, Map map)
        {
            ConsoleHelper.WriteLine("There is a voice says you're surrounded by flames and there's no escape.", _displayColor);
            hero.Kill("You're burnt by flames");
        }

        protected virtual void InteractWithHeroInRoom(Room currentRoom, Hero hero, Map map)
        {
            Location? protentialLocation = GetProtentialLocationForMonster(map);
            if (protentialLocation == null)
            {
                ConsoleHelper.WriteLine("There is a voice says I smell you would be a delicious sausage, I'd burn you.", _displayColor);
            }
            else
            {
                SwapToNewLocation(map, currentRoom, protentialLocation);
                SummonChildMonster(map);
            }
        }

        protected virtual void SummonChildMonster(Map map)
        {
            var protentialChildLocation = GetProtentialLocationForMonster(map);
            if (protentialChildLocation == null)
            {
                ConsoleHelper.WriteLine("Even if I don't need to summon another dragon, you'll be burned to death in no time.", _displayColor);
            }
            else
            {
                SummonOneMonsterAtLocataion(map, protentialChildLocation);
            }
        }
        protected virtual void SwapToNewLocation(Map map, Room currentRoom, Location newLocation)
        {
            currentRoom.RemoveMonster();
            var newRoom = map.GetRoomAtLocation(newLocation);
            newRoom.AddMonster(this);
            _location = newLocation;
        }

        protected virtual void SummonOneMonsterAtLocataion(Map map, Location locaton)
        {
            FireDragon newDragon = new FireDragon(locaton, ConsoleColor.DarkBlue);
            var room = map.GetRoomAtLocation(locaton);
            room.AddMonster(newDragon);
            map.AddSmashers(newDragon);
            ConsoleHelper.WriteLine("The map shook violently, as if another monster had appeared.", _displayColor);
        }

        protected virtual Room DestoryDragonItSelfRoom(Map map)
        {
            var currentRoom = map.GetRoomAtLocation(_location);
            currentRoom.DestoryBy("Fire");
            return currentRoom;
        }
    }
}

