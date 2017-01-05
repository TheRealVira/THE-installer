#region License

// Copyright (c) 2017, Vira
// All rights reserved.
// Solution: Installer
// Project: Testing
// Filename: Program.cs
// Date - created:2017.01.05 - 10:08
// Date - current: 2017.01.05 - 12:43

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace Testing
{
    internal class Program
    {
        /// <summary>
        ///     Represents the list of actions binded on keys.
        /// </summary>
        private static readonly Dictionary<ConsoleKey, KeyValuePair<string, Action>> Menu = new Dictionary
            <ConsoleKey, KeyValuePair<string, Action>>
        {
            // Exit
            {ConsoleKey.E, new KeyValuePair<string, Action>("Exits the console", () => Environment.Exit(0))},

            // Install
            {
                ConsoleKey.I, new KeyValuePair<string, Action>("Installs the files", () =>
                {
                    var ex = Installer.Installer.Install(_fromDir, _toDir, false, true);

                    Console.WriteLine("\n\n----------------\n" +
                                      (ex == null
                                          ? "Successfully installed files!"
                                          : "There was an error:\n\n" + ex.Message));
                })
            },

            // Check
            {
                ConsoleKey.C, new KeyValuePair<string, Action>("Checks the files", () =>
                {
                    var suc = Installer.Installer.Check(_fromDir, _toDir, true);

                    Console.WriteLine("\n\n----------------\n" +
                                      (suc.Count == 0
                                          ? "All files are fine!"
                                          : $"There is something wrong with your files:\n[!]{suc.Aggregate((i, j) => i + "\n[!]" + j)}\n=====\nRepairing is suggested!"));
                }
                    )
            },

            // Repair
            {
                ConsoleKey.R, new KeyValuePair<string, Action>("Repaires the files", () =>
                {
                    var ex = Installer.Installer.Install(_fromDir, _toDir, true, true);

                    Console.WriteLine("\n\n----------------\n" +
                                      (ex == null
                                          ? "Successfully repaired files!"
                                          : "There was an error:\n\n" + ex.Message));
                })
            },

            // Set source directory
            {
                ConsoleKey.F, new KeyValuePair<string, Action>("Set source directory", () =>
                {
                    Console.Write("New directory:  ");
                    var newLoc = Console.ReadLine();
                    if (Directory.Exists(newLoc))
                    {
                        _fromDir = newLoc;
                    }
                    else
                    {
                        Console.WriteLine("This directory does not exist...");
                    }
                })
            },

            // Set finale directory
            {
                ConsoleKey.T, new KeyValuePair<string, Action>("Set finale directory", () =>
                {
                    Console.Write("New directory:  ");
                    var newLoc = Console.ReadLine();
                    if (Directory.Exists(newLoc))
                    {
                        _toDir = newLoc;
                    }
                    else
                    {
                        Console.WriteLine("This directory does not exist...");
                    }
                })
            }
        };

        /// <summary>
        ///     Represents the source direcotry.
        /// </summary>
        private static string _fromDir = "From";

        /// <summary>
        ///     Represents the final destination directory.
        /// </summary>
        private static string _toDir = "To";

        private static void Main(string[] args)
        {
            // Do some basic menuing...
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to THE installer...\n----\n");

                Menu.Select(x => x.Key).ToList().ForEach(x => Console.WriteLine($"-> {x}\t...\t{Menu[x].Key}"));

                Console.Write("--------------\nPlease select an option to contiue:  ");
                var input = Console.ReadKey().Key;
                Console.WriteLine("\n--------------");

                if (Menu.ContainsKey(input)) // if the menu dictionary contains that option...
                {
                    Menu[input].Value.Invoke(); // than trigger the action.
                }

                Console.WriteLine("\n\n==============\n\nDo you want to return to the menu? (y/n)");
            } while (Console.ReadKey(true).Key.Equals(ConsoleKey.Y));
        }
    }
}