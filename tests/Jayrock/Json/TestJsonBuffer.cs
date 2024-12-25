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
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonBuffer
    {
        [Test]
        public void IsSerializable()
        {
            Assert.That(typeof(JsonBuffer).IsSerializable, Is.True);
        }

        [Test]
        public void EmptyHasZeroLength()
        {
            Assert.That(JsonBuffer.Empty.Length, Is.EqualTo(0));
        }

        [Test]
        public void EmptyIsNotNull()
        {
            Assert.That(JsonBuffer.Empty.IsNull, Is.False);
        }

        [Test]
        public void EmptyIsNotScalar()
        {
            Assert.That(JsonBuffer.Empty.IsScalar, Is.False);
        }

        [Test]
        public void EmptyIsNotStructured()
        {
            Assert.That(JsonBuffer.Empty.IsStructured, Is.False);
        }

        [Test]
        public void EmptyIsEmpty()
        {
            Assert.That(JsonBuffer.Empty.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyCannotBeRead()
        {
            Assert.Throws<InvalidOperationException>(() => JsonBuffer.Empty.CreateReader());
        }

        [Test]
        public void EmptyCannotBeIndexedInto()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonBuffer.Empty[0].ToString()); /* [1] */

            //
            // [1] The ToString call is unnecessary but it is added here
            //     instead of taking the indexed value into an unused 
            //     variable to avoid the CS0219 warning issue from Mono's 
            //     C# compiler. See: 
            //     http://bugzilla.novell.com/show_bug.cgi?id=316137
        }

        [Test]
        public void WritingReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteStartArray();
            writer.WriteString("foo");
            writer.WriteString("bar");
            writer.WriteString("baz");
            writer.WriteEndArray();
            JsonBuffer buffer = writer.GetBuffer();
            Assert.That(buffer.Length, Is.EqualTo(5));
            JsonBufferReader reader = buffer.CreateReader();
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("foo"));
            Assert.That(reader.ReadString(), Is.EqualTo("bar"));
            Assert.That(reader.ReadString(), Is.EqualTo("baz"));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.EOF, Is.True);
        }

        [Test]
        public void ScalarValueReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteString("foobar");
            AssertBufferedValueScalarOrNull(JsonToken.String("foobar"), writer);
        }

        [Test]
        public void NullValueReading()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteNull();
            AssertBufferedValueScalarOrNull(JsonToken.Null(), writer);
        }

        [Test]
        public void BufferValueOnArrayEnd()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[[],foo]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // outer array start
            reader.Read(); // inner array start
            reader.Read(); // inner array end
            Assert.That(reader.BufferValue()[0], Is.EqualTo(JsonToken.String("foo")));
        }

        [Test]
        public void BufferValueOnObjectEnd()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[{},foo]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // outer object start
            reader.Read(); // inner object start
            reader.Read(); // inner object end
            Assert.That(reader.BufferValue()[0], Is.EqualTo(JsonToken.String("foo")));
        }

        [Test]
        public void BufferValueOnMember()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("{foo:bar}"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // object start
            reader.Read(); // foo
            Assert.That(reader.BufferValue().CreateReader().Token, Is.EqualTo(JsonToken.String("bar")));
        }

        [Test]
        public void BufferValueOnEOF()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[]"));
            JsonBufferReader reader = writer.GetBuffer().CreateReader();
            reader.Read(); // array start
            reader.Read(); // array end
            Assert.That(reader.BufferValue().IsEmpty, Is.True);
        }

        [Test]
        public void BufferValueOnBOF()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteFromReader(JsonText.CreateReader("[]"));
            JsonBuffer buffer = writer.GetBuffer().CreateReader().BufferValue();
            Assert.That(buffer.Length, Is.EqualTo(2));
            Assert.That(buffer[0], Is.EqualTo(JsonToken.Array()));
            Assert.That(buffer[1], Is.EqualTo(JsonToken.EndArray()));
        }

        [Test]
        public void BufferScalarValue()
        {
            JsonBufferWriter writer = new JsonBufferWriter();
            writer.WriteString("foobar");
            JsonBuffer buffer = writer.GetBuffer();
            JsonBufferReader reader = buffer.CreateReader();
            reader.Read(); // array start
            reader.Read(); // string
            Assert.That(reader.BufferValue().CreateReader().BufferValue().CreateReader().ReadString(), Is.EqualTo("foobar"));
        }

        [Test]
        public void GetArrayLength()
        {
            Assert.That(JsonBuffer.From("[1,2,3]").GetArrayLength(), Is.EqualTo(3));
        }

        [Test]
        public void GetArray()
        {
            JsonBuffer[] values = new JsonBuffer[3];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.That(count, Is.EqualTo(3));
            Assert.That((int)values[0].GetNumber(), Is.EqualTo(1));
            Assert.That((int)values[1].GetNumber(), Is.EqualTo(2));
            Assert.That((int)values[2].GetNumber(), Is.EqualTo(3));
        }

        [Test]
        public void GetArrayShort()
        {
            JsonBuffer[] values = new JsonBuffer[2];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.That(count, Is.EqualTo(~2));
            Assert.That((int)values[0].GetNumber(), Is.EqualTo(1));
            Assert.That((int)values[1].GetNumber(), Is.EqualTo(2));
        }

        [Test]
        public void GetArrayLong()
        {
            JsonBuffer[] values = new JsonBuffer[4];
            int count = JsonBuffer.From("[1,2,3]").GetArray(values);
            Assert.That(count, Is.EqualTo(3));
            Assert.That((int)values[0].GetNumber(), Is.EqualTo(1));
            Assert.That((int)values[1].GetNumber(), Is.EqualTo(2));
            Assert.That((int)values[2].GetNumber(), Is.EqualTo(3));
            Assert.That(values[3].IsEmpty, Is.True);
        }

        [Test]
        public void GetArrayLengthOnNull()
        {
            Assert.That(JsonBuffer.From("[null]").GetArray()[0].GetArrayLength(), Is.EqualTo(0));
        }

        [Test]
        public void GetMemberCount()
        {
            Assert.That(JsonBuffer.From("{a:1,b:2,c:3}").GetMemberCount(), Is.EqualTo(3));
        }

        [Test]
        public void GetMembersIntoArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[3];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.That(count, Is.EqualTo(3));
            Assert.That(members[0].Name, Is.EqualTo("a"));
            Assert.That((int)members[0].Buffer.GetNumber(), Is.EqualTo(1));
            Assert.That(members[1].Name, Is.EqualTo("b"));
            Assert.That((int)members[1].Buffer.GetNumber(), Is.EqualTo(2));
            Assert.That(members[2].Name, Is.EqualTo("c"));
            Assert.That((int)members[2].Buffer.GetNumber(), Is.EqualTo(3));
        }

        [Test]
        public void GetMembersIntoShortArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[2];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.That(count, Is.EqualTo(~2));
            Assert.That(members[0].Name, Is.EqualTo("a"));
            Assert.That((int)members[0].Buffer.GetNumber(), Is.EqualTo(1));
            Assert.That(members[1].Name, Is.EqualTo("b"));
            Assert.That((int)members[1].Buffer.GetNumber(), Is.EqualTo(2));
        }

        [Test]
        public void GetMembersIntoLongArray()
        {
            NamedJsonBuffer[] members = new NamedJsonBuffer[4];
            int count = JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(members);
            Assert.That(count, Is.EqualTo(3));
            Assert.That(members[0].Name, Is.EqualTo("a"));
            Assert.That((int)members[0].Buffer.GetNumber(), Is.EqualTo(1));
            Assert.That(members[1].Name, Is.EqualTo("b"));
            Assert.That((int)members[1].Buffer.GetNumber(), Is.EqualTo(2));
            Assert.That(members[2].Name, Is.EqualTo("c"));
            Assert.That((int)members[2].Buffer.GetNumber(), Is.EqualTo(3));
            Assert.That(members[3].IsEmpty, Is.True);
        }

        [Test]
        public void GetMembersIntoArrayPermitsNull()
        {
            Assert.That(JsonBuffer.From("{a:1,b:2,c:3}").GetMembers(null), Is.EqualTo(3));
        }

        [Test]
        public void GetMembersArray()
        {
            NamedJsonBuffer[] members = JsonBuffer.From("{a:1,b:2,c:3}").GetMembersArray();
            Assert.That(members.Length, Is.EqualTo(3));
            Assert.That(members[0].Name, Is.EqualTo("a"));
            Assert.That((int)members[0].Buffer.GetNumber(), Is.EqualTo(1));
            Assert.That(members[1].Name, Is.EqualTo("b"));
            Assert.That((int)members[1].Buffer.GetNumber(), Is.EqualTo(2));
            Assert.That(members[2].Name, Is.EqualTo("c"));
            Assert.That((int)members[2].Buffer.GetNumber(), Is.EqualTo(3));
        }

        [Test]
        public void GetMembersCountOnNull()
        {
            Assert.That(JsonBuffer.From("[null]").GetArray()[0].GetMemberCount(), Is.EqualTo(0));
        }

        [Test]
        public void IsObject()
        {
            Assert.That(JsonBuffer.From("{}").IsObject, Is.True);
        }

        [Test]
        public void IsArray()
        {
            Assert.That(JsonBuffer.From("[]").IsArray, Is.True);
        }

        [Test]
        public void Equality()
        {
            Assert.That(JsonBuffer.From("[1,2,3]"), Is.EqualTo(JsonBuffer.From("[1,2,3]")));
        }

        [Test]
        public void EmptyEquality()
        {
            Assert.That(JsonBuffer.Empty, Is.EqualTo(JsonBuffer.Empty));
        }

        [Test]
        public void Inequality()
        {
            Assert.That(JsonBuffer.From("[1,2,three,4]"), Is.Not.EqualTo(JsonBuffer.From("[1,2,THREE,4]")));
        }

        [Test]
        public void OpEquality()
        {
            Assert.That(JsonBuffer.From("[1,2,3]") == JsonBuffer.From("[1,2,3]"), Is.True);
        }

        [Test]
        public void OpInequality()
        {
            Assert.That(JsonBuffer.From("[1,2,three,4]") != JsonBuffer.From("[1,2,THREE,4]"), Is.True);
        }

        [Test]
        public void StringRepresentation()
        {
            string str = JsonBuffer.From("[42,foobar,null,true,false,{x:123,y:456}]").ToString();
            Assert.That(str, Is.EqualTo("[42,\"foobar\",null,true,false,{\"x\":123,\"y\":456}]"));
        }

        [Test]
        public void StringRepresentationForEmpty()
        {
            Assert.That(JsonBuffer.Empty.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void StringRepresentationForNull()
        {
            Assert.That(JsonBuffer.From(JsonToken.Null()).ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void StringRepresentationForString()
        {
            Assert.That(JsonBuffer.From(JsonToken.String("foo\nbar")).ToString(), Is.EqualTo("\"foo\\nbar\""));
        }

        [Test]
        public void StringRepresentationForNumber()
        {
            Assert.That(JsonBuffer.From(JsonToken.Number("42")).ToString(), Is.EqualTo("42"));
        }

        [Test]
        public void StringRepresentationForTrue()
        {
            Assert.That(JsonBuffer.From(JsonToken.True()).ToString(), Is.EqualTo("true"));
        }

        [Test]
        public void StringRepresentationForFalse()
        {
            Assert.That(JsonBuffer.From(JsonToken.False()).ToString(), Is.EqualTo("false"));
        }

        [Test(Description = @"http://code.google.com/p/jayrock/issues/detail?id=26")]
        public void Issue26()
        {
            // 1. Create JsonBuffer from array with objects
            JsonBuffer buffer = JsonBuffer.From(@"[{},{a:{},b:{}}]");
            // 2. Create reader from the buffer...
            JsonBufferReader reader = buffer.CreateReader();
            //    ...read in the first object
            while (reader.TokenClass != JsonTokenClass.Object)
                reader.Read();
            reader.Read(); // Read Object token
            reader.Read(); // Read EndObject token

            //    ...create a subbuffer to buffer the next object
            JsonBuffer subBuffer = JsonBuffer.From(reader);
            //    ...create reader from the subbuffer
            JsonBufferReader reader2 = subBuffer.CreateReader();

            // 3. Call reader.BufferValue() this should break
            JsonBuffer buffer2 = reader2.BufferValue();
            Assert.That(buffer2.IsObject, Is.True);
        }

        [Test(Description = @"http://code.google.com/p/jayrock/issues/detail?id=57")]
        public void Issue57()
        {
            int maxDepth = new JsonBufferWriter().MaxDepth + 1;
            string json = new string('[', maxDepth) + new string(']', maxDepth);
            JsonBuffer.From(json);
        }

        private static void AssertBufferedValueScalarOrNull(JsonToken expected, JsonBufferWriter writer)
        {
            JsonBuffer buffer = writer.GetBuffer();
            JsonBufferReader reader = buffer.CreateReader();
            reader.Read();
            reader.Read();
            JsonBuffer value = reader.BufferValue();
            if (expected.Class == JsonTokenClass.Null)
                Assert.That(value.IsNull, Is.True);
            else
                Assert.That(value.IsScalar, Is.True);
            JsonBufferReader vr = value.CreateReader();
            Assert.That(vr.Depth, Is.EqualTo(1));
            Assert.That(vr.Token, Is.EqualTo(expected));
            vr.Read();
            vr.ReadToken(JsonTokenClass.EndArray);
            Assert.That(vr.EOF, Is.True);
        }
    }
}




