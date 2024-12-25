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
    public class TestJsonWriterBase
    {
        private JsonWriter _writer;

        [SetUp]
        public void Init()
        {
            _writer = new EmptyJsonWriter();
        }

        [Test]
        public void StartingDepthIsZero()
        {
            Assert.That(_writer.Depth, Is.EqualTo(0));
        }

        [Test]
        public void DepthIncreasesInsideObject()
        {
            _writer.WriteStartObject();
            Assert.That(_writer.Depth, Is.EqualTo(1));
            _writer.WriteMember(string.Empty);
            _writer.WriteStartObject();
            Assert.That(_writer.Depth, Is.EqualTo(2));
            _writer.WriteEndObject();
            Assert.That(_writer.Depth, Is.EqualTo(1));
            _writer.WriteEndObject();
            Assert.That(_writer.Depth, Is.EqualTo(0));
        }

        [Test]
        public void DepthIncreasesInsideArray()
        {
            _writer.WriteStartArray();
            Assert.That(_writer.Depth, Is.EqualTo(1));
            _writer.WriteStartArray();
            Assert.That(_writer.Depth, Is.EqualTo(2));
            _writer.WriteEndArray();
            Assert.That(_writer.Depth, Is.EqualTo(1));
            _writer.WriteEndArray();
            Assert.That(_writer.Depth, Is.EqualTo(0));
        }

        [Test]
        public void CannotWriteObjectAfterRoot()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            Assert.Throws<JsonException>(() => _writer.WriteStartObject());
        }

        [Test]
        public void CannotWriteArrayAfterRoot()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            Assert.Throws<JsonException>(() => _writer.WriteStartArray());
        }

        [Test]
        public void CannotWriteMemberInsideArray()
        {
            _writer.WriteStartArray();
            Assert.Throws<JsonException>(() => _writer.WriteMember("Member"));
        }

        [Test]
        public void CannotEndObjectWithArray()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteEndArray());
        }

        [Test]
        public void CannotEndArrayWithObject()
        {
            _writer.WriteStartArray();
            Assert.Throws<JsonException>(() => _writer.WriteEndObject());
        }

        [Test]
        public void CannotWriteStringWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteString("string"));
        }

        [Test]
        public void CannotWriteNullWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteNull());
        }

        [Test]
        public void CannotWriteNumberWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteNumber(123));
        }

        [Test]
        public void CannotWriteBooleanWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteBoolean(true));
        }

        [Test]
        public void CannotWriteNestedObjectWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteStartObject());
        }

        [Test]
        public void CannotWriteNestedArrayWithoutMember()
        {
            _writer.WriteStartObject();
            Assert.Throws<JsonException>(() => _writer.WriteStartArray());
        }

        [Test]
        public void WriteStringInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteString("string");
        }

        [Test]
        public void WriteNumberInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteNumber(123);
        }

        [Test]
        public void WriteBooleanInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteBoolean(true);
        }

        [Test]
        public void WriteNullInsideArray()
        {
            _writer.WriteStartArray();
            _writer.WriteNull();
        }

        [Test]
        public void WriteObjectMemberString()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteString("string");
        }

        [Test]
        public void WriteObjectMemberNumber()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteNumber(123);
        }

        [Test]
        public void WriteObjectMemberBoolean()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteBoolean(true);
        }

        [Test]
        public void WriteObjectMemberNull()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member");
            _writer.WriteNull();
        }

        [Test]
        public void WriteObjectWithNullMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteNull();
            _writer.WriteMember("member2");
            _writer.WriteNull();
        }

        [Test]
        public void WriteObjectWithNumberMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteNumber(123);
            _writer.WriteMember("member2");
            _writer.WriteNumber(456);
        }

        [Test]
        public void WriteObjectWithBooleanMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteBoolean(true);
            _writer.WriteMember("member2");
            _writer.WriteBoolean(false);
        }

        [Test]
        public void WriteObjectWithStringMembers()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("member1");
            _writer.WriteString("string1");
            _writer.WriteMember("member2");
            _writer.WriteString("string2");
        }

        [Test]
        public void InitialBracket()
        {
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Pending));
        }

        [Test]
        public void BracketAfterStartingObject()
        {
            _writer.WriteStartObject();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Object));
        }

        [Test]
        public void BracketAfterStartingArray()
        {
            _writer.WriteStartArray();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Array));
        }

        [Test]
        public void BracketAfterWritingMember()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("test");
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Member));
        }

        [Test]
        public void BracketAfterWritingMemberValue()
        {
            _writer.WriteStartObject();
            _writer.WriteMember("test");
            _writer.WriteNull();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Object));
        }

        [Test]
        public void BracketAfterEndingObject()
        {
            _writer.WriteStartObject();
            _writer.WriteEndObject();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Closed));
        }

        [Test]
        public void BracketAfterEndingArray()
        {
            _writer.WriteStartArray();
            _writer.WriteEndArray();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Closed));
        }

        [Test]
        public void NestedBracketing()
        {
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Pending));
            _writer.WriteStartArray();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Array));
            _writer.WriteStartObject();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Object));
            _writer.WriteEndObject();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Array));
            _writer.WriteEndArray();
            Assert.That(_writer.Bracket, Is.EqualTo(JsonWriterBracket.Closed));
        }

        [Test]
        public void InitialIndex()
        {
            Assert.That(_writer.Index, Is.EqualTo(-1));
        }

        [Test]
        public void IndexZeroOnStartingObject()
        {
            _writer.WriteStartObject();
            Assert.That(_writer.Index, Is.EqualTo(0));
        }

        [Test]
        public void IndexZeroAfertStartingArray()
        {
            _writer.WriteStartArray();
            Assert.That(_writer.Index, Is.EqualTo(0));
        }

        [Test]
        public void IndexRunInsideArray()
        {
            _writer.WriteStartArray();
            Assert.That(_writer.Index, Is.EqualTo(0));
            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(1));
            _writer.WriteString("foo");
            Assert.That(_writer.Index, Is.EqualTo(2));
            _writer.WriteNumber(42);
            Assert.That(_writer.Index, Is.EqualTo(3));
            _writer.WriteBoolean(true);
            Assert.That(_writer.Index, Is.EqualTo(4));
            _writer.WriteEndArray();
            Assert.That(_writer.Index, Is.EqualTo(-1));
        }

        [Test]
        public void IndexRunInsideObject()
        {
            _writer.WriteStartObject();
            Assert.That(_writer.Index, Is.EqualTo(0));

            _writer.WriteMember("m1");
            Assert.That(_writer.Index, Is.EqualTo(0));
            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(1));

            _writer.WriteMember("m2");
            Assert.That(_writer.Index, Is.EqualTo(1));
            _writer.WriteString("foo");
            Assert.That(_writer.Index, Is.EqualTo(2));

            _writer.WriteMember("m3");
            Assert.That(_writer.Index, Is.EqualTo(2));
            _writer.WriteNumber(42);
            Assert.That(_writer.Index, Is.EqualTo(3));

            _writer.WriteMember("m4");
            Assert.That(_writer.Index, Is.EqualTo(3));
            _writer.WriteBoolean(true);
            Assert.That(_writer.Index, Is.EqualTo(4));

            _writer.WriteEndObject();
            Assert.That(_writer.Index, Is.EqualTo(-1));
        }

        [Test]
        public void IndexingAcrossNestings()
        {
            _writer.WriteStartObject();

            _writer.WriteMember("m1");
            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(1));

            _writer.WriteMember("m2");
            _writer.WriteStartArray();
            Assert.That(_writer.Index, Is.EqualTo(0));

            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(1));

            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(2));

            _writer.WriteNull();
            Assert.That(_writer.Index, Is.EqualTo(3));

            _writer.WriteEndArray();
            Assert.That(_writer.Index, Is.EqualTo(2));
        }

        [Test]
        public void DefaultMaxDepth()
        {
            Assert.That(_writer.MaxDepth, Is.EqualTo(30));
        }

        [Test]
        public void SetMaxDepth()
        {
            Assert.That(_writer.MaxDepth, Is.Not.EqualTo(42));
            _writer.MaxDepth = 42;
            Assert.That(_writer.MaxDepth, Is.EqualTo(42));
        }

        [Test]
        public void CannotExceedMaxDepth()
        {
            _writer.MaxDepth = 2;
            _writer.WriteStartArray();
            _writer.WriteStartArray();
            Assert.Throws<Exception>(() => _writer.WriteStartArray());
        }
    }
}


