using System.Reflection;
using MinotaurLabyrinth;
using Moq;
using Moq.Protected;

namespace MinotaurLabyrinthTest
{
    [TestClass]
    public class RoomTest
    {
        [TestMethod]
        public void Test_Destory()
        {
            Room room = new Room();
            room.DestoryBy("Fire");
            Assert.AreEqual(RoomState.Destoryed, room.State);
        }

        [TestMethod]
        public void Test_CanOccupyByMonster_WithMonster()
        {
            Room room = new Room();
            room.AddMonster(new FireDragon(new Location(1, 1)));
            Assert.AreEqual(false, room.CanOccupyByMonster());
        }

        [TestMethod]
        public void Test_CanOccupyByMonster_WithoutMonster_Room()
        {
            Room room = new Room();
            Assert.AreEqual(true, room.CanOccupyByMonster());
        }

        [TestMethod]
        public void Test_CanOccupyByMonster_WithoutMonster_Wall()
        {
            Room room = new Wall();
            Assert.AreEqual(false, room.CanOccupyByMonster());
        }

        [TestMethod]
        public void Test_CanOccupyByMonster_WithoutMonster_FeatureRoom_Active()
        {
            Room room = new Toxic();
            Assert.AreEqual(false, room.CanOccupyByMonster());
        }

        [TestMethod]
        public void Test_CanOccupyByMonster_WithoutMonster_FeatureRoom_Not_Active()
        {
            Room room = new Toxic();
            PropertyInfo property = typeof(Room).GetProperty("IsActive");
            property.SetValue(room, false);
            Assert.AreEqual(true, room.CanOccupyByMonster());
        }

        [TestMethod]
        public void Test_CanBeDestoryed_Already_Destoryed()
        {
            Room room = new Room();
            room.DestoryBy("Fire");
            Assert.AreEqual(false, room.CanBeDestoryed());
        }

        [TestMethod]
        public void Test_CanBeDestoryed_Not_Destoryed_Room()
        {
            Room room = new Room();
            Assert.AreEqual(true, room.CanBeDestoryed());
        }

        [TestMethod]
        public void Test_CanBeDestoryed_Not_Destoryed_Wall()
        {
            Room room = new Wall();
            Assert.AreEqual(false, room.CanBeDestoryed());
        }

        [TestMethod]
        public void Test_CanBeDestoryed_Not_Destoryed_FeatureRoom_Active()
        {
            Room room = new Pit();
            Assert.AreEqual(false, room.CanBeDestoryed());
        }

        [TestMethod]
        public void Test_CanBeDestoryed_Not_Destoryed_FeatureRoom_Not_Active()
        {
            Room room = new Pit();
            PropertyInfo property = typeof(Room).GetProperty("IsActive");
            property.SetValue(room, false);
            Assert.AreEqual(true, room.CanBeDestoryed());
        }

        [TestMethod]
        public void Test_IsDestoryed()
        {
            Room room = new Room();
            room.DestoryBy("Fire");
            Assert.AreEqual(true, room.IsDestoryed());
        }
    }
}
