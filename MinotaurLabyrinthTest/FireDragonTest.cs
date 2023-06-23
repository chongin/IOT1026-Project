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
            method.Invoke(dragon, new object[] {  });

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
            Hero hero = new Hero(new Location(2,2));
            List<Room> mockAdjacentRooms = new();
            for (int i = 0; i < 4; ++i)
            {
                var room = new Room();
                room.DestoryBy("mock");
                mockAdjacentRooms.Add(room);
            }

            var mapMock = new Mock<Map>(4, 4);    
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

            var mapMock = new Mock<Map>(4, 4);
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

            var mapMock = new Mock<Map>(4, 4);
            mapMock.Setup<List<Room>>(ins => ins.GetAdjacentRooms(hero.Location)).Returns(mockAdjacentRooms);
            MethodInfo method = typeof(FireDragon).GetMethod("IsSurroundByFire", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)method.Invoke(dragon, new object[] { hero, mapMock.Object });
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_GetProtentialCanBeDestoryedRooms()
        {
            var dragon = new FireDragon(new Location(3, 3));
            var hero = new Hero(new Location(2, 2));
            var mapMock = new Mock<Map>(4, 4);

            List<Location> mockLocations = new();
            for (int i = 0; i < 1; ++i)
            {
                mockLocations.Add(new Location(i, 0));
            }

            //Test can be Destoryed
            var room = new Room(); //room default can be destoryed
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(mockLocations[0])).Returns(room);
            MethodInfo method = typeof(FireDragon).GetMethod("GetProtentialCanBeDestoryedRooms", BindingFlags.NonPublic | BindingFlags.Instance);
            var rooms = (List<Room>)method.Invoke(dragon, new object[] { hero, mapMock.Object, mockLocations });
            Assert.AreEqual(1, rooms.Count);

            //Test cannot be Destroyed
            var cannotBeDestoryedRoom = new Room();
            cannotBeDestoryedRoom.DestoryBy("mock");
            mapMock.Setup<Room>(ins => ins.GetRoomAtLocation(mockLocations[0])).Returns(cannotBeDestoryedRoom);
            var noRooms = (List<Room>)method.Invoke(dragon, new object[] { hero, mapMock.Object, mockLocations });
            Assert.AreEqual(0, noRooms.Count);
        }
    }
}

