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
    public class TestTupleExporter
    {
        [Test]
        public void CannotInitializeWithNonTupleType()
        {
            Assert.Throws<ArgumentException>(() => new TupleExporter(typeof(object)));
        }

        [Test]
        public void Superclass()
        {
            Assert.That(new TupleExporter(typeof(Tuple<object>)), Is.InstanceOf<ExporterBase>());
        }

        [Test]
        public void InputTypeIsTuple()
        {
            Assert.That(new TupleExporter(typeof(Tuple<object>)).InputType, Is.SameAs(typeof(Tuple<object>)));
        }

        [Test]
        public void Export()
        {
            var reader = Export(Tuple.Create(123, "foo", true));
            Assert.That(reader.MoveToContent(), Is.True);
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(reader.ReadString(), Is.EqualTo("foo"));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(true));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.Read(), Is.False);
        }

        private static JsonReader Export(object value)
        {
            var writer = new JsonBufferWriter();
            JsonConvert.Export(value, writer);
            return writer.GetBuffer().CreateReader();
        }
    }
}

#endif




