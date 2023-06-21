using System;
namespace MinotaurLabyrinth
{
    public class FireDragon : Monster, IDestoryable
    {
        private int _fireableDistance = 4; //because 4 is the smallest size of this labyrinth game.
        private Location _location;
        int _fireCount = 0;
        public FireDragon(Location location)
        {
            _location = location;
        }

        /// <summary>
        /// </summary>
        /// <param name="hero">The hero encountering the minotaur.</param>
        /// <param name="map">The current game map.</param>
        public override void Activate(Hero hero, Map map)
        {

        }

        //I want to design a feature that the monster can make a fire in the empty room, 
        //so that the hereo cannot enter into these rooms
        public void DestoryRoom(Hero hero, Map map)
        {
            List<Location> fireableLocations = GetFireableLocations(hero, map, _fireableDistance);
            List<Room> emptyRooms = new List<Room>();
            foreach (var location in fireableLocations)
            {
                var room = map.GetRoomAtLocation(location);
                if (room.Type == RoomType.Room && !room.IsActive && room.State != RoomState.Destoryed) //it means this room is empty.
                {
                    emptyRooms.Add(room);
                }
                else
                {
                    Console.WriteLine($"This room is not available to destory row, cloumn: [{location.Row}, {location.Column}, type: {room.Type}, state: {room.State}]");
                }
            }

            int index = RandomNumberGenerator.Next(0, emptyRooms.Count);
            emptyRooms[index].Destory("Fire");
            _fireCount += 1;

        }

        public void CaseSpell() //when accumlate enought fire, then can case a spell. 
        {

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
                ConsoleHelper.WriteLine("You hear growling and stomping. The minotaur is nearby!", ConsoleColor.Blue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Displays the current state of the minotaur.
        /// </summary>
        /// <returns>Returns a DisplayDetails object containing the minotaur's display information.</returns>
        public override DisplayDetails Display()
        {
            return new DisplayDetails("[D]", ConsoleColor.Blue);
        }

        public List<Location> GetFireableLocations(Hero hero, Map map, int distance)
        {
            List<Location> locations = new List<Location>();
            int x = hero.Location.Row;
            int y = hero.Location.Column;

            for (int i = 0; i < map.Rows; ++i)
            {
                for (int j = 0; j < map.Columns; ++j)
                {
                    int interval = Math.Abs(i - x) + Math.Abs(j - y);
                    if (interval < distance)
                    {
                        locations.Add(new Location(i, j));
                    }
                }
            }
            return locations;
        }
    }
}

