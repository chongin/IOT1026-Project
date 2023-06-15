using System.Reflection;
using MinotaurLabyrinth;
using Moq;
using Moq.Protected;

namespace MinotaurLabyrinthTest
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void DummyTest()
        {
            Assert.AreNotSame(1, 2);
        }


        [TestMethod]
        public void Test_Create_Toxic_Room_ByType()
        {
            Room room = RoomFactory.Instance.BuildRoom(RoomType.Toxic);
            string className = room.GetType().FullName;
            Assert.AreEqual(className, "MinotaurLabyrinth.Toxic");
        }

        [TestMethod]
        public void Test_PlayGame_Win_When_Dice10_And_Small_True()
        {
            var mock = new Mock<Toxic>();
            mock.Protected().Setup<int>("Dice3Dices").Returns(10);
            var toxic = mock.Object;

            bool smallFlag = true;
            MethodInfo playGameMethod = typeof(Toxic).GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)playGameMethod.Invoke(toxic, new object[] { smallFlag });

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Test_PlayGame_Lost_When_Dice10_And_Small_False()
        {
            var mock = new Mock<Toxic>();
            mock.Protected().Setup<int>("Dice3Dices").Returns(10);
            var toxic = mock.Object;

            bool smallFlag = false;
            MethodInfo playGameMethod = typeof(Toxic).GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)playGameMethod.Invoke(toxic, new object[] { smallFlag });

            Assert.AreEqual(result, false);
        }


        [TestMethod]
        public void Test_PlayGame_Lost_When_Dice11_And_Small_True()
        {
            var mock = new Mock<Toxic>();
            mock.Protected().Setup<int>("Dice3Dices").Returns(11);
            var toxic = mock.Object;

            bool smallFlag = true;
            MethodInfo playGameMethod = typeof(Toxic).GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)playGameMethod.Invoke(toxic, new object[] { smallFlag });

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Test_PlayGame_Win_When_Dice11_And_Small_False()
        {
            var mock = new Mock<Toxic>();
            mock.Protected().Setup<int>("Dice3Dices").Returns(11);
            var toxic = mock.Object;
            bool smallFlag = false;

            MethodInfo playGameMethod = typeof(Toxic).GetMethod("PlayGame", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)playGameMethod.Invoke(toxic, new object[] { smallFlag });

            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void Test_Activate_WinGame()
        {
            var mock = new Mock<Toxic>() { CallBase = true }; //need to set Callbase = true, then can call the override/virtual method
            mock.Protected().Setup<int>("Dice3Dices").Returns(11);
            mock.Protected().Setup<bool>("GetUserSelectOption").Returns(false);
            var toxic = mock.Object;

            (Map map, Hero hero) = LabyrinthCreator.InitializeMap(Size.Small);
            toxic.Activate(hero, map);

            mock.Protected().Verify("HandleWinGame", Times.Once());
            Assert.AreEqual(hero.IsPoisoned, false);
        }

        [TestMethod]
        public void Test_Activate_LostGame()
        {
            var mock = new Mock<Toxic>() { CallBase = true }; //need to set Callbase = true, then can call the override/virtual method
            mock.Protected().Setup<int>("Dice3Dices").Returns(11);
            mock.Protected().Setup<bool>("GetUserSelectOption").Returns(true);
            var toxic = mock.Object;

            (Map map, Hero hero) = LabyrinthCreator.InitializeMap(Size.Small);
            toxic.Activate(hero, map);

            mock.Protected().Verify("HandleLostGame", Times.Once(), new object[] { hero });
            Assert.AreEqual(hero.IsPoisoned, true);
        }
    }
}
