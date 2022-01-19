using NUnit.Framework;

namespace Giis.Selema.Framework.Nunit3
{
    /// <summary>
    /// Asserts for compatibility in translation from JUnit4 to NUnit3
    /// </summary>
    public class Asserts
    {
        public static void AssertAreEqual(string expected, string actual, string message)
        {
            Assert.AreEqual(expected, actual, message);
        }
        public static void AssertContains(string value, string substring)
        {
            StringAssert.Contains(substring, value);
        }
        public static void AssertIsTrue(bool condition, string message)
        {
            Assert.IsTrue(condition, message);
        }
    }
}
