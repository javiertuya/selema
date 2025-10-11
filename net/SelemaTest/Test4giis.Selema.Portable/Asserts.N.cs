using System;

namespace Test4giis.Selema.Portable
{
    /// <summary>Asserts for compatibility in translation from JUnit4 to NUnit3 (asserts with messages)</summary>
    public class Asserts
    {
        private Asserts()
        {
            throw new InvalidOperationException("Utility class");
        }

        public static void AssertAreEqual(string expected, string actual, string message)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void AssertIsTrue(bool condition, string message)
        {
            Assert.IsTrue(condition, message);
        }

        public static void AssertContains(string expectedSubstring, string actual)
        {
            Assert.IsTrue(actual.Contains(expectedSubstring), "Expected substring should be contained in actual: " + actual);
        }

    }
}
