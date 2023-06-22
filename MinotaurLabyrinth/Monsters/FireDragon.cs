﻿using System;
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

            Location? availableLocation = GetLocationOfNotDestroyed(map);
            if (availableLocation == null)
            {
                ConsoleHelper.WriteLine("I smell you will be a delicous sausage, I will burn you.", ConsoleColor.Red);
            }
            else
            {
                currentRoom.RemoveMonster();
                var availableRoom = map.GetRoomAtLocation(availableLocation);
                availableRoom.AddMonster(this);
                _location = availableLocation;

                availableLocation = GetLocationOfNotDestroyed(map);
                if (availableRoom == null)
                {
                    Console.WriteLine("There is no available location to put one more dragon.");
                }
                else
                {
                    FireDragon newDragon = new FireDragon(availableLocation);
                    availableRoom.AddMonster(newDragon);
                    map.AddDestoryables(newDragon);
                    ConsoleHelper.WriteLine("I call another dragon to kill you, you will die soon,", ConsoleColor.Red);
                }
            }
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
                if (!room.IsActive && room.State != RoomState.Destoryed) //it can destory empty room and the room that are not active.
                {
                    emptyRooms.Add(room);
                }
                else
                {
                    Console.WriteLine($"This room is not available to destory row, cloumn: [{location.Row}, {location.Column}, type: {room.Type}, state: {room.State}]");
                }
            }

            if (emptyRooms.Count == 0)
            {
                ConsoleHelper.WriteLine("No rooms can be destoryed, you're waitting to burn.", ConsoleColor.DarkGreen);
            }
            else
            {
                int index = RandomNumberGenerator.Next(0, emptyRooms.Count);
                emptyRooms[index].Destory("Fire");

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
                        //when accumlate enought fire, then can case a spell. 
                        CaseSpell();
                    }
                }
            }

        }

        //decrease the fireable distance to make the fire more close to hero.
        public void CaseSpell() 
        {
            _fireableDistance = Math.Clamp(_fireableDistance - 1, 1, _fireableDistance - 1);
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

            for (int row = 0; row < map.Rows; ++row)
            {
                for (int col = 0; col < map.Columns; ++col)
                {
                    int interval = Math.Abs(row - x) + Math.Abs(col - y);
                    if (interval < distance)
                    {
                        locations.Add(new Location(row, col));
                    }
                }
            }
            return locations;
        }

        public Location? GetLocationOfNotDestroyed(Map map)
        {
            List<Location> availableLocations = new();
            for (int row = 0; row < map.Rows; ++row)
            {
                for (int col = 0; col < map.Columns; ++col)
                {
                    if (row == _location.Row && col == _location.Column)
                    {// skip the current location
                        continue;
                    }
                    var location = new Location(row, col);
                    var room = map.GetRoomAtLocation(location);
                    if (!room.IsActive)
                    {
                        availableLocations.Add(location);
                    }
                }
            }

            if (availableLocations.Count == 0)
                return null;
            else
            {
                int index = RandomNumberGenerator.Next(0, availableLocations.Count);
                return availableLocations[index];
            }
        }

        private bool IsSurroundByFire(Hero hero, Map map)
        {
            bool isSurroundFires = true;
            List<Room> adjacentRooms = map.GetAdjacentRooms(hero.Location);
            foreach (var room in adjacentRooms)
            {
                if (room.State != RoomState.Destoryed || room.Type != RoomType.Wall || room.Type != RoomType.Entrance)
                {
                    isSurroundFires = false;
                    break;
                }
            }

            return isSurroundFires;
        }
    }
}

