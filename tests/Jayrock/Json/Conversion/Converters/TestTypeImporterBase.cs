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
    using System.IO;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestTypeImporterBase
    {
        [Test]
        public void OutputTypeInitialization()
        {
            TestImporter importer = new TestImporter();
            Assert.That(importer.OutputType, Is.SameAs(typeof(object)));
        }

        [Test]
        public void NullHandling()
        {
            JsonReader reader = CreateReader("null");
            TestImporter importer = new TestImporter();
            Assert.That(importer.Import(new ImportContext(), reader), Is.Null);
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void CannotImportNumber()
        {
            Assert.Throws<JsonException>(() => Import("42"));
        }

        [Test]
        public void CannotImportBoolean()
        {
            Assert.Throws<JsonException>(() => Import("true"));
        }

        [Test]
        public void CannotImportString()
        {
            Assert.Throws<JsonException>(() => Import("'string'"));
        }

        [Test]
        public void CannotImportObject()
        {
            Assert.Throws<JsonException>(() => Import("{}"));
        }

        [Test]
        public void CannotImportArray()
        {
            Assert.Throws<JsonException>(() => Import("[]"));
        }

        [Test]
        public void NumberCallsImportNumber()
        {
            JsonReader reader = CreateReader("42");
            ImporterMock importer = new ImporterMock();
            const int result = 42;
            importer.Number = result;
            Assert.That(importer.Import(new ImportContext(), reader), Is.EqualTo(result));
        }

        [Test]
        public void StringCallsImportString()
        {
            JsonReader reader = CreateReader("''");
            ImporterMock importer = new ImporterMock();
            const string result = "hello";
            importer.String = result;
            Assert.That(importer.Import(new ImportContext(), reader), Is.EqualTo(result));
        }

        [Test]
        public void BooleanCallsImportBoolean()
        {
            JsonReader reader = CreateReader("true");
            ImporterMock importer = new ImporterMock();
            importer.Boolean = true;
            Assert.That(importer.Import(new ImportContext(), reader), Is.EqualTo(true));
        }

        [Test]
        public void ArrayCallsImportArray()
        {
            JsonReader reader = CreateReader("[]");
            ImporterMock importer = new ImporterMock();
            object result = new object();
            importer.Array = result;
            Assert.That(importer.Import(new ImportContext(), reader), Is.EqualTo(result));
        }

        [Test]
        public void ObjectCallsImportObject()
        {
            JsonReader reader = CreateReader("{}");
            ImporterMock importer = new ImporterMock();
            object result = new object();
            importer.Object = result;
            Assert.That(importer.Import(new ImportContext(), reader), Is.EqualTo(result));
        }

        private static void Import(string s)
        {
            (new TestImporter()).Import(new ImportContext(), CreateReader(s));
        }

        private static JsonReader CreateReader(string s)
        {
            return new JsonTextReader(new StringReader(s));
        }

        private class TestImporter : ImporterBase
        {
            public TestImporter() :
                base(typeof(object))
            { }
        }

        private class ImporterMock : TestImporter
        {
            public object Boolean = null;
            public object Number = null;
            public object String = null;
            public object Object = null;
            public object Array = null;

            protected override object ImportFromBoolean(ImportContext context, JsonReader reader)
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(reader, Is.Not.Null);

                return Boolean;
            }

            protected override object ImportFromNumber(ImportContext context, JsonReader reader)
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(reader, Is.Not.Null);

                return Number;
            }

            protected override object ImportFromString(ImportContext context, JsonReader reader)
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(reader, Is.Not.Null);

                return String;
            }

            protected override object ImportFromArray(ImportContext context, JsonReader reader)
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(reader, Is.Not.Null);

                return Array;
            }

            protected override object ImportFromObject(ImportContext context, JsonReader reader)
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(reader, Is.Not.Null);

                return Object;
            }
        }
    }
}




