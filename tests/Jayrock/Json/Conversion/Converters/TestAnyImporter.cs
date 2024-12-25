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

    using System.IO;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestAnyImporter
    {
        [Test]
        public void ParseNumber()
        {
            Assert.That((int)(JsonNumber)Parse("123"), Is.EqualTo(123));
        }

        [Test]
        public void ParseString()
        {
            // TODO: Move some of these test to JsonTextWriter since these have more to do with parsing than importing.

            Assert.That(Parse("\"Hello World\""), Is.EqualTo("Hello World"), "Double-quoted string.");
            Assert.That(Parse("'Hello World'"), Is.EqualTo("Hello World"), "Single-quoted string.");
            Assert.That(Parse("\"Hello 'World'\""), Is.EqualTo("Hello 'World'"));
            Assert.That(Parse("'Hello \"World\"'"), Is.EqualTo("Hello \"World\""));
        }

        [Test]
        public void ParseBoolean()
        {
            Assert.That(Parse("true"), Is.True);
            Assert.That(Parse("false"), Is.False);
        }

        [Test]
        public void ParseNull()
        {
            Assert.That(Parse("null"), Is.Null);
        }

        [Test]
        public void ParseEmptyArray()
        {
            JsonArray values = (JsonArray)Parse("[]");
            Assert.That(values, Is.Not.Null);
            Assert.That(values.Length, Is.EqualTo(0));
        }

        [Test]
        public void ParseArray()
        {
            JsonArray values = (JsonArray)Parse("[123,'Hello World',true]");
            Assert.That(values, Is.Not.Null);
            Assert.That(values.Length, Is.EqualTo(3));
            Assert.That((int)(JsonNumber)values[0], Is.EqualTo(123));
            Assert.That(values[1], Is.EqualTo("Hello World"));
            Assert.That(values[2], Is.True);
        }

        [Test]
        public void ParseEmptyObject()
        {
            JsonObject o = (JsonObject)Parse("{}");
            Assert.That(o, Is.Not.Null);
            Assert.That(o.Count, Is.EqualTo(0));
        }

        [Test]
        public void ParseObject()
        {
            JsonObject article = (JsonObject)Parse(@"
                /* Article */ {
                    Title : 'Introduction to JSON',
                    Rating : 2,
                    Abstract : null,
                    Author : {
                        Name : 'John Doe',
                        'E-Mail Address' : 'john.doe@example.com' 
                    },
                    References : [
                        { Title : 'JSON RPC', Link : 'http://www.json-rpc.org/' }
                    ]
                }");

            Assert.That(article, Is.Not.Null);
            Assert.That(article.Count, Is.EqualTo(5));
            Assert.That(article["Title"], Is.EqualTo("Introduction to JSON"));
            Assert.That((int)(JsonNumber)article["Rating"], Is.EqualTo(2));
            Assert.That(article.Contains("Abstract"), Is.True);
            Assert.That(article["Abstract"], Is.Null);

            JsonObject author = (JsonObject)article["Author"];
            Assert.That(author, Is.Not.Null);
            Assert.That(author.Count, Is.EqualTo(2));
            Assert.That(author["Name"], Is.EqualTo("John Doe"));
            Assert.That(author["E-Mail Address"], Is.EqualTo("john.doe@example.com"));

            JsonArray references = (JsonArray)article["References"];
            Assert.That(references, Is.Not.Null);
            Assert.That(references.Length, Is.EqualTo(1));

            JsonObject reference = (JsonObject)references[0];
            Assert.That(reference, Is.Not.Null);
            Assert.That(reference.Count, Is.EqualTo(2));
            Assert.That(reference["Title"], Is.EqualTo("JSON RPC"));
            Assert.That(reference["Link"], Is.EqualTo("http://www.json-rpc.org/"));
        }

        private object Parse(string s)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(s));
            object value = JsonConvert.Import(reader);
            Assert.That(reader.EOF, Is.True);
            return value;
        }
    }
}
