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
    using System.Collections;
    using System.IO;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonArray
    {
        [Test]
        public void AddNullValue()
        {
            JsonArray a = new JsonArray();
            a.Add(null);
            Assert.That(a.Count, Is.EqualTo(1));
            Assert.That(a[0], Is.Null);
        }

        [Test]
        public void AddNullValueViaIList()
        {
            IList list = new JsonArray();
            list.Add(null);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.Null);
        }

        [Test]
        public void Import()
        {
            JsonArray a = new JsonArray();
            a.Import(new JsonTextReader(new StringReader("[123,'Hello World',true]")));
            Assert.That(a.Length, Is.EqualTo(3));
            Assert.That((int)(JsonNumber)a[0], Is.EqualTo(123));
            Assert.That(a[1], Is.EqualTo("Hello World"));
            Assert.That(a[2], Is.EqualTo(true));
        }

        [Test]
        public void Export()
        {
            JsonArray a = new JsonArray(new object[] { 123, "Hello World", true });
            JsonRecorder writer = new JsonRecorder();
            a.Export(writer);
            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(a[0]));
            Assert.That(reader.ReadString(), Is.EqualTo(a[1]));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(a[2]));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void ContentsClearedBeforeImporting()
        {
            JsonArray a = new JsonArray();
            a.Add(new object());
            Assert.That(a.Length, Is.EqualTo(1));
            a.Import(new JsonTextReader(new StringReader("[123]")));
            Assert.That(a.Length, Is.EqualTo(1));
        }

        [Test]
        public void ImportIsExceptionSafe()
        {
            JsonArray a = new JsonArray();
            object o = new object();
            a.Add(o);

            try
            {
                a.Import(new JsonTextReader(new StringReader("[123,456,")));
            }
            catch (JsonException)
            {
            }

            Assert.That(a.Count, Is.EqualTo(1));
            Assert.That(a[0], Is.SameAs(o));
        }

        [Test]
        public void CannotUseNullReaderWithImport()
        {
            IJsonImportable array = new JsonArray();
            Assert.Throws<ArgumentNullException>(() => array.Import(new ImportContext(), null));
        }

        [Test]
        public void CannotUseNullContextWithImport()
        {
            IJsonImportable array = new JsonArray();
            Assert.Throws<ArgumentNullException>(() => array.Import(null, (new JsonRecorder()).CreatePlayer()));
        }

        [Test]
        public void CannotUseNullArgWithExport()
        {
            IJsonExportable array = new JsonArray();
            Assert.Throws<ArgumentNullException>(() => array.Export(null, null));
        }
    }
}




