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

namespace Jayrock.Collections
{
    #region Imports

    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestKeyedCollection
    {
        [Test]
        public void AddValue()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values[value.Name], Is.SameAs(value));
        }

        [Test]
        public void CannotAddWithNullKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            Assert.Throws<ArgumentException>(() => values.Add(new NamedValue(null, new object())));
        }

        [Test]
        public void ValueContainment()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            Assert.That(values.Contains(value.Name), Is.False);
            values.Add(value);
            Assert.That(values.Contains(value.Name), Is.True);
        }

        [Test]
        public void CannotUseNullKeyForContainmentTest()
        {
            NamedValueCollection values = new NamedValueCollection();
            Assert.Throws<ArgumentNullException>(() => values.Contains(null));
        }

        [Test]
        public void RemoveValue()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values.Remove(value.Name), Is.True);
            Assert.That(values.Contains(value.Name), Is.False);
            Assert.That(values.Count, Is.EqualTo(0));
        }

        [Test]
        public void CannotRemoveNullKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            Assert.Throws<ArgumentNullException>(() => values.Remove(null));
        }

        [Test]
        public void RemoveByIndex()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.That(values.Contains(value.Name), Is.True);
            Assert.That(values.Count, Is.EqualTo(1));
            values.RemoveAt(0);
            Assert.That(values.Contains(value.Name), Is.False);
            Assert.That(values.Count, Is.EqualTo(0));
        }

        [Test]
        public void ResetValueByIndex()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.That(values.Contains(value.Name), Is.True);
            Assert.That(values.Count, Is.EqualTo(1));
            NamedValue newValue = new NamedValue("Bar", new object());
            ((IList)values)[0] = newValue;
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values.Contains(value.Name), Is.False);
            Assert.That(values.Contains(newValue.Name), Is.True);
        }

        [Test]
        public void Clear()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value = new NamedValue("Foo", new object());
            values.Add(value);
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values.Contains(value.Name), Is.True);
            values.Clear();
            Assert.That(values.Count, Is.EqualTo(0));
            Assert.That(values.Contains(value.Name), Is.False);
        }

        [Test]
        public void GetKeys()
        {
            NamedValueCollection values = new NamedValueCollection();
            string[] names = new string[] { "one", "two", "three" };
            foreach (string name in names)
                values.Add(new NamedValue(name, new object()));
            Assert.That(values.Count, Is.EqualTo(names.Length));
            string[] keys = values.GetKeys();
            Assert.That(keys, Is.EqualTo(names));
        }

        [Test]
        public void Enumeration()
        {
            NamedValueCollection values = new NamedValueCollection();
            NamedValue value1 = new NamedValue("one", new object()); values.Add(value1);
            NamedValue value2 = new NamedValue("two", new object()); values.Add(value2);
            NamedValue value3 = new NamedValue("three", new object()); values.Add(value3);
            Assert.That(values.Count, Is.EqualTo(3));
            IEnumerator e = values.GetEnumerator();
            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current, Is.SameAs(value1));
            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current, Is.SameAs(value2));
            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current, Is.SameAs(value3));
            Assert.That(e.MoveNext(), Is.False);
        }

        [Test]
        public void RemoveNonExistingKey()
        {
            NamedValueCollection values = new NamedValueCollection();
            Assert.That(values.Remove("something"), Is.False);
        }

        [Test]
        public void CannotSendNullArrayToBaseListKeysByIndex()
        {
            Assert.Throws<ArgumentNullException>(() => (new NamedValueCollection()).ListKeysTo(null));
        }

        [Test]
        public void CannotSendMultiDimensionalArrayToBaseListKeysByIndex()
        {
            Assert.Throws<ArgumentException>(() => (new NamedValueCollection()).ListKeysTo(new string[0, 0]));
        }

        [Test]
        public void CannotIndexByNullKey()
        {
            Assert.Throws<ArgumentNullException>(() => new NamedValueCollection()[null].ToString());
        }

        [Test]
        public void AddsNoSerializableState()
        {
            MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(KeyedCollection));
            foreach (MemberInfo member in members)
                Assert.That(member.DeclaringType, Is.Not.EqualTo(typeof(KeyedCollection)));
        }

#if NETFRAMEWORK
        //not supported at .NET any more: BinaryFormatter: "BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information."
        [Test]
        public void MappingIsRestoredUponDeserialization()
        {
            NamedValueCollection collection = new NamedValueCollection();
            Assert.That(collection.OnDeserializationCallbackCalled, Is.False);
            collection.Add(new NamedValue("foo", "bar"));
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, collection);
            ms.Position = 0; // rewind
            collection = (NamedValueCollection)formatter.Deserialize(ms);
            Assert.That(collection.OnDeserializationCallbackCalled, Is.True);
            NamedValue entry = collection["foo"];
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.Name, Is.EqualTo("foo"));
            Assert.That(entry.Value, Is.EqualTo("bar"));
        }
#endif

        [Serializable]
        private sealed class NamedValue
        {
            public string Name;
            public object Value;

            public NamedValue(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        [Serializable]
        private sealed class NamedValueCollection : KeyedCollection
        {
            public bool OnDeserializationCallbackCalled;

            public NamedValue this[string key]
            {
                get { return (NamedValue)GetByKey(key); }
            }

            public void Add(NamedValue value)
            {
                base.Add(value);
            }

            public bool Contains(string key)
            {
                return base.Contains(key);
            }

            public bool Remove(string key)
            {
                return base.Remove(key);
            }

            public string[] GetKeys()
            {
                string[] keys = new string[Count];
                ListKeysByIndex(keys);
                return keys;
            }

            public void ListKeysTo(Array keys)
            {
                ListKeysByIndex(keys);
            }

            protected override object KeyFromValue(object value)
            {
                return ((NamedValue)value).Name;
            }

            protected override void OnDeserializationCallback(object sender)
            {
                OnDeserializationCallbackCalled = true;
                base.OnDeserializationCallback(sender);
            }
        }
    }
}


