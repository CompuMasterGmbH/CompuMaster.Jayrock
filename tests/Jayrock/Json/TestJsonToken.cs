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
    public class TestJsonToken
    {
        [Test]
        public void NullToken()
        {
            Assert.That(JsonToken.Null().Class, Is.EqualTo(JsonTokenClass.Null));
        }

        [Test]
        public void StringToken()
        {
            JsonToken token = JsonToken.String("hello");
            Assert.That(token.Class, Is.EqualTo(JsonTokenClass.String));
            Assert.That(token.Text, Is.EqualTo("hello"));
        }

        [Test]
        public void StringTokenNeverNull()
        {
            Assert.That(JsonToken.String(null).Text, Is.Not.Null);
        }

        [Test]
        public void NumberToken()
        {
            JsonToken token = JsonToken.Number("123");
            Assert.That(token.Class, Is.EqualTo(JsonTokenClass.Number));
            Assert.That(token.Text, Is.EqualTo("123"));
        }

        [Test]
        public void NumberTokenTextCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => JsonToken.Number(null));
        }

        [Test]
        public void NumberTokenTextCannotBeZeroLength()
        {
            Assert.Throws<ArgumentException>(() => JsonToken.Number(""));
        }

        [Test]
        public void BooleanToken()
        {
            JsonToken token = JsonToken.Boolean(true);
            Assert.That(token.Class, Is.EqualTo(JsonTokenClass.Boolean));
            Assert.That(token.Text, Is.EqualTo("true"));

            token = JsonToken.Boolean(false);
            Assert.That(token.Class, Is.EqualTo(JsonTokenClass.Boolean));
            Assert.That(token.Text, Is.EqualTo("false"));
        }

        [Test]
        public void ObjectTokens()
        {
            Assert.That(JsonToken.Object().Class, Is.EqualTo(JsonTokenClass.Object));
            Assert.That(JsonToken.EndObject().Class, Is.EqualTo(JsonTokenClass.EndObject));
        }

        [Test]
        public void MemberToken()
        {
            JsonToken token = JsonToken.Member("test");
            Assert.That(token.Class, Is.EqualTo(JsonTokenClass.Member));
            Assert.That(token.Text, Is.EqualTo("test"));
        }

        [Test]
        public void ArrayTokens()
        {
            Assert.That(JsonToken.Array().Class, Is.EqualTo(JsonTokenClass.Array));
            Assert.That(JsonToken.EndArray().Class, Is.EqualTo(JsonTokenClass.EndArray));
        }

        [Test]
        public void BOFEOF()
        {
            Assert.That(JsonToken.BOF().Class, Is.EqualTo(JsonTokenClass.BOF));
            Assert.That(JsonToken.EOF().Class, Is.EqualTo(JsonTokenClass.EOF));
        }

        [Test]
        public void JsonTokenString()
        {
            Assert.That(JsonToken.String("hello").ToString(), Is.EqualTo("String:hello"));
            Assert.That(JsonToken.Number("123").ToString(), Is.EqualTo("Number:123"));
            Assert.That(JsonToken.Boolean(true).ToString(), Is.EqualTo("Boolean:true"));
            Assert.That(JsonToken.Boolean(false).ToString(), Is.EqualTo("Boolean:false"));
            Assert.That(JsonToken.Null().ToString(), Is.EqualTo("Null:null"));
            Assert.That(JsonToken.Array().ToString(), Is.EqualTo("Array"));
            Assert.That(JsonToken.EndArray().ToString(), Is.EqualTo("EndArray"));
            Assert.That(JsonToken.Object().ToString(), Is.EqualTo("Object"));
            Assert.That(JsonToken.EndObject().ToString(), Is.EqualTo("EndObject"));
            Assert.That(JsonToken.Member("test").ToString(), Is.EqualTo("Member:test"));
            Assert.That(JsonToken.BOF().ToString(), Is.EqualTo("BOF"));
            Assert.That(JsonToken.EOF().ToString(), Is.EqualTo("EOF"));
        }

        [Test]
        public void EqualityWhenSameClassAndText()
        {
            Assert.That(JsonToken.String("hello").Equals(JsonToken.String("hello")), Is.True);
        }

        [Test]
        public void InEqualityWhenSameClassDifferentText()
        {
            Assert.That(JsonToken.String("hello").Equals(JsonToken.String("world")), Is.False);
        }

        [Test]
        public void InEqualityWhenDifferentClassSameText()
        {
            Assert.That(JsonToken.String("123").Equals(JsonToken.Number("123")), Is.False);
        }

        [Test]
        public void EqualityWithTextlessClass()
        {
            Assert.That(JsonToken.BOF(), Is.EqualTo(JsonToken.BOF()));
        }

        [Test]
        public void InEqualityWithNull()
        {
            Assert.That(JsonToken.Null().Equals(null), Is.False);
        }

        [Test]
        public void InEqualityWithAnotherType()
        {
            Assert.That(JsonToken.Null().Equals(123), Is.False);
        }

        [Test]
        public void HashCodeNonZero()
        {
            Assert.That(JsonToken.EOF().GetHashCode(), Is.Not.EqualTo(0));
            Assert.That(JsonToken.String("string").GetHashCode(), Is.Not.EqualTo(0));
        }
    }
}


