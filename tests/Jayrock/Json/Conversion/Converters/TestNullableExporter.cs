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

#if !NET_1_0 && !NET_1_1 

namespace Jayrock.Json.Conversion.Converters
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TestNullableExporter
    {
        [Test]
        public void Superclass()
        {
            Assert.That(new NullableExporter(typeof(int?)), Is.InstanceOf<ExporterBase>());
        }

        [Test]
        public void CannotInitializeWithNonNullableType()
        {
            Assert.Throws<ArgumentException>(() => new NullableExporter(typeof(int)));
        }

        [Test]
        public void InputTypeIsNullable()
        {
            Assert.That((new NullableExporter(typeof(int?))).InputType, Is.SameAs(typeof(int?)));
        }

        [Test]
        public void ExportNull()
        {
            Export(null).ReadNull();
        }

        [Test]
        public void ExportNonNull()
        {
            Assert.That(Export(42).ReadNumber().ToInt32(), Is.EqualTo(42));
        }

        private static JsonReader Export(int? value)
        {
            JsonRecorder writer = new JsonRecorder();
            ExportContext context = JsonConvert.CreateExportContext();
            new NullableExporter(typeof(int?)).Export(context, value, writer);
            return writer.CreatePlayer();
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 
