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
    using System.IO;
    using System.Runtime.CompilerServices;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

#if !NET_1_0 && !NET_1_1

    using System.Collections.Generic;

#endif

#if !NET_1_0 && !NET_1_1 && !NET_2_0

    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.CSharp.RuntimeBinder;

#endif

    #endregion

    [TestFixture]
    public class TestJsonObject
    {
        [Test]
        public void Names()
        {
            JsonObject o = new JsonObject();
            o.Put("one", 1);
            o.Put("two", 2);
            ICollection names = o.Names;
            IEnumerator e = names.GetEnumerator();
            e.MoveNext(); Assert.That(e.Current, Is.EqualTo("one"));
            e.MoveNext(); Assert.That(e.Current, Is.EqualTo("two"));
        }

        [Test]
        public void InitWithKeyValuePairs()
        {
            JsonObject o = new JsonObject(new string[] { "one", "two", }, new object[] { 1, 2 });
            Assert.That(o.Count, Is.EqualTo(2));
            Assert.That(o["one"], Is.EqualTo(1));
            Assert.That(o["two"], Is.EqualTo(2));
        }

        [Test]
        public void InitWithKeyValuePairsAccumulates()
        {
            JsonObject o = new JsonObject(new string[] { "one", "two", "three", "two" }, new object[] { 1, 2, 3, 4 });
            Assert.That(o.Count, Is.EqualTo(3));
            Assert.That(o["one"], Is.EqualTo(1));
            IList two = o["two"] as IList;
            Assert.That(two, Is.Not.Null, "Member 'two' should be an ordered list of accumulated values.");
            Assert.That(two.Count, Is.EqualTo(2), "Count of values under 'two'.");
            Assert.That(two[0], Is.EqualTo(2));
            Assert.That(two[1], Is.EqualTo(4));
            Assert.That(o["three"], Is.EqualTo(3));
        }

        [Test]
        public void InitWithExtraKeys()
        {
            JsonObject o = new JsonObject(new string[] { "one", "two", "three" }, new object[] { 1, 2 });
            Assert.That(o.Count, Is.EqualTo(3));
            Assert.That(o["one"], Is.EqualTo(1));
            Assert.That(o["two"], Is.EqualTo(2));
            Assert.That(JsonNull.LogicallyEquals(o["three"]), Is.True);
        }

        [Test]
        public void InitWithNullValues()
        {
            JsonObject o = new JsonObject(new string[] { "one", "two", "three" }, null);
            Assert.That(o.Count, Is.EqualTo(3));
            Assert.That(JsonNull.LogicallyEquals(o["one"]), Is.True);
            Assert.That(JsonNull.LogicallyEquals(o["two"]), Is.True);
            Assert.That(JsonNull.LogicallyEquals(o["three"]), Is.True);
        }

        [Test]
        public void InitWithExtraValues()
        {
            JsonObject o = new JsonObject(new string[] { "one", "two" }, new object[] { 1, 2, 3, 4 });
            Assert.That(o.Count, Is.EqualTo(2));
            Assert.That(o["one"], Is.EqualTo(1));
            IList two = (IList)o["two"];
            Assert.That(two.Count, Is.EqualTo(3), "Count of values under 'two'.");
            Assert.That(two[0], Is.EqualTo(2));
            Assert.That(two[1], Is.EqualTo(3));
            Assert.That(two[2], Is.EqualTo(4));
        }

        [Test]
        public void InitWithNullKeys()
        {
            JsonObject o = new JsonObject(null, new object[] { 1, 2, 3, 4 });
            Assert.That(o.Count, Is.EqualTo(1));
            IList values = (IList)o[""];
            Assert.That(values.Count, Is.EqualTo(4), "Count of values.");
            Assert.That(values[0], Is.EqualTo(1));
            Assert.That(values[1], Is.EqualTo(2));
            Assert.That(values[2], Is.EqualTo(3));
            Assert.That(values[3], Is.EqualTo(4));
        }

        [Test]
        public void Import()
        {
            JsonObject article = new JsonObject();

            article.Import(new JsonTextReader(new StringReader(@"
                /* Article */ {
                    Title : 'Introduction to JSON',
                    Rating : 2,
                    Abstract : null,
                    Author : {
                        Name : 'John Doe',
                        'E-Mail Address' : 'john.doe@example.com' 
                    },
                    References : [
                        { Title : 'JSON RPC', Link : 'http://www.json-rpc.org/' }
                    ]
                }")));

            Assert.That(article, Is.Not.Null);
            Assert.That(article.Count, Is.EqualTo(5));
            Assert.That(article["Title"], Is.EqualTo("Introduction to JSON"));
            Assert.That((int)(JsonNumber)article["Rating"], Is.EqualTo(2));
            Assert.That(article["Abstract"], Is.Null);

            IDictionary author = (IDictionary)article["Author"];
            Assert.That(author, Is.Not.Null);
            Assert.That(author.Count, Is.EqualTo(2));
            Assert.That(author["Name"], Is.EqualTo("John Doe"));
            Assert.That(author["E-Mail Address"], Is.EqualTo("john.doe@example.com"));

            JsonArray references = (JsonArray)article["References"];
            Assert.That(references, Is.Not.Null);
            Assert.That(references.Length, Is.EqualTo(1));

            IDictionary reference = (IDictionary)references[0];
            Assert.That(reference, Is.Not.Null);
            Assert.That(reference.Count, Is.EqualTo(2));
            Assert.That(reference["Title"], Is.EqualTo("JSON RPC"));
            Assert.That(reference["Link"], Is.EqualTo("http://www.json-rpc.org/"));
        }

        [Test]
        public void ContentsClearedBeforeImporting()
        {
            JsonObject o = new JsonObject();
            o.Put("foo", "bar");
            Assert.That(o.Count, Is.EqualTo(1));
            o.Import(new JsonTextReader(new StringReader("{}")));
            Assert.That(o.Count, Is.EqualTo(0));
        }

        [Test]
        public void CannotUseNullReaderWithImport()
        {
            IJsonImportable o = new JsonObject();
            Assert.Throws<ArgumentNullException>(() => o.Import(new ImportContext(), null));
        }

        [Test]
        public void CannotUseNullContextWithImport()
        {
            IJsonImportable o = new JsonObject();
            Assert.Throws<ArgumentNullException>(() => o.Import(null, (new JsonRecorder()).CreatePlayer()));
        }

        [Test]
        public void CannotUseNullArgWithExport()
        {
            IJsonExportable o = new JsonObject();
            Assert.Throws<ArgumentNullException>(() => o.Export(null, null));
        }

        [Test]
        public void Export()
        {
            JsonObject o = new JsonObject();
            o.Put("Number", 123);
            o.Put("String", "Hello World");
            o.Put("Boolean", true);
            JsonRecorder writer = new JsonRecorder();
            o.Export(writer);
            JsonReader reader = writer.CreatePlayer();
            reader.ReadToken(JsonTokenClass.Object);
            string[] members = (string[])o.GetNamesArray().ToArray(typeof(string));
            Assert.That(reader.ReadMember(), Is.EqualTo(members[0]));
            Assert.That(reader.ReadNumber().ToInt32(), Is.EqualTo(o[members[0]]));
            Assert.That(reader.ReadMember(), Is.EqualTo(members[1]));
            Assert.That(reader.ReadString(), Is.EqualTo(o[members[1]]));
            Assert.That(reader.ReadMember(), Is.EqualTo(members[2]));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(o[members[2]]));
            Assert.That(reader.TokenClass, Is.EqualTo(JsonTokenClass.EndObject));
        }

        [Test]
        public void CannotPutUsingNullName()
        {
            Assert.Throws<ArgumentNullException>(() => (new JsonObject()).Put(null, new object()));
        }

        [Test]
        public void CannotRemoveByNullName()
        {
            Assert.Throws<ArgumentNullException>(() => (new JsonObject()).Remove(null));
        }

        [Test]
        public void CannotTestContainmentForNullName()
        {
            Assert.Throws<ArgumentNullException>(() => (new JsonObject()).Contains(null));
        }

        [Test]
        public void CannotListNamesToNullList()
        {
            Assert.Throws<ArgumentNullException>(() => (new JsonObject()).ListNames(null));
        }

        [Test]
        public void CannotExportToNullWriter()
        {
            Assert.Throws<ArgumentNullException>(() => (new SubJsonObject()).SubExport(new ExportContext(), null));
        }

        [Test]
        public void CannotAccumulateUsingNullName()
        {
            Assert.Throws<ArgumentNullException>(() => (new JsonObject()).Accumulate(null, new object()));
        }

        [Test]
        public void NoHasMemberWhenEmpty()
        {
            Assert.That((new JsonObject()).HasMembers, Is.False);
        }

        [Test]
        public void HasMemberWhenNotEmpty()
        {
            JsonObject o = new JsonObject();
            Assert.That(o.HasMembers, Is.False);
            o.Put("foo", "bar");
            Assert.That(o.HasMembers, Is.True);
        }

        [Test]
        public void Add()
        {
            JsonObject o = new JsonObject();
            o.Add("foo", "bar");
            Assert.That(o["foo"], Is.EqualTo("bar"));
            ICollection names = o.Names;
            Assert.That(names.Count, Is.EqualTo(1));
            Assert.That(CollectionHelper.ToArray(names, typeof(string)), Is.EqualTo(new string[] { "foo" }));
        }

        [Test]
        public void CannotAddMemberNameDuplicates()
        {
            JsonObject o = new JsonObject();
            o.Add("foo", "bar");
            Assert.Throws<ArgumentException>(() => o.Add("foo", "baz"));
        }

        [Test]
        public void Enumeration()
        {
            JsonObject o = new JsonObject();
            o.Add("one", 1);
            o.Add("two", 2);
            o.Add("three", 3);
            o.Add("four", 4);

            JsonObject.JsonMemberEnumerator e = o.GetEnumerator();

            Assert.That(e, Is.Not.Null);

            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current.Name, Is.EqualTo("one"));
            Assert.That(e.Current.Value, Is.EqualTo(1));

            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current.Name, Is.EqualTo("two"));
            Assert.That(e.Current.Value, Is.EqualTo(2));

            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current.Name, Is.EqualTo("three"));
            Assert.That(e.Current.Value, Is.EqualTo(3));

            Assert.That(e.MoveNext(), Is.True);
            Assert.That(e.Current.Name, Is.EqualTo("four"));
            Assert.That(e.Current.Value, Is.EqualTo(4));

            Assert.That(e.MoveNext(), Is.False);
        }

        private sealed class SubJsonObject : JsonObject
        {
            public void SubExport(ExportContext context, JsonWriter writer)
            {
                Export(context, writer);
            }
        }

#if !NET_1_0 && !NET_1_1 && !NET_2_0

        [Test]
        public void DynamicallyGetMember()
        {
            var obj = new JsonObject { { "foo", "bar" } };
            Assert.That(((dynamic)obj).foo, Is.EqualTo("bar"));
        }

        [Test, Ignore("To be completed")]
        public void DynamicallyGetMemberIgnoringCase()
        {
            var obj = new JsonObject { { "foo", "bar" } };
            var dmo = ((IDynamicMetaObjectProvider)obj).GetMetaObject(Expression.Parameter(typeof(JsonObject)));
            var s0 = CallSite<Func<CallSite, DynamicMetaObject, object>>.Create(new FakeGetMemberBinder("FOO", true));
            Assert.That(s0.Target(s0, dmo), Is.EqualTo("bar"));
        }

        private class FakeGetMemberBinder : GetMemberBinder
        {
            public FakeGetMemberBinder(string name, bool ignoreCase) :
                base(name, ignoreCase)
            { }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void CannotDynamicallyGetNonExistingMember()
        {
            var obj = new JsonObject();
            Assert.Throws<RuntimeBinderException>(() => { var unused = ((dynamic)obj).foo; });
        }

        [Test]
        public void DynamicallySetMember()
        {
            var obj = new JsonObject();
            Assert.That(obj.HasMembers, Is.False);
            ((dynamic)obj).foo = "bar";
            Assert.That(obj.HasMembers, Is.True);
            Assert.That(obj.GetNamesArray()[0], Is.EqualTo("foo"));
            Assert.That(obj["foo"], Is.EqualTo("bar"));
        }

        [Test]
        public void DynamicallyGetMemberNames()
        {
            var obj = new JsonObject { { "x", 123 }, { "y", 456 } };
            var dmo = ((IDynamicMetaObjectProvider)obj).GetMetaObject(Expression.Parameter(typeof(object)));
            using (var e = dmo.GetDynamicMemberNames().GetEnumerator())
            {
                Assert.That(e.MoveNext(), Is.True); Assert.That(e.Current, Is.EqualTo("x"));
                Assert.That(e.MoveNext(), Is.True); Assert.That(e.Current, Is.EqualTo("y"));
                Assert.That(e.MoveNext(), Is.False);
            }
        }

        // TODO Add test for dynamically deleting member

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0

#if !NET_1_0 && !NET_1_1

        [Test]
        public void AddViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            Assert.That(obj.Count, Is.EqualTo(0));
            obj.Add("one", 1);
            Assert.That(obj.Count, Is.EqualTo(1));
            obj.Add("two", 2);
            Assert.That(obj.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            Assert.That(obj.Count, Is.EqualTo(0));
            obj.Add(new KeyValuePair<string, object>("one", 1));
            Assert.That(obj.Count, Is.EqualTo(1));
            obj.Add(new KeyValuePair<string, object>("two", 2));
            Assert.That(obj.Count, Is.EqualTo(2));
        }

        [Test]
        public void CannotAddDuplicateKeyViaGenericCollection()
        {
            IDictionary<string, object> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("foo", "bar"));
            Assert.Throws<ArgumentException>(() => obj.Add(new KeyValuePair<string, object>("foo", "baz")));
        }

        [Test]
        public void KeyContainmentViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            Assert.That(obj.ContainsKey("foo"), Is.False);
            obj.Add(new KeyValuePair<string, object>("foo", "bar"));
            Assert.That(obj.ContainsKey("foo"), Is.True);
        }

        [Test]
        public void CannotTestKeyContainmentViaGenericDictionaryWithNullKey()
        {
            IDictionary<string, object> obj = new JsonObject();
            Assert.Throws<ArgumentNullException>(() => obj.ContainsKey(null));
        }

        [Test]
        public void KeysViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            obj.Add("first", 123);
            Assert.That(obj.Keys, Is.EqualTo(new string[] { "first" }));
            obj.Add("second", 456);
            Assert.That(obj.Keys, Is.EqualTo(new string[] { "first", "second" }));
        }

        [Test]
        public void ValuesViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            obj.Add("first", 123);
            Assert.That(obj.Values, Is.EqualTo(new object[] { 123 }));
            obj.Add("second", 456);
            Assert.That(obj.Values, Is.EqualTo(new object[] { 123, 456 }));
        }

        [Test]
        public void RemoveExistingViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            obj.Add("first", 123);
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove("first"), Is.True);
            Assert.That(obj.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveNonExistingViaGenericDictionary()
        {
            IDictionary<string, object> obj = new JsonObject();
            obj.Add("first", 123);
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove("second"), Is.False);
            Assert.That(obj.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveExistingViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove(new KeyValuePair<string, object>("first", 123)), Is.True);
            Assert.That(obj.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveNonExistingViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove(new KeyValuePair<string, object>("second", 456)), Is.False);
            Assert.That(obj.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveExistingKeyNonMatchingValueViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove(new KeyValuePair<string, object>("first", 456)), Is.False);
            Assert.That(obj.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveNonExistingKeyMatchingValueViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Count, Is.EqualTo(1));
            Assert.That(obj.Remove(new KeyValuePair<string, object>("second", 123)), Is.False);
            Assert.That(obj.Count, Is.EqualTo(1));
        }

        [Test]
        public void ContainsExistingViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Contains(new KeyValuePair<string, object>("first", 123)), Is.True);
        }

        [Test]
        public void ContainsNonExistingViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Contains(new KeyValuePair<string, object>("second", 456)), Is.False);
        }

        [Test]
        public void ContainsExistingKeyNonMatchingValueViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Contains(new KeyValuePair<string, object>("first", 456)), Is.False);
        }

        [Test]
        public void ContainsNonExistingKeyMatchingValueViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("first", 123));
            Assert.That(obj.Contains(new KeyValuePair<string, object>("second", 123)), Is.False);
        }

        [Test]
        public void IsReadOnlyViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            Assert.That(obj.IsReadOnly, Is.False);
        }

        [Test]
        public void CopyToViaGenericCollection()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            KeyValuePair<string, object> first = new KeyValuePair<string, object>("first", 123);
            obj.Add(first);
            KeyValuePair<string, object> second = new KeyValuePair<string, object>("second", 456);
            obj.Add(second);
            KeyValuePair<string, object>[] pairs = new KeyValuePair<string, object>[2];
            obj.CopyTo(pairs, 0);
            Assert.That(pairs[0], Is.EqualTo(first));
            Assert.That(pairs[1], Is.EqualTo(second));
        }

        [Test]
        public void CannotCopyToViaGenericCollectionWithNullArray()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            Assert.Throws<ArgumentNullException>(() => obj.CopyTo(null, 0));
        }

        [Test]
        public void CannotCopyToViaGenericCollectionWithNegativeArrayIndex()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            Assert.Throws<ArgumentOutOfRangeException>(() => obj.CopyTo(new KeyValuePair<string, object>[0], -1));
        }

        [Test]
        public void CannotCopyToViaGenericCollectionWithTooSmallArray()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();
            obj.Add(new KeyValuePair<string, object>("foo", "bar"));
            Assert.Throws<ArgumentException>(() => obj.CopyTo(new KeyValuePair<string, object>[1], 1));
        }

        [Test]
        public void KeyValuePairEnumeration()
        {
            ICollection<KeyValuePair<string, object>> obj = new JsonObject();

            KeyValuePair<string, object> first = new KeyValuePair<string, object>("first", 123);
            obj.Add(first);

            KeyValuePair<string, object> second = new KeyValuePair<string, object>("second", 456);
            obj.Add(second);

            using (IEnumerator<KeyValuePair<string, object>> e = obj.GetEnumerator())
            {
                Assert.That(e, Is.Not.Null);
                Assert.That(e.MoveNext(), Is.True);
                Assert.That(e.Current, Is.EqualTo(first));
                Assert.That(e.MoveNext(), Is.True);
                Assert.That(e.Current, Is.EqualTo(second));
                Assert.That(e.MoveNext(), Is.False);
            }
        }

        [Test]
        public void JsonMemberEnumeratorUsesSameEnumeratorAsGetEnumerator()
        {
            JsonObject obj = new JsonObject();
            Assert.That(((IEnumerable<JsonMember>)obj).GetEnumerator().GetType(), Is.EqualTo(obj.GetEnumerator().GetType()));
        }

        [Test]
        public void NonGenericEnumeratorUsesSameEnumeratorAsGetEnumerator()
        {
            JsonObject obj = new JsonObject();
            Assert.That(((IEnumerable)obj).GetEnumerator().GetType(), Is.EqualTo(obj.GetEnumerator().GetType()));
        }

#if !NET_2_0

        [Test]
        public void InitWithJsonMemberSequence()
        {
            JsonObject obj = new JsonObject(
                from i in Enumerable.Range(0, 10)
                let ch = (char)('a' + i)
                select new JsonMember(ch.ToString(), i));

            Assert.That(obj.Count, Is.EqualTo(10));
            Assert.That(obj["a"], Is.EqualTo(0));
            Assert.That(obj["b"], Is.EqualTo(1));
            Assert.That(obj["c"], Is.EqualTo(2));
            Assert.That(obj["d"], Is.EqualTo(3));
            Assert.That(obj["e"], Is.EqualTo(4));
            Assert.That(obj["f"], Is.EqualTo(5));
            Assert.That(obj["g"], Is.EqualTo(6));
            Assert.That(obj["h"], Is.EqualTo(7));
            Assert.That(obj["i"], Is.EqualTo(8));
            Assert.That(obj["j"], Is.EqualTo(9));
        }

#endif // !NET_2_0
#endif // !NET_1_0 && !NET_1_1
    }
}
