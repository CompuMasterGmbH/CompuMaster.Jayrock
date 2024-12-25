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
    public class TestNamedJsonBuffer
    {
        [Test]
        public void IsSerializable()
        {
            Assert.That(typeof(NamedJsonBuffer).IsSerializable, Is.True);
        }

        [Test]
        public void EmptyHasNullName()
        {
            Assert.That(NamedJsonBuffer.Empty.Name, Is.Null);
        }

        [Test]
        public void EmptyHasEmptyBuffer()
        {
            Assert.That(NamedJsonBuffer.Empty.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyIsEmpty()
        {
            Assert.That(NamedJsonBuffer.Empty.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyToString()
        {
            string str = NamedJsonBuffer.Empty.ToString();
            Assert.That(str, Is.Not.Null);
            Assert.That(str.Length, Is.EqualTo(0));
        }

        [Test]
        public void CannotBeInitializedWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new NamedJsonBuffer(null, JsonBuffer.From("null")));
        }

        [Test]
        public void CannotBeInitializedWithEmptyBuffer()
        {
            Assert.Throws<ArgumentException>(() => new NamedJsonBuffer("foo", JsonBuffer.Empty));
        }

        [Test]
        public void Initialzation()
        {
            NamedJsonBuffer nb = new NamedJsonBuffer("foo", JsonBuffer.From("bar"));
            Assert.That(nb.Name, Is.EqualTo("foo"));
            Assert.That(nb.Buffer.GetString(), Is.EqualTo("bar"));
        }

        [Test]
        public void Equality()
        {
            NamedJsonBuffer nb1 = new NamedJsonBuffer("foo", JsonBuffer.From("bar"));
            NamedJsonBuffer nb2 = new NamedJsonBuffer("foo", JsonBuffer.From("bar"));
            Assert.That(nb1, Is.EqualTo(nb2));
        }

        [Test]
        public void NameInequality()
        {
            NamedJsonBuffer nb1 = new NamedJsonBuffer("foo", JsonBuffer.From("bar"));
            NamedJsonBuffer nb2 = new NamedJsonBuffer("FOO", JsonBuffer.From("bar"));
            Assert.That(nb1, Is.Not.EqualTo(nb2));
        }

        [Test]
        public void BufferInequality()
        {
            NamedJsonBuffer nb1 = new NamedJsonBuffer("foo", JsonBuffer.From("bar"));
            NamedJsonBuffer nb2 = new NamedJsonBuffer("foo", JsonBuffer.From("123"));
            Assert.That(nb1, Is.Not.EqualTo(nb2));
        }

        [Test]
        public void AnonymousToString()
        {
            Assert.That(new NamedJsonBuffer(string.Empty, JsonBuffer.From("foo")).ToString(), Is.EqualTo("(anonymous): \"foo\""));
        }

        [Test]
        public void ToObject()
        {
            JsonBuffer obj = NamedJsonBuffer.ToObject(
                new NamedJsonBuffer("foo", JsonBuffer.From("bar")),
                new NamedJsonBuffer("qux", JsonBuffer.From("quux")));
            Assert.That(obj.Length, Is.EqualTo(6));
            Assert.That(obj[0], Is.EqualTo(JsonToken.Object()));
            Assert.That(obj[1], Is.EqualTo(JsonToken.Member("foo")));
            Assert.That(obj[2], Is.EqualTo(JsonToken.String("bar")));
            Assert.That(obj[3], Is.EqualTo(JsonToken.Member("qux")));
            Assert.That(obj[4], Is.EqualTo(JsonToken.String("quux")));
            Assert.That(obj[5], Is.EqualTo(JsonToken.EndObject()));
        }
    }
}




