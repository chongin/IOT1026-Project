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
    }
}

