using System;
using System.IO;
using System.Text;

namespace Test4giis.Selema.Portable
{
    /// <summary>Asserts for compatibility in translation from JUnit4 to NUnit3 (asserts with messages)</summary>
    public class Asserts
    {
        // Minimum size of a valid recorded video, the shortest videos recorded in CI take several tens of KB
        private const long MinVideoSize = 10000;

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

        /// <summary>Checks that a recorded video file exists, is not empty or truncated and has the mp4 signature
        /// ('ftyp' box at bytes 4-7)</summary>
        public static void AssertVideoFile(string fileName)
        {
            Assert.IsTrue(File.Exists(fileName), "Video file should exist: " + fileName);
            long length = new FileInfo(fileName).Length;
            Assert.IsTrue(length >= MinVideoSize, "Video file should have at least " + MinVideoSize + " bytes, but has " + length + ": " + fileName);
            byte[] header = new byte[8];
            using (FileStream stream = File.OpenRead(fileName))
            {
                int read = 0;
                while (read < header.Length)
                {
                    int count = stream.Read(header, read, header.Length - read);
                    if (count == 0)
                        break;
                    read += count;
                }
            }
            Assert.AreEqual("ftyp", Encoding.ASCII.GetString(header, 4, 4), "Video file should have the mp4 signature: " + fileName);
        }

    }
}
