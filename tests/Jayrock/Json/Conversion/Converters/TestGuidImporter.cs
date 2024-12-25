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
    public class TestGuidImporter
    {
        [Test]
        public void ImportString()
        {
            AssertImport(new Guid("89158d71-cf6a-4f3c-b180-063b8910f658"), "'89158d71-cf6a-4f3c-b180-063b8910f658'");
        }

        [Test]
        public void CannotImportBadlyFormattedGuidString()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "'foo'"));
        }

        [Test]
        public void ImportNull()
        {
            AssertImport(null, "null");
        }

        [Test]
        public void CannotImportNumber()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "123"));
        }

        [Test]
        public void CannotImportBoolean()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "true"));
        }

        [Test]
        public void CannotImportArray()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "[]"));
        }

        [Test]
        public void CannotImportObject()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "{}"));
        }

        private static void AssertImport(object expected, string input)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            GuidImporter importer = new GuidImporter();
            object o = importer.Import(new ImportContext(), reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            if (expected != null)
                Assert.IsInstanceOf<Guid>(o);
            Assert.AreEqual(expected, o);
        }
    }
}


