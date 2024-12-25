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
    using Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonImportingWriter
    {
        [Test]
        public void Blank()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            Assert.That(writer.Value, Is.Null);
            Assert.That(writer.IsArray, Is.False);
            Assert.That(writer.IsObject, Is.False);
        }

        [Test]
        public void WriteString()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteString("foobar");
            Assert.That(GetSingleValue(writer), Is.EqualTo("foobar"));
        }

        [Test]
        public void WriteNumber()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteNumber("1234");
            Assert.That(GetSingleValue(writer), Is.EqualTo(new JsonNumber("1234")));
        }

        [Test]
        public void WriteNull()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteNull();
            Assert.That(GetSingleValue(writer), Is.Null);
        }

        [Test]
        public void WriteTrueBoolean()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteBoolean(true);
            Assert.That(GetSingleValue(writer), Is.EqualTo(true));
        }

        [Test]
        public void WriteFalseBoolean()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteBoolean(false);
            Assert.That(GetSingleValue(writer), Is.EqualTo(false));
        }

        [Test]
        public void WriteEmptyArray()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStringArray(new string[0]);
            Assert.That(GetArray(writer), Is.EqualTo(new object[0]));
        }

        [Test]
        public void WriteArray()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartArray();
            writer.WriteNumber(123);
            writer.WriteString("Hello World");
            writer.WriteBoolean(true);
            writer.WriteEndArray();
            Assert.That(GetArray(writer), Is.EqualTo(new object[] { new JsonNumber("123"), "Hello World", true }));
        }

        [Test]
        public void WriteEmptyObject()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartObject();
            writer.WriteEndObject();
            Assert.That(((IDictionary)writer.Value).Count, Is.EqualTo(0));
        }

        [Test]
        public void WriteObject()
        {
            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteStartObject();
            writer.WriteMember("Name");
            writer.WriteString("John Doe");
            writer.WriteMember("Salary");
            writer.WriteNumber(123456789);
            writer.WriteEndObject();
            Assert.That(writer.Value, Is.Not.Null);
            IDictionary obj = (IDictionary)writer.Value;
            Assert.That(obj.Count, Is.EqualTo(2));
            Assert.That(obj["Name"], Is.EqualTo("John Doe"));
            Assert.That(Convert.ToInt32(obj["Salary"]), Is.EqualTo(123456789));
        }

        [Test]
        public void WriteFromReader()
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(@"
                { 'menu': {
                    'id': 'file',
                    'value': 'File:',
                    'popup': {
                      'menuitem': [
                        {'value': 'New', 'onclick': 'CreateNewDoc()'},
                        {'value': 'Open', 'onclick': 'OpenDoc()'},
                        {'value': 'Close', 'onclick': 'CloseDoc()'}
                      ]
                    }
                  }
                }"));

            JsonImportingWriter writer = new JsonImportingWriter();
            writer.WriteFromReader(reader);

            IDictionary root = (IDictionary)writer.Value;
            Assert.That(root.Count, Is.EqualTo(1));
            IDictionary menu = (IDictionary)root["menu"];
            Assert.That(menu.Count, Is.EqualTo(3));
            Assert.That(menu["id"], Is.EqualTo("file"));
            Assert.That(menu["value"], Is.EqualTo("File:"));
            IDictionary popup = (IDictionary)menu["popup"];
            Assert.That(popup.Count, Is.EqualTo(1));
            IList menuitems = (IList)popup["menuitem"];
            Assert.That(menuitems.Count, Is.EqualTo(3));
            IDictionary menuitem;
            menuitem = (IDictionary)menuitems[0];
            Assert.That(menuitem["value"], Is.EqualTo("New"));
            Assert.That(menuitem["onclick"], Is.EqualTo("CreateNewDoc()"));
            menuitem = (IDictionary)menuitems[1];
            Assert.That(menuitem["value"], Is.EqualTo("Open"));
            Assert.That(menuitem["onclick"], Is.EqualTo("OpenDoc()"));
            menuitem = (IDictionary)menuitems[2];
            Assert.That(menuitem["value"], Is.EqualTo("Close"));
            Assert.That(menuitem["onclick"], Is.EqualTo("CloseDoc()"));
        }

        private static object GetSingleValue(JsonImportingWriter writer)
        {
            Assert.That(writer.IsArray, Is.True);
            Assert.That(writer.IsObject, Is.False);
            IEnumerator e = ((IEnumerable)writer.Value).GetEnumerator();
            Assert.That(e.MoveNext(), Is.True);
            object result = e.Current;
            Assert.That(e.MoveNext(), Is.False);
            return result;
        }

        private static object[] GetArray(JsonImportingWriter writer)
        {
            Assert.That(writer.IsArray, Is.True);
            Assert.That(writer.IsObject, Is.False);
            ICollection collection = ((ICollection)writer.Value);
            object[] result = new object[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }
    }
}
