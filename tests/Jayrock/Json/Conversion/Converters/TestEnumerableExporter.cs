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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestEnumerableExporter
    {
        [Test]
        public void Superclass()
        {
            Assert.That(new EnumerableExporter(typeof(Array)), Is.InstanceOf<ExporterBase>());
        }

        [Test]
        public void InputTypeInitialization()
        {
            Type type = typeof(Array);
            EnumerableExporter exporter = new EnumerableExporter(type);
            Assert.That(exporter.InputType, Is.SameAs(type));
        }

        [Test]
        public void ExportEmpty()
        {
            JsonReader reader = Export(new object[] { });
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.EndArray));
        }

        [Test]
        public void ExportFlatArray()
        {
            JsonReader reader = Export(new int[] { 11, 22, 33 });
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(11));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(22));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(33));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void ExportList()
        {
            JsonReader reader = Export(new ArrayList(new int[] { 11, 22, 33 }));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(11));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(22));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(33));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void ExportNestedArrays()
        {
            JsonReader reader = Export(new ArrayList(new object[] { 11, 22, new object[] { 33, 44 }, 55 }));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(11));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(22));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(33));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(44));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(55));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        private static JsonReader Export(IEnumerable values)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(values, writer);
            return writer.CreatePlayer();
        }
    }
}
