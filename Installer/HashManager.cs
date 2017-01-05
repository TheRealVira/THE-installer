#region License

// Copyright (c) 2017, Vira
// All rights reserved.
// Solution: Installer
// Project: Installer
// Filename: HashManager.cs
// Date - created:2017.01.05 - 09:04
// Date - current: 2017.01.05 - 12:43

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;

#endregion

namespace Installer
{
    internal static class HashManager
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Item[]),
            new XmlRootAttribute {ElementName = "items"});

        /// <summary>
        ///     Creates the MD5 hash for a single file.
        /// </summary>
        /// <param name="file">The source file for the MD5 hash.</param>
        /// <returns>Returns the generated MD5 hash-</returns>
        public static byte[] CreateHashForFile(string file, MD5 md5 = null)
        {
            md5 = md5 ?? MD5.Create(); // Create a new one if there is no one given over parameter.

            using (var reader = File.OpenRead(file))
            {
                md5.ComputeHash(reader);

                reader.Flush();
                reader.Close();
                reader.Dispose();
            }

            return md5.Hash;
        }

        /// <summary>
        ///     This will save the your byte array into a file.
        /// </summary>
        /// <param name="data">Your data that you want to save inside that file.</param>
        /// <param name="destination">The destination file.</param>
        public static void Save(this Dictionary<string, byte[]> data, string destination)
        {
            using (var writer = new StreamWriter(destination))
            {
                Serializer.Serialize(writer, data.Select(x => new Item {id = x.Key, value = x.Value}).ToArray());

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        ///     Loads your byte array from a given file (destination).
        /// </summary>
        /// <param name="destination">The file destination.</param>
        /// <returns>Returns your loaded byte array.</returns>
        public static Dictionary<string, byte[]> Load(this string destination)
        {
            Dictionary<string, byte[]> toRet;
            using (var reader = new StreamReader(destination))
            {
                toRet = ((Item[]) Serializer.Deserialize(reader))
                    .ToDictionary(i => i.id, i => i.value);

                reader.Close();
                reader.Dispose();
            }

            return toRet;
        }
    }
}