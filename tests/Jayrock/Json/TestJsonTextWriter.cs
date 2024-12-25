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
    using System.IO;
    using NUnit.Framework;
    using Jayrock.Json.Conversion;

    #endregion

    [TestFixture]
    public class TestJsonTextWriter
    {
        [Test]
        public void Blank()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            Assert.That(writer.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void WriteString()
        {
            WriteString("[\"Hello\"]", "Hello");
            WriteString("[\"Hello World\"]", "Hello World");
            WriteString("[\"And before he parted, he said, \\\"Goodbye, people!\\\"\"]", "And before he parted, he said, \"Goodbye, people!\"");
            WriteString("[\"Hello\\tWorld\"]", "Hello\tWorld");
            WriteString("[\"Hello\\u0000World\"]", "Hello" + ((char)0) + "World");
        }

        private static void WriteString(string expected, string value)
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteString(value);
            Assert.That(writer.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteNumber()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteNumber(123);
            Assert.That(writer.ToString(), Is.EqualTo("[123]"));
        }

        [Test]
        public void WriteNull()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteNull();
            Assert.That(writer.ToString(), Is.EqualTo("[null]"));
        }

        [Test]
        public void WriteTrueBoolean()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteBoolean(true);
            Assert.That(writer.ToString(), Is.EqualTo("[true]"));
        }

        [Test]
        public void WriteFalseBoolean()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteBoolean(false);
            Assert.That(writer.ToString(), Is.EqualTo("[false]"));
        }

        [Test]
        public void WriteEmptyArray()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStringArray(new string[0]);
            Assert.That(writer.ToString(), Is.EqualTo("[]"));
        }

        [Test]
        public void WriteArray()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStringArray(new object[] { 123, "Hello \"Old\" World", true });
            Assert.That(writer.ToString(), Is.EqualTo("[\"123\",\"Hello \\\"Old\\\" World\",\"True\"]"));
        }

        [Test]
        public void WriteEmptyObject()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStartObject();
            writer.WriteEndObject();
            Assert.That(writer.ToString(), Is.EqualTo("{}"));
        }

        [Test]
        public void WriteObject()
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            writer.WriteStartObject();
            writer.WriteMember("Name");
            writer.WriteString("John Doe");
            writer.WriteMember("Salary");
            writer.WriteNumber(123456789);
            writer.WriteEndObject();
            Assert.That(writer.ToString(), Is.EqualTo("{\"Name\":\"John Doe\",\"Salary\":123456789}"));
        }

        [Test]
        public void WriteNullValue()
        {
            Assert.That(JsonConvert.ExportToString(JsonNull.Value), Is.EqualTo("[null]"));
        }

        [Test]
        public void WriteValue()
        {
            Assert.That(WriteValue((byte)123), Is.EqualTo("[123]"), "Byte");
            Assert.That(WriteValue((sbyte)123), Is.EqualTo("[\"123\"]"), "Short byte");
            Assert.That(WriteValue((short)123), Is.EqualTo("[123]"), "Short integer");
            Assert.That(WriteValue(123), Is.EqualTo("[123]"), "Integer");
            Assert.That(WriteValue(123L), Is.EqualTo("[123]"), "Long integer");
            Assert.That(WriteValue(123m), Is.EqualTo("[123]"), "Decimal");
        }

        [Test]
        public void WriteObjectArray()
        {
            JsonObject o = new JsonObject();
            o.Put("one", 1);
            o.Put("two", 2);
            o.Put("three", 3);
            Assert.That(WriteValue(new object[] { o, o, o }), Is.EqualTo("[{\"one\":1,\"two\":2,\"three\":3},{\"one\":1,\"two\":2,\"three\":3},{\"one\":1,\"two\":2,\"three\":3}]"));
        }

        [Test]
        public void WriteNestedArrays()
        {
            int[] inner = new int[] { 1, 2, 3 };
            int[][] outer = new int[][] { inner, inner, inner };
            Assert.That(WriteValue(outer), Is.EqualTo("[[1,2,3],[1,2,3],[1,2,3]]"));
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

            JsonTextWriter writer = new JsonTextWriter();
            writer.WriteFromReader(reader);
            Assert.That(writer.ToString(), Is.EqualTo("{\"menu\":{\"id\":\"file\",\"value\":\"File:\",\"popup\":{\"menuitem\":[{\"value\":\"New\",\"onclick\":\"CreateNewDoc()\"},{\"value\":\"Open\",\"onclick\":\"OpenDoc()\"},{\"value\":\"Close\",\"onclick\":\"CloseDoc()\"}]}}}"));
        }

        [Test]
        public void PrettyPrinting()
        {
            JsonTextWriter writer = new JsonTextWriter();
            writer.PrettyPrint = true;
            writer.WriteFromReader(new JsonTextReader(new StringReader("{'menu':{'id':'file','value':'File:','popup':{'menuitem':[{'value':'New','onclick':'CreateNewDoc()'},{'value':'Open','onclick':'OpenDoc()'},{'value':'Close','onclick':'CloseDoc()'}]}}}")));
            Assert.That(writer.ToString() + Environment.NewLine, Is.EqualTo(RewriteLines(@"{ 
    ""menu"": { 
        ""id"": ""file"",
        ""value"": ""File:"",
        ""popup"": { 
            ""menuitem"": [ { 
                ""value"": ""New"",
                ""onclick"": ""CreateNewDoc()""
            }, { 
                ""value"": ""Open"",
                ""onclick"": ""OpenDoc()""
            }, { 
                ""value"": ""Close"",
                ""onclick"": ""CloseDoc()""
            } ]
        }
    }
}")));
        }

        private static string WriteValue(object value)
        {
            JsonTextWriter writer = new JsonTextWriter(new StringWriter());
            JsonConvert.Export(value, writer);
            return writer.ToString();
        }

        private static string RewriteLines(string s)
        {
            StringReader reader = new StringReader(s);
            StringWriter writer = new StringWriter();

            string line = reader.ReadLine();
            while (line != null)
            {
                writer.WriteLine(line);
                line = reader.ReadLine();
            }

            return writer.ToString();
        }
    }
}
