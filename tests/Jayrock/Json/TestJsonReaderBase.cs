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
    using System.Globalization;
    using Jayrock.Json.Conversion.Converters;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonReaderBase
    {
        [Test]
        public void EOF()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();

            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.BOF));
            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ReadAfterEOF()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();

            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.BOF));
            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.EOF, Is.True);
            Assert.That(reader.Read(), Is.False);
        }

        [Test]
        public void ReadUnexpectedToken()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().End();

            Assert.Throws<JsonException>(() => reader.ReadToken(JsonTokenClass.Object));
        }

        [Test]
        public void ReadNull()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Null().End();

            reader.ReadNull();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ReadString()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().String("hello").End();

            Assert.That(reader.ReadString(), Is.EqualTo("hello"));
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ReadNumber()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Number(123456).End();

            Assert.That(reader.ReadNumber().ToString(), Is.EqualTo("123456"));
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ReadBoolean()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Boolean(true).End();

            Assert.That(reader.ReadBoolean(), Is.True);
            Assert.That(reader.EOF, Is.True);

            reader = new MockedJsonReader();
            reader.Begin().Boolean(false).End();

            Assert.That(reader.ReadBoolean(), Is.False);
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ReadTypedNumber()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().
                Number(123).
                Number(456).
                Number(2.5).
                Number(4.2).
                Number(9.99m).
            EndArray().End();

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That((int)reader.ReadNumber(), Is.EqualTo(123));
            Assert.That((long)reader.ReadNumber(), Is.EqualTo(456L));
            Assert.That((float)reader.ReadNumber(), Is.EqualTo(2.5f));
            Assert.That((double)reader.ReadNumber(), Is.EqualTo(4.2));
            Assert.That((decimal)reader.ReadNumber(), Is.EqualTo(9.99m));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.Read(), Is.False);
        }

        [Test]
        public void ReadMember()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object().Member("mon", "Monday").EndObject().End();

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("mon"));
            Assert.That(reader.ReadString(), Is.EqualTo("Monday"));
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.EndObject));
            Assert.That(reader.Read(), Is.False);
        }

        [Test]
        public void StepOutOfArrayFromStart()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();

            reader.MoveToContent();
            reader.StepOut();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void StepOutOfArrayFromMiddle()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();

            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.StepOut();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void StepOutOfArrayFromEnd()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("one").String("two").String("three").EndArray().End();

            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.ReadString();
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.EndArray));
            reader.StepOut();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void StepOutFromWithinNestedArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array()
                .String("one")
                .String("two").Array()
                    .String("three")
                    .String("four")
                    .EndArray()
                .String("five")
                .EndArray()
            .End();

            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadString();
            reader.ReadString();
            reader.StepOut();
            Assert.That(reader.ReadString(), Is.EqualTo("five"));
        }

        [Test]
        public void StepOutThroughNestedArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array()
                .String("one")
                .String("two").Array()
                    .String("three")
                    .String("four")
                    .EndArray()
                .String("five")
                .EndArray()
            .End();

            reader.MoveToContent();
            reader.StepOut();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void SkipScalar()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().String("test").End();
            reader.Skip();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void SkipObject()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object().Member("foo", "bar").EndObject().End();
            reader.Skip();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void SkipArray()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("test").EndArray().End();
            reader.Skip();
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void SkipArrayElement()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().String("foo").String("bar").EndArray().End();
            reader.Read();
            reader.Read();
            reader.Skip();
            Assert.That(reader.ReadString(), Is.EqualTo("bar"));
        }

        [Test]
        public void SkipObjectMember()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Object()
                .Member("m1", "v1")
                .Member("m2").Object()
                    .Member("m3", "v3")
                    .Member("m4", "v4")
                    .EndObject()
                .Member("m5", "v5")
                .EndObject()
            .End();
            reader.Read();
            reader.Read();
            reader.Skip();
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.Member));
            Assert.That(reader.Text, Is.EqualTo("m2"));
            reader.Skip();
            Assert.That(reader.ReadMember(), Is.EqualTo("m5"));
        }

        [Test]
        public void SkipAdjacentObjects()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.Begin().Array().Object().EndObject().Object().EndObject().EndArray().End();
            reader.ReadToken(JsonTokenClass.Array);
            reader.Skip();
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.Object));
            reader.Skip();
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void DefaultMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            Assert.That(reader.MaxDepth, Is.EqualTo(100));
        }

        [Test]
        public void SetMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            Assert.That(reader.MaxDepth, Is.Not.EqualTo(42));
            reader.MaxDepth = 42;
            Assert.That(reader.MaxDepth, Is.EqualTo(42));
        }

        [Test]
        public void CannotExceedMaxDepth()
        {
            MockedJsonReader reader = new MockedJsonReader();
            reader.MaxDepth = 2;
            reader.Array().Array().Array();
            reader.Read();
            reader.Read();
            reader.Read();

            // Exception will be thrown here, when attempting to read 
            // first value *after* the depth has been exceeded.

            Assert.Throws<Exception>(() => reader.Read());
        }

        private sealed class MockedJsonReader : JsonReaderBase
        {
            private Queue _queue = new Queue();

            protected override JsonToken ReadTokenImpl()
            {
                return (JsonToken)_queue.Dequeue();
            }

            private MockedJsonReader Append(JsonToken token)
            {
                _queue.Enqueue(token);
                return this;
            }

            public MockedJsonReader Begin()
            {
                _queue.Clear();
                return this;
            }

            public MockedJsonReader End()
            {
                return Append(JsonToken.EOF());
            }

            public MockedJsonReader Array()
            {
                return Append(JsonToken.Array());
            }

            public MockedJsonReader EndArray()
            {
                return Append(JsonToken.EndArray());
            }

            public MockedJsonReader String(string s)
            {
                return Append(JsonToken.String(s));
            }

            public MockedJsonReader Number(int i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }

            public MockedJsonReader Number(double i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }

            public MockedJsonReader Number(decimal i)
            {
                return Append(JsonToken.Number(i.ToString(CultureInfo.InvariantCulture)));
            }

            public MockedJsonReader Boolean(bool b)
            {
                return Append(JsonToken.Boolean(b));
            }

            public MockedJsonReader Object()
            {
                return Append(JsonToken.Object());
            }

            public MockedJsonReader EndObject()
            {
                return Append(JsonToken.EndObject());
            }

            public MockedJsonReader Member(string name, string value)
            {
                return Member(name).Append(JsonToken.String(value));
            }

            public MockedJsonReader Member(string name)
            {
                return Append(JsonToken.Member(name));
            }

            public MockedJsonReader Null()
            {
                return Append(JsonToken.Null());
            }
        }
    }
}

