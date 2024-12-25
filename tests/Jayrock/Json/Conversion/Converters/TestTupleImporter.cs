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

#if !NET_1_0 && !NET_1_1 && !NET_2_0 

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestTupleImporter
    {
        [Test]
        public void CannotInitializeWithNonTupleType()
        {
            Assert.Throws<ArgumentException>(() => new TupleImporter(typeof(object)));
        }

        [Test]
        public void Superclass()
        {
            Assert.That(new TupleImporter(typeof(Tuple<object>)), Is.InstanceOf<ImporterBase>());
        }

        [Test]
        public void InputTypeIsTuple()
        {
            Assert.That(new TupleImporter(typeof(Tuple<object>)).OutputType, Is.SameAs(typeof(Tuple<object>)));
        }

        [Test]
        public void ImportTuple1FromNumber()
        {
            AssertImport(Tuple.Create(42), "42");
        }

        [Test]
        public void ImportTuple1FromString()
        {
            AssertImport(Tuple.Create("foo"), "foo");
        }

        [Test]
        public void ImportTuple1FromBoolean()
        {
            AssertImport(Tuple.Create(true), "true");
        }

        [Test]
        public void ImportTuple1FromNull()
        {
            var importer = new TupleImporter(typeof(Tuple<int>));
            var result = importer.Import(JsonConvert.CreateImportContext(), JsonText.CreateReader("null"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ImportTuple1FromObject()
        {
            var tuple = JsonConvert.Import<Tuple<JsonObject>>("{x:123,y:456}");
            Assert.That(tuple.Item1, Is.Not.Null);
            dynamic obj = tuple.Item1;
            Assert.That(obj.x.ToInt32(), Is.EqualTo(123));
            Assert.That(obj.y.ToInt32(), Is.EqualTo(456));
        }

        [Test]
        public void ImportTuple1FromArray()
        {
            var expected = Tuple.Create(new[] { 123, 456, 789 });
            var actual = JsonConvert.Import<Tuple<int[]>>("[123, 456, 789]");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Item1, Is.EqualTo(expected.Item1));
        }

        [Test]
        public void ImportTuple3FromArray()
        {
            AssertImport(Tuple.Create(42, "foo", true), "[42,foo,true]");
        }

        [Test]
        public void CannotImportTuple2FromNumber()
        {
            Assert.Throws<JsonException>(() => JsonConvert.Import<Tuple<int, int>>("42"));
        }

        [Test]
        public void CannotImportTuple2FromString()
        {
            Assert.Throws<JsonException>(() => JsonConvert.Import<Tuple<string, string>>("foo"));
        }

        [Test]
        public void CannotImportTuple2FromBoolean()
        {
            Assert.Throws<JsonException>(() => JsonConvert.Import<Tuple<bool, bool>>("true"));
        }

        [Test]
        public void CannotImportTuple2FromObject()
        {
            Assert.Throws<JsonException>(() => JsonConvert.Import<Tuple<JsonObject, JsonObject>>("{}"));
        }

        [Test]
        public void CannotImportWhenArrayHasTooFewElements()
        {
            Assert.Throws<JsonException>(() => JsonConvert.Import<Tuple<int, string, bool>>("[123,foo]"));
        }

        [Test]
        public void SubClassCannotExportValueWithNullContext()
        {
            var tuple = Tuple.Create(42);
            var exporter = new ExportValueTestExporter(tuple.GetType());
            exporter.ForceNullContext = true;
            var context = JsonConvert.CreateExportContext();
            var writer = new JsonBufferWriter();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(context, tuple, writer));
        }

        [Test]
        public void SubClassCannotExportValueWithNullWriter()
        {
            var tuple = Tuple.Create(42);
            var exporter = new ExportValueTestExporter(tuple.GetType());
            exporter.ForceNullWriter = true;
            var context = JsonConvert.CreateExportContext();
            var writer = new JsonBufferWriter();
            Assert.Throws<ArgumentNullException>(() => exporter.Export(context, tuple, writer));
        }

        private static void AssertImport(object expected, string input)
        {
            var importer = new TupleImporter(expected.GetType());
            var reader = JsonText.CreateReader(input);
            var context = JsonConvert.CreateImportContext();
            var actual = importer.Import(context, reader);
            Assert.That(reader.EOF, Is.True, "Reader must be at EOF.");
            Assert.That(actual, Is.EqualTo(expected));
        }

        class ExportValueTestExporter : TupleExporter
        {
            public bool ForceNullContext;
            public bool ForceNullWriter;

            public ExportValueTestExporter(Type inputType) :
                base(inputType)
            { }

            protected override void ExportValue(ExportContext context, object value, JsonWriter writer)
            {
                base.ExportValue(ForceNullContext ? null : context, value, ForceNullWriter ? null : writer);
            }
        }
    }
}

#endif




