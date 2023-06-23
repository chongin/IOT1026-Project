using System.Reflection;
using MinotaurLabyrinth;
using Moq;
using Moq.Protected;

namespace MinotaurLabyrinthTest
{
    [TestClass]
    public class FireDragonTest
    {
        [TestMethod]
        public void TestLevelUp()
        {
            var dragon = new FireDragon(new Location(1, 1));
            MethodInfo method = typeof(FireDragon).GetMethod("LevelUp", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragon, new object[] { });

            PropertyInfo property = typeof(FireDragon).GetProperty("_fireableDistance", BindingFlags.NonPublic | BindingFlags.Instance);
            int val = (int)property.GetValue(dragon);

            Assert.AreEqual(FireDragon.MAX_FIREABLE_DISTANCE - 1, val);

            //loop 100 times
            for (int i = 0; i < 100; i++)
            {
                method.Invoke(dragon, new object[] { });
            }

            val = (int)property.GetValue(dragon);
            Assert.AreEqual(FireDragon.MIN_FIREABLE_DISTANCE, val);
        }

        [TestMethod]
        public void Test_IsSurroundByFire_All_Adjacent_Rooms_Destoryed()
        {
            var dragon = new FireDragon(new Location(1, 1));
            Hero hero = new Hero(new Location(2, 2));
            List<Room> mockAdjacentRooms = new();
            for (int i = 0; i < 4; ++i)
            {
                var room = new Room();
                room.DestoryBy("mock");
                mockAdjacentRooms.Add(room);
            }

            var mapMock = new Mock<Map>(new Object[] { 4, 4 });
            mapMock.Setup<List<Room>>(ins => ins.GetAdjacentRooms(hero.Location)).Returns(mockAdjacentRooms);
            MethodInfo method = typeof(FireDragon).GetMethod("IsSurroundByFire", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)method.Invoke(dragon, new object[] { hero, mapMock.Object });
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Test_IsSurroundByFire_Adjacent_Rooms_Destoryed_And_Has_Wall_Room()
        {
            var dragon = new FireDragon(new Location(1, 1));
            Hero hero = new Hero(new Location(2, 2));
            List<Room> mockAdjacentRooms = new();
            for (int i = 0; i < 3; ++i)
            {
                var room = new Pit();
                room.DestoryBy("mock");
                mockAdjacentRooms.Add(room);
            }

            mockAdjacentRooms.Add(new Wall());

            var mapMock = new Mock<Map>(new Object[] { 4, 4 });
            mapMock.Setup<List<Room>>(ins => ins.GetAdjacentRooms(hero.Location)).Returns(mockAdjacentRooms);
            MethodInfo method = typeof(FireDragon).GetMethod("IsSurroundByFire", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)method.Invoke(dragon, new object[] { hero, mapMock.Object });
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Test_IsSurroundByFire_Adjacent_Rooms_Has_Not_Destoryed()
        {
            var dragon = new FireDragon(new Location(1, 1));
            Hero hero = new Hero(new Location(2, 2));
            List<Room> mockAdjacentRooms = new();
            for (int i = 0; i < 2; ++i)
            {
                var room = new Pit();
                room.DestoryBy("mock");
                mockAdjacentRooms.Add(room);
            }
            mockAdjacentRooms.Add(new Entrance());
            mockAdjacentRooms.Add(new Pit());

            var mapMock = new Mock<Map>(new Object[] { 4, 4 });
            mapMock.Setup<List<Room>>(ins => ins.GetAdjacentRooms(hero.Location)).Returns(mockAdjacentRooms);
            MethodInfo method = typeof(FireDragon).GetMethod("IsSurroundByFire", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)method.Invoke(dragon, new object[] { hero, mapMock.Object });
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_GetProtentialCanBeDestoryedRooms()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(3, 3), ConsoleColor.Cyan });
            dragonMock.CallBase = true; //very important, then it can call the virtual method(exclude the methods that Setup)

            var hero = new Hero(new Location(2, 2));
            var mapMock = new Mock<Map>(new Object[] { 4, 4 });
            List<Location> mockLocations = new();
            for (int i = 0; i < 1; ++i)
            {
                mockLocations.Add(new Location(i, 0));
            }

            ////Test can be Destoryed
            var room = new Room(); //room default can be destoryed
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(mockLocations[0])).Returns(room);

            //MyExperience: ?? When I want to mock GetFireableLocations, it depends on its parameter value to find the collect method. I didn't understand how it works now, need to reasearch
            //for example if change the parameter 4 to 3, then the Setup method will be failed, it throws: System.Reflection.TargetInvocationException
            dragonMock.Protected().Setup<List<Location>>("GetFireableLocations", new Object[] { hero, mapMock.Object, FireDragon.MAX_FIREABLE_DISTANCE }).Returns(mockLocations);

            var method = typeof(FireDragon).GetMethod("GetProtentialCanBeDestoryedRooms", BindingFlags.NonPublic | BindingFlags.Instance);
            var rooms = (List<Room>)method.Invoke(dragonMock.Object, new Object[] { hero, mapMock.Object });
            Assert.AreEqual(1, rooms.Count);

            //Test cannot be Destroyed
            var cannotBeDestoryedRoom = new Room();
            cannotBeDestoryedRoom.DestoryBy("mock");
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(mockLocations[0])).Returns(cannotBeDestoryedRoom);
            var noRooms = (List<Room>)method.Invoke(dragonMock.Object, new object[] { hero, mapMock.Object });
            Assert.AreEqual(0, noRooms.Count);
        }

        [TestMethod]
        public void Test_SwapToNewLocation()
        {
            var dragon = new FireDragon(new Location(1, 1));
            Room currentRoom = new Room();
            currentRoom.AddMonster(dragon);
            var mapMock = new Mock<Map>(new Object[] { 4, 4 });

            Location newLocation = new Location(3, 2);
            Room newRoom = new Room();
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(newLocation)).Returns(newRoom);
            var method = typeof(FireDragon).GetMethod("SwapToNewLocation", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragon, new object[] { mapMock.Object, currentRoom, newLocation });

            //MyExperience: Cannot use GetProperty to get the private property, because it didn't use reflection, so have to use GetField method.
            //PropertyInfo property = typeof(Room).GetProperty("_monster", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo property = typeof(Room).GetField("_monster", BindingFlags.NonPublic | BindingFlags.Instance);
            Monster monsterAdded = (Monster)property.GetValue(newRoom);
            Assert.AreEqual(dragon, monsterAdded);

            Monster monsterRemoved = (Monster)property.GetValue(currentRoom);
            Assert.AreEqual(null, monsterRemoved);

            FieldInfo locationProperty = typeof(FireDragon).GetField("_location", BindingFlags.NonPublic | BindingFlags.Instance);
            Location location = (Location)locationProperty.GetValue(dragon);
            Assert.AreEqual(location, newLocation);
        }

        [TestMethod]
        public void Test_SummonOneMonsterAtLocataion()
        {
            FireDragon dragon = new FireDragon(new Location(1, 1));
            var mapMock = new Mock<Map>(new Object[] { 4, 4 });
            Room newRoom = new Room();
            Location location = new Location(2, 1);
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(location)).Returns(newRoom);

            var method = typeof(FireDragon).GetMethod("SummonOneMonsterAtLocataion", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragon, new Object[] { mapMock.Object, location });

            var smashersProperty = typeof(Map).GetField("_smashers", BindingFlags.NonPublic | BindingFlags.Instance);
            List<ISmasher> smashers = (List<ISmasher>)smashersProperty.GetValue(mapMock.Object);
            Assert.AreEqual(1, smashers.Count);

            //MyExperience: Can set CallbBase later, so I control when will the object mock method or call the real method, but remember the method should be virtual
            mapMock.CallBase = true; //here I need to call the real method to get the room, and check the room really add a new monster
            method = typeof(Map).GetMethod("GetRoomAtLocation");
            Room realRoom = (Room)method.Invoke(mapMock.Object, new Object[] { location });

            FieldInfo property = typeof(Room).GetField("_monster", BindingFlags.NonPublic | BindingFlags.Instance);
            Monster monsterAdded = (Monster)property.GetValue(realRoom);
            Assert.AreNotEqual(null, monsterAdded);
        }

        [TestMethod]
        public void Test_GetProtentialLocationForMonster_Skip_Dragon_Itself_Location()
        {
            FireDragon dragon = new FireDragon(new Location(1, 0));
            Map map = new Map(2, 2);

            Room[,] mockRooms = new Room[2, 2];
            mockRooms[0, 0] = new Toxic(); //cannot occupy by monster
            mockRooms[0, 1] = new Pit();
            mockRooms[1, 0] = new Room(); //can occupy but he location is the same as dragon itself
            mockRooms[1, 1] = new Sword();

            //Overwrite the _rooms data in the map instance.
            FieldInfo roomInfo = typeof(Map).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);
            roomInfo.SetValue(map, mockRooms);

            MethodInfo method = typeof(FireDragon).GetMethod("GetProtentialLocationForMonster", BindingFlags.NonPublic | BindingFlags.Instance);
            Location? location = (Location)method.Invoke(dragon, new Object[] { map });
            Assert.AreEqual(null, location);
        }

        [TestMethod]
        public void Test_GetProtentialLocationForMonster_Return_Specify_Location()
        {
            FireDragon dragon = new FireDragon(new Location(1, 0));
            Map map = new Map(2, 2);

            Room[,] mockRooms = new Room[2, 2];
            mockRooms[0, 0] = new Toxic(); //cannot occupy by monster
            mockRooms[0, 1] = new Room(); //can occupy,check with this value
            mockRooms[1, 0] = new Room(); //can occupy but he location is the same as dragon itself
            mockRooms[1, 1] = new Sword();

            //MyExperence: Can use SetValue to set the private property value.
            //Overwrite the _rooms data in the map instance.
            FieldInfo roomInfo = typeof(Map).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);
            roomInfo.SetValue(map, mockRooms);

            MethodInfo method = typeof(FireDragon).GetMethod("GetProtentialLocationForMonster", BindingFlags.NonPublic | BindingFlags.Instance);
            Location? location = (Location)method.Invoke(dragon, new Object[] { map });
            Assert.AreEqual(0, location.Row);
            Assert.AreEqual(1, location.Column);
        }

        [TestMethod]
        public void Test_GetFireableLocations_Skip_Hero_location()
        {
            FireDragon dragon = new FireDragon(new Location(1, 0));
            Map map = new Map(4, 4);
            Location heroLocation = new Location(1, 1);
            Hero hero = new Hero(heroLocation);

            MethodInfo method = typeof(FireDragon).GetMethod("GetFireableLocations", BindingFlags.NonPublic | BindingFlags.Instance);
            var locations = (List<Location>)method.Invoke(dragon, new Object[] { hero, map, 4 });
            Assert.AreEqual(15, locations.Count);

            bool existHeroLocation = false;
            foreach (var location in locations)
            {
                if (location.Column == heroLocation.Column && location.Row == heroLocation.Row)
                {
                    existHeroLocation = true;
                    break;
                }
            }

            Assert.AreEqual(false, existHeroLocation);
        }

        [TestMethod]
        public void Test_GetFireableLocations_Exclude_Exceed_Distance_Locations()
        {
            FireDragon dragon = new FireDragon(new Location(1, 0));
            Map map = new Map(4, 4);
            Location heroLocation = new Location(1, 1);
            Hero hero = new Hero(heroLocation);

            MethodInfo method = typeof(FireDragon).GetMethod("GetFireableLocations", BindingFlags.NonPublic | BindingFlags.Instance);
            int max_distance = 2;
            var locations = (List<Location>)method.Invoke(dragon, new Object[] { hero, map, max_distance });
            Assert.AreEqual(10, locations.Count);

            bool exceedDistance = false;
            foreach (var location in locations)
            {
                if (
                    (location.Column == 3 && location.Row == 2) ||
                    (location.Column == 3 && location.Row == 3) ||
                    (location.Column == 2 && location.Row == 3) ||
                    (location.Column == 0 && location.Row == 3)
                   )
                {
                    exceedDistance = true;
                    break;
                }
            }

            Assert.AreEqual(false, exceedDistance);
        }

        [TestMethod]
        public void Test_HandleProtentialRoomsCanBeDestoryed_SurroundByFire()
        {
            //MyExperience: even the class contruction function has the default parameter, also need to pass into the Object[], otherwise, cannot create a object.
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(0, 1));
            Map map = new Map(4, 4);

            List<Room> protentialToBeDestoryedRooms = new List<Room>() { new Room() };
            dragonMock.Protected().Setup<bool>("IsSurroundByFire", new Object[] { hero, map }).Returns(true);
            MethodInfo method = typeof(FireDragon).GetMethod("HandleProtentialRoomsCanBeDestoryed", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragonMock.Object, new Object[] { hero, map, protentialToBeDestoryedRooms });
            dragonMock.Protected().Verify("HandleHeroWasSurroundByFire", Times.Once(), new object[] { hero, map });
        }

        [TestMethod]
        public void Test_HandleProtentialRoomsCanBeDestoryed_Not_SurroundByFile()
        {
            //MyExperience: even the class contruction function has the default parameter, also need to pass into the Object[], otherwise, cannot create a object.
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(0, 1));
            Map map = new Map(4, 4);

            List<Room> protentialToBeDestoryedRooms = new List<Room>() { new Room() };
            dragonMock.Protected().Setup<bool>("IsSurroundByFire", new Object[] { hero, map }).Returns(false);
            MethodInfo method = typeof(FireDragon).GetMethod("HandleProtentialRoomsCanBeDestoryed", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragonMock.Object, new Object[] { hero, map, protentialToBeDestoryedRooms });
            dragonMock.Protected().Verify("HandleHeroWasSurroundByFire", Times.Never(), new object[] { hero, map });

            FieldInfo fieldInfo = typeof(FireDragon).GetField("_fireCount", BindingFlags.NonPublic | BindingFlags.Instance);
            int fireCount = (int)fieldInfo.GetValue(dragonMock.Object);

            Assert.AreEqual(1, fireCount);

            //call more 2 times, then test call level up
            method.Invoke(dragonMock.Object, new Object[] { hero, map, protentialToBeDestoryedRooms });
            method.Invoke(dragonMock.Object, new Object[] { hero, map, protentialToBeDestoryedRooms });
            dragonMock.Protected().Verify("LevelUp", Times.Once(), new object[] { });
        }

        [TestMethod]
        public void Test_InteractWithHeroInRoom_No_Protential_Location_For_Monster()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);
            Room room = new();

            //MyExperience: When to mock a function to return nullable object, for example: Location?, you should define it first like below, cannot .Returns(null)
            Location nullLocation = null;
            dragonMock.Protected().Setup<Location>("GetProtentialLocationForMonster", new Object[] { map }).Returns(nullLocation);
            MethodInfo method = typeof(FireDragon).GetMethod("InteractWithHeroInRoom", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(dragonMock.Object, new Object[] { room, hero, map });
            dragonMock.Protected().Verify("SwapToNewLocation", Times.Never(), new Object[] { map, room, new Location(3, 3) });
        }

        [TestMethod]
        public void Test_InteractWithHeroInRoom_Has_Protential_Location()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);
            Room room = new();

            //MyExperience: When to mock a function to return nullable object, for example: Location?, you should define it first like below, cannot .Returns(null)
            Location location = new(1, 1);
            dragonMock.Protected().Setup<Location>("GetProtentialLocationForMonster", new Object[] { map }).Returns(location);

            MethodInfo method = typeof(FireDragon).GetMethod("InteractWithHeroInRoom", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(dragonMock.Object, new Object[] { room, hero, map });
            dragonMock.Protected().Verify("SwapToNewLocation", Times.Once(), new Object[] { map, room, location });
            dragonMock.Protected().Verify("SummonChildMonster", Times.Once(), new Object[] { map });
        }

        [TestMethod]
        public void Test_SummonChildMonster_No_Protential_Location_For_Monster()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);
            Room room = new();

            //MyExperience: When to mock a function to return nullable object, for example: Location?, you should define it first like below, cannot .Returns(null)
            Location nullLocation = null;
            dragonMock.Protected().Setup<Location>("GetProtentialLocationForMonster", new Object[] { map }).Returns(nullLocation);
            MethodInfo method = typeof(FireDragon).GetMethod("SummonChildMonster", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(dragonMock.Object, new Object[] { map });
            dragonMock.Protected().Verify("SummonOneMonsterAtLocataion", Times.Never(), new Object[] { map, new Location(3, 3) });
        }

        [TestMethod]
        public void Test_SummonChildMonster_Has_Protential_Location_For_Monster()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);
            Room room = new();

            //MyExperience: When to mock a function to return nullable object, for example: Location?, you should define it first like below, cannot .Returns(null)
            Location location = new Location(1, 1);
            dragonMock.Protected().Setup<Location>("GetProtentialLocationForMonster", new Object[] { map }).Returns(location);
            MethodInfo method = typeof(FireDragon).GetMethod("SummonChildMonster", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(dragonMock.Object, new Object[] { map });
            dragonMock.Protected().Verify("SummonOneMonsterAtLocataion", Times.Once(), new Object[] { map, location });
        }

        [TestMethod]
        public void Test_DestoryRoom_No_Protential_Room_Can_Be_Destroyed()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);

            List<Room> protentialRooms = new();
            dragonMock.Protected().Setup<List<Room>>("GetProtentialCanBeDestoryedRooms", new Object[] { hero, map }).Returns(protentialRooms);

            dragonMock.Object.DestoryRoom(hero, map);
            dragonMock.Protected().Verify("HandleNoProtentialRoomCanBeDestoryed", Times.Once(), new Object[] { });
        }

        [TestMethod]
        public void Test_DestoryRoom_To_Handle_Destroyed_Room()
        {
            var dragonMock = new Mock<FireDragon>(new Object[] { new Location(1, 2), ConsoleColor.Cyan }) { CallBase = true };
            Hero hero = new Hero(new Location(1, 0));
            Map map = new Map(4, 4);

            List<Room> protentialRooms = new() { new Room() };
            dragonMock.Protected().Setup<List<Room>>("GetProtentialCanBeDestoryedRooms", new Object[] { hero, map }).Returns(protentialRooms);

            dragonMock.Object.DestoryRoom(hero, map);
            dragonMock.Protected().Verify("HandleNoProtentialRoomCanBeDestoryed", Times.Never(), new Object[] { });
            dragonMock.Protected().Verify("HandleProtentialRoomsCanBeDestoryed", Times.Once(), new Object[] { hero, map, protentialRooms });
        }

        [TestMethod]
        public void Test_HandleHeroWasSurroundByFire()
        {
            var dragon = new FireDragon(new Location(1, 1));
            var hero = new Hero(new Location(2, 2));
            var map = new Map(4, 4);

            MethodInfo method = typeof(FireDragon).GetMethod("HandleHeroWasSurroundByFire", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(dragon, new Object[] { hero, map });

            Assert.AreEqual(false, hero.IsAlive);
        }

        [TestMethod]
        public void Test_DestoryDragonItSelfRoom()
        {
            var dragon = new FireDragon(new Location(1, 1));
            var map = new Map(4, 4);
            var method = typeof(FireDragon).GetMethod("DestoryDragonItSelfRoom", BindingFlags.NonPublic | BindingFlags.Instance);
            var room = (Room)method.Invoke(dragon, new Object[] { map });
            Assert.AreEqual(RoomState.Destoryed, room.State);
        }

        [TestMethod]
        public void Test_Activate_HandleHeroWasSurroundByFire()
        {
            var dragonMock = new Mock<FireDragon>(new Location(1, 1), ConsoleColor.Cyan);
            dragonMock.CallBase = true;
            Hero hero = new(new Location(1, 1));
            Map map = new(4, 4);

            var room = new Room();
            dragonMock.Protected().Setup<bool>("IsSurroundByFire", new Object[] { hero, map }).Returns(true);
            dragonMock.Protected().Setup<Room>("DestoryDragonItSelfRoom", new Object[] { map }).Returns(room);

            dragonMock.Object.Activate(hero, map);
            dragonMock.Protected().Verify("HandleHeroWasSurroundByFire", Times.Once(), new Object[] { hero, map });
            dragonMock.Protected().Verify("InteractWithHeroInRoom", Times.Never(), new Object[] { room, hero, map });
        }

        [TestMethod]
        public void Test_Activate_InteractWithHeroInRoom()
        {
            var dragonMock = new Mock<FireDragon>(new Location(1, 1), ConsoleColor.Cyan);
            dragonMock.CallBase = true;
            Hero hero = new(new Location(1, 1));
            Map map = new(4, 4);

            var room = new Room();
            dragonMock.Protected().Setup<bool>("IsSurroundByFire", new Object[] { hero, map }).Returns(false);
            dragonMock.Protected().Setup<Room>("DestoryDragonItSelfRoom", new Object[] { map }).Returns(room);

            dragonMock.Object.Activate(hero, map);
            dragonMock.Protected().Verify("HandleHeroWasSurroundByFire", Times.Never(), new Object[] { hero, map });
            dragonMock.Protected().Verify("InteractWithHeroInRoom", Times.Once(), new Object[] { room, hero, map });
        }

        [TestMethod]
        public void Test_Display()
        {
            var dragon = new FireDragon(new Location(1, 1));
            DisplayDetails detail = dragon.Display(false);
            Assert.AreEqual(detail.Text, "[D]");

            DisplayDetails detail_1 = dragon.Display(true);
            Assert.AreEqual(detail_1.Text, "<D>");
        }
    }
}
