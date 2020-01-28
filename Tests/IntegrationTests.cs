using System;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;
using SimpleArchiver;
using SimpleArchiver.Models;

namespace Tests
{
    public class IntegrationTests
    {
        private readonly string inputFileName = Guid.NewGuid().ToString();
        private readonly string compressedFileName = Guid.NewGuid().ToString();
        private readonly string decompressedFileName = Guid.NewGuid().ToString();
        private readonly int fileSize = 1024 * 1024 * 128;

        [Test]
        public void CompressAndDecompressFile()
        {
            // Given
            var inputFile = File.Create(inputFileName);

            for (int i = 0; i < fileSize; i++)
            {
                inputFile.WriteByte(1);
            }

            inputFile.Close();
            var compressArgs = new string[3] { ArchiverOperation.Compress.ToString(), inputFileName, compressedFileName };
            var decompressArgs = new string[3] { ArchiverOperation.Decompress.ToString(), compressedFileName, decompressedFileName };

            // When
            Assert.AreEqual(Program.Main(compressArgs), 0);
            Assert.AreEqual(Program.Main(decompressArgs), 0);

            // Then
            Assert.AreEqual(GetFileHash(inputFileName), GetFileHash(decompressedFileName));
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(inputFileName)) File.Delete(inputFileName);
            if (File.Exists(compressedFileName)) File.Delete(compressedFileName);
            if (File.Exists(decompressedFileName)) File.Delete(decompressedFileName);
        }

        private string GetFileHash(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}