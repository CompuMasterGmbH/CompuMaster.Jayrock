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
    using System.Collections.Specialized;
    using System.IO;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestNameValueCollectionImporter
    {
        [Test]
        public void ImportNull()
        {
            Assert.That(UncheckedImport("null"), Is.Null);
        }

        [Test]
        public void ImportEmpty()
        {
            NameValueCollection collection = Import("{}");
            Assert.That(collection.Count, Is.EqualTo(0));
        }

        [Test]
        public void ImportOneNameValue()
        {
            NameValueCollection collection = Import("{\"foo\":\"bar\"}");
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection["foo"], Is.EqualTo("bar"));
        }

        [Test]
        public void ImportEmptyName()
        {
            NameValueCollection collection = Import("{\"\":\"bar\"}");
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[""], Is.EqualTo("bar"));
        }

        [Test]
        public void ImportEmptyValue()
        {
            NameValueCollection collection = Import("{\"foo\":\"\"}");
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection["foo"], Is.EqualTo(""));
        }

        [Test]
        public void ImportNullValue()
        {
            NameValueCollection collection = Import("{\"foo\":null}");
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection.Keys[0], Is.EqualTo("foo"));
            Assert.That(collection.Get(0), Is.Null);
        }

        [Test]
        public void ImportValuesArray()
        {
            NameValueCollection collection = Import("{\"foo\":[\"bar1\",\"bar2\",\"bar3\"]}");
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection.GetValues("foo"), Is.EqualTo(new string[] { "bar1", "bar2", "bar3" }));
        }

        [Test]
        public void CannotImportObjectValue()
        {
            Assert.Throws<JsonException>(() => Import("{\"foo\":{}}"));
        }

        [Test]
        public void PositionAfterImport()
        {
            JsonReader reader = new JsonTextReader(new StringReader("[{},'end']"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.Object));
            IImporter importer = new NameValueCollectionImporter();
            importer.Import(new ImportContext(), reader);
            Assert.That(reader.ReadString(), Is.EqualTo("end"));
        }

        private static NameValueCollection UncheckedImport(string s)
        {
            JsonReader reader = new JsonTextReader(new StringReader(s));
            IImporter importer = new NameValueCollectionImporter();
            NameValueCollection import = (NameValueCollection)importer.Import(new ImportContext(), reader);
            Assert.That(reader.EOF, Is.True, "Reader must be at EOF.");
            return import;
        }

        private static NameValueCollection Import(string s)
        {
            object o = UncheckedImport(s);
            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.InstanceOf<NameValueCollection>());
            return (NameValueCollection)o;
        }
    }
}

