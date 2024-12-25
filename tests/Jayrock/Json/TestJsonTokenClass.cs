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
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonTokenClass
    {
        [Test]
        public void Names()
        {
            Assert.That(JsonTokenClass.Null.Name, Is.EqualTo("Null"));
            Assert.That(JsonTokenClass.Array.Name, Is.EqualTo("Array"));
            Assert.That(JsonTokenClass.String.Name, Is.EqualTo("String"));
            Assert.That(JsonTokenClass.Boolean.Name, Is.EqualTo("Boolean"));
            Assert.That(JsonTokenClass.Number.Name, Is.EqualTo("Number"));
            Assert.That(JsonTokenClass.Object.Name, Is.EqualTo("Object"));
        }

        [Test]
        public void ToStringReturnsName()
        {
            foreach (JsonTokenClass tokenClass in JsonTokenClass.All)
                Assert.That(tokenClass.Name, Is.EqualTo(tokenClass.ToString()));
        }

        [Test]
        public void HashCodeFromName()
        {
            foreach (JsonTokenClass tokenClass in JsonTokenClass.All)
                Assert.That(tokenClass.GetHashCode(), Is.EqualTo(tokenClass.Name.GetHashCode()));
        }

        [Test]
        public void AllIsReadOnly()
        {
            Assert.That(((IList)JsonTokenClass.All).IsReadOnly, Is.True);
        }

        [Test]
        public void AllIsFixedSize()
        {
            Assert.That(((IList)JsonTokenClass.All).IsFixedSize, Is.True);
        }

        [Test]
        public void All()
        {
            ArrayList list = new ArrayList();

            foreach (FieldInfo field in typeof(JsonTokenClass).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (typeof(JsonTokenClass).IsAssignableFrom(field.FieldType))
                    list.Add(field.GetValue(null));
            }

            Assert.That(JsonTokenClass.All.Count, Is.EqualTo(list.Count));

            foreach (JsonTokenClass tokenClass in JsonTokenClass.All)
                list.Remove(tokenClass);

            if (list.Count > 0)
                Assert.Fail(string.Format("{0} not found in All collection.", list[0]));
        }

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
        public void ObjectToken()
        {
            Assert.That(JsonToken.Object().Class, Is.EqualTo(JsonTokenClass.Object));
            Assert.That(JsonToken.EndObject().Class, Is.EqualTo(JsonTokenClass.EndObject));
        }

        [Test]
        public void ArrayToken()
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

#if NETFRAMEWORK
        [Test]
        public void DeserializesToFactoryInstance()
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, JsonTokenClass.Null);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.That(formatter.Deserialize(stream), Is.SameAs(JsonTokenClass.Null));
        }
#endif

        [Test]
        public void EqualityByReference()
        {
            foreach (JsonTokenClass clazz in JsonTokenClass.All)
                Assert.That(clazz.Equals(clazz), Is.True, clazz.ToString());
        }

        [Test]
        public void InequalityWithNull()
        {
            foreach (JsonTokenClass clazz in JsonTokenClass.All)
                Assert.That(clazz.Equals(null), Is.False, clazz.ToString());
        }

        [Test]
        public void InequalityWithIncompatibleType()
        {
            foreach (JsonTokenClass clazz in JsonTokenClass.All)
                Assert.That(clazz.Equals(new object()), Is.False, clazz.ToString());
        }
    }
}



