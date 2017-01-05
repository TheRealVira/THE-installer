#region License

// Copyright (c) 2017, Vira
// All rights reserved.
// Solution: Installer
// Project: Installer
// Filename: Item.cs
// Date - created:2017.01.05 - 09:50
// Date - current: 2017.01.05 - 12:43

#endregion

#region Usings

using System.Xml.Serialization;

#endregion

namespace Installer
{
    public class Item
    {
        [XmlAttribute] public string id;

        [XmlAttribute] public byte[] value;
    }
}