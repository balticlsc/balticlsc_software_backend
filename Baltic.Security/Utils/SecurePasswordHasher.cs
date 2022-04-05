using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Baltic.Security.Utils
{
    public static class SecurePasswordHasher
    {
        private const int DefaultVersion = 1;
        
        private const int DefaultIterations = 10017;
        private const int SizeOfVersion = sizeof(short);
        private const int SizeOfIterationCount = sizeof(int);

        private const int IndexVersion = 0;
        private const int IndexIteration = SizeOfVersion;
        private const int IndexSalt = IndexIteration + SizeOfIterationCount;

        private class HashVersion
        {
            public short Version { get; set; }
            public int SaltSize { get; set; }
            public int HashSize { get; set; }
            public KeyDerivationPrf KeyDerivation { get; set; }
        }

        private static readonly Dictionary<short, HashVersion> Versions = new Dictionary<short, HashVersion>
        {
            {
                1, new HashVersion
                {
                    Version = 1,
                    KeyDerivation = KeyDerivationPrf.HMACSHA512,
                    HashSize = 256 / 8,
                    SaltSize = 128 / 8
                }
            }
        };

        //private static HashVersion DefaultVersion => Versions[1];

        private static bool IsLatestHashVersion(byte[] data)
        {
            var version = BitConverter.ToInt16(data, 0);
            var usedVersion = Versions[DefaultVersion].Version;

            return version == usedVersion;
        }

        public static bool IsLatestHashVersion(string data)
        {
            var dataBytes = Convert.FromBase64String(data);
            return IsLatestHashVersion(dataBytes);
        }

        private static byte[] GetRandomBytes(int length)
        {
            var data = new byte[length];

            using
                var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(data);

            return data;
        }

        public static int GetHashSize()
        {
            var currentVersion = Versions[DefaultVersion];
            const double bitSizeOfByte = 8;
            const double bitSizeOfBase64Char = 6;
                
            return (int)Math.Ceiling(((SizeOfVersion + SizeOfIterationCount + currentVersion.SaltSize + currentVersion.HashSize) * bitSizeOfByte) / bitSizeOfBase64Char);
        }

        public static byte[] Hash(string clearText, int iterations = DefaultIterations)
        {
            // get current version
            var currentVersion = Versions[DefaultVersion];

            // get the byte arrays of the hash and meta information
            var saltBytes = GetRandomBytes(currentVersion.SaltSize);
            var versionBytes = BitConverter.GetBytes(currentVersion.Version);
            var iterationBytes = BitConverter.GetBytes(iterations);
            var hashBytes = KeyDerivation.Pbkdf2(clearText, saltBytes, currentVersion.KeyDerivation, iterations, currentVersion.HashSize);

            // calculate the indexes for the combined hash
            var indexHash = IndexSalt + currentVersion.SaltSize;

            // combine all data to one result hash
            var resultBytes = new byte[SizeOfVersion + SizeOfIterationCount + currentVersion.SaltSize + currentVersion.HashSize];
            Array.Copy(versionBytes, 0, resultBytes, IndexVersion, SizeOfVersion);
            Array.Copy(iterationBytes, 0, resultBytes, IndexIteration, SizeOfIterationCount);
            Array.Copy(saltBytes, 0, resultBytes, IndexSalt, currentVersion.SaltSize);
            Array.Copy(hashBytes, 0, resultBytes, indexHash, currentVersion.HashSize);
            return resultBytes;
        }

        public static string HashToString(string clearText, int iterations = DefaultIterations)
        {
            var data = Hash(clearText, iterations);
            return Convert.ToBase64String(data);
        }

        public static bool Verify(string clearText, byte[] data)
        {
            // Get the current version and number of iterations
            var currentVersion = Versions[BitConverter.ToInt16(data, 0)];
            var iteration = BitConverter.ToInt32(data, 2);

            // Create the byte arrays for the salt and hash
            var saltBytes = new byte[currentVersion.SaltSize];
            var hashBytes = new byte[currentVersion.HashSize];

            // Calculate the indexes of the salt and the hash
            var indexHash = IndexSalt + currentVersion.SaltSize;

            // Fill the byte arrays with salt and hash
            Array.Copy(data, IndexSalt, saltBytes, 0, currentVersion.SaltSize);
            Array.Copy(data, indexHash, hashBytes, 0, currentVersion.HashSize);

            // Hash the current clearText with the parameters given via the data
            var verificationHashBytes = KeyDerivation.Pbkdf2(clearText, saltBytes, currentVersion.KeyDerivation, iteration, currentVersion.HashSize);

            // Check if generated hashes are equal
            return hashBytes.SequenceEqual(verificationHashBytes);
        }

        public static bool Verify(string clearText, string data)
        {
            var dataBytes = Convert.FromBase64String(data);
            return Verify(clearText, dataBytes);
        }
    }
}