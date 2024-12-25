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
    public class TestFreeJsonMemberReadingHelper
    {
        [Test]
        public void CannotInitializeWithNullReader()
        {
            Assert.Throws<ArgumentNullException>(() => new FreeJsonMemberReadingHelper(null));
        }

        [Test]
        public void BaseReaderInitialization()
        {
            JsonReader reader = JsonText.CreateReader("[]");
            FreeJsonMemberReadingHelper helper = new FreeJsonMemberReadingHelper(reader);
            Assert.That(helper.BaseReader, Is.SameAs(reader));
        }

        [Test]
        public void StringRepresentation()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("[]");
            Assert.That(helper.ToString(), Is.EqualTo(helper.BaseReader.ToString()));
        }

        [Test]
        public void UnorderedReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.That(helper.ReadMember("x").ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(helper.ReadMember("y").ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(helper.ReadMember("z").ReadNumber().ToInt32(), Is.EqualTo(789));
            Assert.That(helper.BaseReader.TokenClass, Is.EqualTo(JsonTokenClass.EndObject));
        }

        [Test]
        public void OrderedReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ x: 123, y: 456, z: 789 }");
            Assert.That(helper.ReadMember("x").ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(helper.ReadMember("y").ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(helper.ReadMember("z").ReadNumber().ToInt32(), Is.EqualTo(789));
            Assert.That(helper.BaseReader.TokenClass, Is.EqualTo(JsonTokenClass.EndObject));
        }

        [Test]
        public void TailMemberAfterUnorderedReader()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"
                { y: 456, x: 123, z: 789, comment: tail }");
            Assert.That(helper.ReadMember("x").ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(helper.ReadMember("y").ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(helper.ReadMember("z").ReadNumber().ToInt32(), Is.EqualTo(789));
            JsonReader reader = helper.BaseReader;
            Assert.That(reader.ReadMember(), Is.EqualTo("comment"));
            Assert.That(reader.ReadString(), Is.EqualTo("tail"));
        }

        [Test]
        public void CannotReadNonExistingMember()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.Throws<JsonException>(() => helper.ReadMember("foo"));
        }

        [Test]
        public void TryReadNonExistingMemberGivenEmptyObject()
        {
            Assert.That(CreateHelper("{}").TryReadMember("foo"), Is.Null);
        }

        [Test]
        public void TryReadNonExistingMember()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.That(helper.TryReadMember("foo"), Is.Null);
        }

        [Test]
        public void TryReadMemberCaseSensitivity()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.That(helper.TryReadMember("X"), Is.Null);
        }

        [Test]
        public void TailReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"
                { y: 456, 
                  x: 123, 
                  z: 789, 
                  obj1: { foo: bar }, 
                  comment: null, 
                  arr: [ 123, 456, 789 ],
                  obj2: { a: 1, b: 2, }, }");
            Assert.That(helper.ReadMember("x").ReadNumber().ToInt32(), Is.EqualTo(123));
            helper.ReadMember("comment").ReadNull();
            JsonReader reader = helper.BaseReader;
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.Member));
            Assert.That(reader.Text, Is.EqualTo("arr"));
            JsonReader tail = helper.GetTailReader();
            Assert.That(tail.ReadMember(), Is.EqualTo("y"));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(tail.ReadMember(), Is.EqualTo("z"));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(789));
            Assert.That(tail.ReadMember(), Is.EqualTo("obj1"));
            tail.ReadToken(JsonTokenClass.Object);
            Assert.That(tail.ReadMember(), Is.EqualTo("foo"));
            Assert.That(tail.ReadString(), Is.EqualTo("bar"));
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.That(tail.ReadMember(), Is.EqualTo("arr"));
            tail.ReadToken(JsonTokenClass.Array);
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(789));
            tail.ReadToken(JsonTokenClass.EndArray);
            Assert.That(tail.ReadMember(), Is.EqualTo("obj2"));
            tail.StepOut();
            Assert.That(reader.Read(), Is.False);
        }

        [Test]
        public void EmptyTailReading()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper(@"{ y: 456, x: 123, z: 789 }");
            Assert.That(helper.ReadMember("x").ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(helper.ReadMember("y").ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(helper.ReadMember("z").ReadNumber().ToInt32(), Is.EqualTo(789));
            JsonReader tail = helper.GetTailReader();
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.That(tail.Read(), Is.False);
        }

        [Test]
        public void TailReadingWithNoBufferedMembers()
        {
            JsonReader tail = CreateHelper(@"{ y: 456, x: 123, z: 789 }").GetTailReader();
            Assert.That(tail.ReadMember(), Is.EqualTo("y"));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(456));
            Assert.That(tail.ReadMember(), Is.EqualTo("x"));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(123));
            Assert.That(tail.ReadMember(), Is.EqualTo("z"));
            Assert.That(tail.ReadNumber().ToInt32(), Is.EqualTo(789));
            tail.ReadToken(JsonTokenClass.EndObject);
            Assert.That(tail.Read(), Is.False);
        }

        [Test]
        public void CannotTailReadEmptySource()
        {
            Assert.Throws<JsonException>(() => CreateHelper(string.Empty).GetTailReader());
        }

        [Test]
        public void CannotTailReadOnArray()
        {
            Assert.Throws<JsonException>(() => CreateHelper("[]").GetTailReader());
        }

        [Test]
        public void CannotReadMemberOnArray()
        {
            Assert.Throws<JsonException>(() => CreateHelper("[]").ReadMember("foo"));
        }

        [Test]
        public void CannotReadMemberOnString()
        {
            Assert.Throws<JsonException>(() => CreateHelper("foo").ReadMember("foo"));
        }

        [Test]
        public void CannotReadMemberOnNumber()
        {
            Assert.Throws<JsonException>(() => CreateHelper("123").ReadMember("foo"));
        }

        [Test]
        public void ScopedToCurrentObject()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("[{},{foo:bar}]");
            helper.BaseReader.ReadToken(JsonTokenClass.Array);
            Assert.That(helper.TryReadMember("foo"), Is.Null);
            Assert.That(helper.TryReadMember("foo"), Is.Null);
        }

        [Test]
        public void ReadingSameMemberMoreThanOnce()
        {
            FreeJsonMemberReadingHelper helper = CreateHelper("{foo:bar}");
            helper.ReadMember("foo").Skip();
            Assert.That(helper.TryReadMember("foo"), Is.Null);
        }

        [Test]
        public void CannotInitializeWithReaderOnEnd()
        {
            JsonReader reader = JsonText.CreateReader("42");
            reader.ReadNumber();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void CannotInitializeWithReaderOnNull()
        {
            JsonReader reader = JsonText.CreateReader("null");
            reader.MoveToContent();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void CannotInitializeWithReaderOnNumber()
        {
            JsonReader reader = JsonText.CreateReader("42");
            reader.MoveToContent();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void CannotInitializeWithReaderOnBoolean()
        {
            JsonReader reader = JsonText.CreateReader("true");
            reader.MoveToContent();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void CannotInitializeWithReaderOnString()
        {
            JsonReader reader = JsonText.CreateReader("foobar");
            reader.MoveToContent();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void CannotInitializeWithReaderOnArray()
        {
            JsonReader reader = JsonText.CreateReader("[]");
            reader.MoveToContent();
            Assert.Throws<ArgumentException>(() => new FreeJsonMemberReadingHelper(reader));
        }

        [Test]
        public void InitializeWithReaderOnObjectStart()
        {
            JsonReader reader = JsonText.CreateReader("{}");
            reader.MoveToContent();
            new FreeJsonMemberReadingHelper(reader);
        }

        [Test]
        public void InitializeWithReaderOnMember()
        {
            JsonReader reader = JsonText.CreateReader("{foo:bar}");
            reader.MoveToContent();
            reader.ReadToken(JsonTokenClass.Object);
            new FreeJsonMemberReadingHelper(reader);
        }

        private static FreeJsonMemberReadingHelper CreateHelper(string json)
        {
            return new FreeJsonMemberReadingHelper(JsonText.CreateReader(json));
        }
    }
}



