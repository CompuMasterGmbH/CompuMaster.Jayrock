#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.Json
{
    #region Imports

    using System;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonNull
    {
        [Test]
        public void Equality()
        {
            Assert.That(JsonNull.Value.Equals(JsonNull.Value), Is.True);
        }

#if NETFRAMEWORK
        [Test]
        public void ReferenceEqualityPostDeserialization()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, JsonNull.Value);
            stream.Position = 0;
            object o = formatter.Deserialize(stream);
            Assert.That(JsonNull.Value.Equals(o), Is.True);
        }
#endif

        [Test]
        public void LogicalEquality()
        {
            Assert.That(JsonNull.LogicallyEquals(JsonNull.Value), Is.True, "Equals self?");
            Assert.That(JsonNull.LogicallyEquals(new object()), Is.False, "Equals non-nullable?");
            Assert.That(JsonNull.LogicallyEquals(null), Is.True, "Equals null reference?");
            Assert.That(JsonNull.LogicallyEquals(DBNull.Value), Is.True, "Equals DBNull?");
        }

        [Test]
        public void StringRepresentation()
        {
            Assert.That(JsonNull.Value.ToString(), Is.EqualTo("null"));
        }
    }
}
