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
            dragonMock.Protected().Setup<List<Location>>("GetFireableLocations", new Object[] { hero, mapMock.Object, 4 }).Returns(mockLocations);



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

            mapMock.CallBase = true; //here I need to call the real method to get the room, and check the room really add a new monster
            method = typeof(Map).GetMethod("GetRoomAtLocation");
            Room realRoom = (Room)method.Invoke(mapMock.Object, new Object[] { location });

            FieldInfo property = typeof(Room).GetField("_monster", BindingFlags.NonPublic | BindingFlags.Instance);
            Monster monsterAdded = (Monster)property.GetValue(realRoom);
            Assert.AreNotEqual(null, monsterAdded);
        }

    }
}