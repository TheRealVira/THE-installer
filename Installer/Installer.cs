#region License

// Copyright (c) 2017, Vira
// All rights reserved.
// Solution: Installer
// Project: Installer
// Filename: Installer.cs
// Date - created:2017.01.05 - 09:07
// Date - current: 2017.01.05 - 12:43

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace Installer
{
    public static class Installer
    {
        /// <summary>
        ///     This file is going to save lots of time when it comes to checking if the installation went successful.
        /// </summary>
        private const string CHECKER = "SHIP_CK";

        /// <summary>
        ///     "Copies" the files from the source to the final destination.
        /// </summary>
        /// <param name="source">The source destination from where to copy from.</param>
        /// <param name="final">The final destination from where to cioy to.</param>
        /// <param name="replace">true: Replaces existing files; fales: skips existing files</param>
        /// <param name="output">true: prints the copied parts over the console output stream</param>
        /// <returns>null: successfully copied the files; Exception type: there was an error;</returns>
        public static Exception Install(string source, string final, bool replace = false, bool output = false)
        {
            // Thx to: http://stackoverflow.com/a/3822913

            if (Directory.Exists(final) && !replace)
            {
                if (output)
                {
                    Console.WriteLine(
                        "The directory currently exists! If there is a problem, we'd suggest the repairing option!");

                    return null;
                }
            }

            try
            {
                if (File.Exists(source + "\\" + CHECKER))
                {
                    File.Delete(source + "\\" + CHECKER);
                }

                if (File.Exists(final + "\\" + CHECKER))
                {
                    File.Delete(final + "\\" + CHECKER);
                }

                // Create the final direcotry (does not do anything when it currently exists!)
                Directory.CreateDirectory(final);

                // Creating all necessary directories at the final destination
                Directory.GetDirectories(source,
                    "*",
                    SearchOption.AllDirectories) // For all directories
                    .ToList().ForEach(dirPath =>
                    {
                        Directory.CreateDirectory(dirPath.Replace(source, final));
                        // Create a new directory at the final destination

                        // Print it to the console output stream if needed
                        if (output)
                        {
                            Console.WriteLine(dirPath);
                        }
                    });

                // Copying all necassary files from the source to the final destination (inside the created directories).
                Directory.GetFiles(source,
                    "*.*",
                    SearchOption.AllDirectories)
                    // Foreach files in all directories inside the sourcedirectory (root directry included)...
                    .Where(x => !Path.GetFileName(x).Equals(CHECKER))
                    . // Where the file is not equal to the checking file...
                    ToList().ForEach(newPath =>
                    {
                        // Print it to the console output stream if needed
                        if (output && !File.Exists(newPath.Replace(source, final)))
                        {
                            Console.WriteLine(newPath.Replace(source, final));
                        }

                        File.Copy(newPath, newPath.Replace(source, final), replace);
                        // Copy the file to the final destination.
                    });

                // If the checking routine failes -> rais an exception.
                var failes = CheckingRoutine(source, final);
                if (replace)
                {
                    failes.ToList().ForEach(x => File.Delete(final + "\\" + x));
                }

                failes = Check(source, final, output);

                if (failes.Count > 0)
                {
                    if (!output) throw new Exception("Checkingroutine failed...");

                    Console.WriteLine("\n===============\nThere was an error at the file(s):");
                    failes.ToList().ForEach(Console.WriteLine);

                    throw new Exception("Checkingroutine failed...");
                }
            }
            catch (Exception ex)
            {
                return ex; // There was an error thrown, so I give my duty and return the errormessage to the devs...
            }

            // All wen great! Successfully copied all files and directories.
            return null;
        }

        /// <summary>
        ///     Compares two directories if they are equal. (If their MD5 value is equal)
        /// </summary>
        /// <param name="source">The first directory.</param>
        /// <param name="final">The second directory.</param>
        /// <returns>Returns all missmatches</returns>
        private static HashSet<string> CheckingRoutine(string source, string final)
        {
            // Create the checking hash for the source files:
            var finHash = GetHashesForDir(final);

            // Save it into an checking file:
            finHash.Save(final + "\\" + CHECKER);

            var sourceHash = GetHashesForDir(source); // The source hash

            // Save it into an checking file:
            sourceHash.Save(source + "\\" + CHECKER);

            // Now check if the HashValue of the source directory equals the HashValue fo the final destination directory:
            return CheckSequence(sourceHash, finHash);
        }

        /// <summary>
        ///     Generates a dictionary containing every hash value for every file inside a directory.
        /// </summary>
        /// <param name="directory">The directory where you want to get the values from.</param>
        /// <param name="excludeChecker">true: excludes the CHECK file</param>
        /// <returns>key: "\"+elative file path; value: MD5 value</returns>
        private static Dictionary<string, byte[]> GetHashesForDir(string directory, bool excludeChecker = true)
        {
            if (!Directory.Exists(directory))
            {
                return new Dictionary<string, byte[]>();
            }

            var toRet = new Dictionary<string, byte[]>();

            Directory.GetFiles(directory,
                "*.*",
                SearchOption.AllDirectories)
                // Foreach files in all directories inside the sourcedirectory (root directry included)...
                .Where(x => !Path.GetFileName(x).Equals(CHECKER) || excludeChecker)
                . // Where the file is not equal to the checking file...
                ToList().ForEach(x => toRet.Add(x.Remove(0, directory.Length), HashManager.CreateHashForFile(x)));

            return toRet;
        }

        /// <summary>
        ///     Checks if the checker files are equal.
        /// </summary>
        /// <param name="source">The source directory.</param>
        /// <param name="final">The final destination directory.</param>
        /// <param name="output">true: prints the output on the console output stream.</param>
        /// <returns>A list of all incorrect files.</returns>
        public static HashSet<string> Check(string source, string final, bool output)
        {
            var sD = GetHashesForDir(source, false);
            var fD = GetHashesForDir(final, false);

            return CheckSequence(sD, fD);
        }

        /// <summary>
        ///     Checks if the dictionaries are the same. (I CANNOT use the 'Explicit' given by LINQ, because I am comparing two
        ///     byte arrays!)
        /// </summary>
        /// <returns>Returns all differences of those two dictionary inside a HashSet.</returns>
        private static HashSet<string> CheckSequence(Dictionary<string, byte[]> first,
            Dictionary<string, byte[]> second)
            =>
                new HashSet<string>(
                    first.Where(x => !second.ContainsKey(x.Key) || !x.Value.SequenceEqual(second[x.Key]))
                        .Concat(
                            second.Where(x => !first.ContainsKey(x.Key) || !x.Value.SequenceEqual(first[x.Key])))
                        .Select(x => x.Key));
    }
}