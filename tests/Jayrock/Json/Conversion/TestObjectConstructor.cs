#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
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

namespace Jayrock.Tests.Json.Conversion
{
    #region Imports

    using System;
    using System.Collections;
    using System.Reflection;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestObjectConstructor
    {
        [Test]
        public void CannotInitializeWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectConstructor(null));
        }

        [Test]
        public void CannotInitializeWithConstructorsFromDifferentTypes()
        {
            ConstructorInfo[] fc = typeof(FooBase).GetConstructors();
            Assert.That(fc.Length, Is.EqualTo(1));
            ConstructorInfo[] fdc = typeof(FooDerived).GetConstructors();
            Assert.That(fdc.Length, Is.EqualTo(1));
            ConstructorInfo[] ctors = new ConstructorInfo[fc.Length + fdc.Length];
            Array.Copy(fc, 0, ctors, 0, fc.Length);
            Array.Copy(fdc, 0, ctors, fc.Length, fdc.Length);
            Assert.Throws<ArgumentException>(() => new ObjectConstructor(typeof(FooDerived), ctors));
        }

        class FooBase { }
        class FooDerived : FooBase { }

        [Test]
        public void CannotInitializeWithNonConstructibleType()
        {
            Assert.Throws<ArgumentException>(() => new ObjectConstructor(typeof(PrivateThing)));
        }

        class PrivateThing { private PrivateThing() { } }

        [Test]
        public void CannotInitializeWithZeroConstructors()
        {
            Assert.Throws<ArgumentException>(() => new ObjectConstructor(typeof(object), new ConstructorInfo[0]));
        }

        [Test]
        public void Construction()
        {
            ObjectConstructor ctor = new ObjectConstructor(typeof(Point));
            ImportContext context = JsonConvert.CreateImportContext();
            const string json = "{ y: 456, x: 123 }";
            ObjectConstructionResult result = ctor.CreateObject(context, JsonText.CreateReader(json));
            Point point = (Point)result.Object;
            Assert.That(point.X, Is.EqualTo(123));
            Assert.That(point.Y, Is.EqualTo(456));
            Assert.That(JsonBuffer.From(result.TailReader).GetMemberCount(), Is.EqualTo(0));
        }

        [Test]
        public void ConstructionWithTail()
        {
            ObjectConstructor ctor = new ObjectConstructor(typeof(Point));
            ImportContext context = JsonConvert.CreateImportContext();
            const string json = "{ y: 456, z: 789, x: 123 }";
            ObjectConstructionResult result = ctor.CreateObject(context, JsonText.CreateReader(json));
            Point point = (Point)result.Object;
            Assert.That(point.X, Is.EqualTo(123));
            Assert.That(point.Y, Is.EqualTo(456));
            NamedJsonBuffer[] tail = JsonBuffer.From(result.TailReader).GetMembersArray();
            Assert.That(tail.Length, Is.EqualTo(1));
            NamedJsonBuffer z = tail[0];
            Assert.That(z.Name, Is.EqualTo("z"));
            Assert.That(z.Buffer.GetNumber().ToInt32(), Is.EqualTo(789));
        }

        [Test]
        public void CannotCreateIfNoneConstructorsMatch()
        {
            ObjectConstructor ctor = new ObjectConstructor(typeof(Point));
            ImportContext context = JsonConvert.CreateImportContext();
            Assert.Throws<JsonException>(() => ctor.CreateObject(context, JsonText.CreateReader("{ z: x: 123 }")));
        }

        class Point
        {
            private readonly int _x;
            private readonly int _y;

            public Point(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public int X { get { return _x; } }
            public int Y { get { return _y; } }
        }

        [Test]
        public void ConstructorSpecificity()
        {
            Thing thing = CreateThing("{ num: 42 }");
            Assert.That(thing.Number, Is.EqualTo(42));
            Assert.That(thing.Constructor, Is.EqualTo(GetThingConstructor(typeof(int))));

            thing = CreateThing("{ num: 42, flag: true }");
            Assert.That(thing.Number, Is.EqualTo(42));
            Assert.That(thing.Flag, Is.True);
            Assert.That(thing.Constructor, Is.EqualTo(GetThingConstructor(typeof(int), typeof(bool))));

            thing = CreateThing("{ num: 42, flag: true, str: hello }");
            Assert.That(thing.Number, Is.EqualTo(42));
            Assert.That(thing.Flag, Is.True);
            Assert.That(thing.String, Is.EqualTo("hello"));
            Assert.That(thing.Constructor, Is.EqualTo(GetThingConstructor(typeof(int), typeof(bool), typeof(string))));

            thing = CreateThing("{ num: 42, flag: true, str: hello, obj: { a: 123, b: 456 } }");
            Assert.That(thing.Number, Is.EqualTo(42));
            Assert.That(thing.Flag, Is.True);
            Assert.That(thing.String, Is.EqualTo("hello"));
            Assert.That(thing.Object.Count, Is.EqualTo(2));
            Assert.That(Convert.ToInt32(thing.Object["a"]), Is.EqualTo(123));
            Assert.That(Convert.ToInt32(thing.Object["b"]), Is.EqualTo(456));
            Assert.That(thing.Constructor, Is.EqualTo(GetThingConstructor(typeof(int), typeof(bool), typeof(string), typeof(IDictionary))));
        }

        private static ConstructorInfo GetThingConstructor(params Type[] types)
        {
            return typeof(Thing).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types, null);
        }

        private Thing CreateThing(string json)
        {
            ObjectConstructor ctor = new ObjectConstructor(typeof(Thing));
            ImportContext context = JsonConvert.CreateImportContext();
            ObjectConstructionResult result = ctor.CreateObject(context, JsonText.CreateReader(json));
            return (Thing)result.Object;
        }

        class Thing
        {
            private readonly int _num;
            private readonly bool _flag;
            private readonly string _str;
            private readonly IDictionary _obj;
            private readonly ConstructorInfo _ctor;

            // ReSharper disable UnusedMember.Local
            public Thing(int num)
                : this(num, false, null, null, (ConstructorInfo)MethodBase.GetCurrentMethod()) { }

            public Thing(int num, bool flag)
                : this(num, flag, null, null, (ConstructorInfo)MethodBase.GetCurrentMethod()) { }

            public Thing(int num, bool flag, string str) :
                this(num, flag, str, null, (ConstructorInfo)MethodBase.GetCurrentMethod())
            { }

            public Thing(int num, bool flag, string str, IDictionary obj) :
                this(num, flag, str, obj, (ConstructorInfo)MethodBase.GetCurrentMethod())
            { }

            private Thing(int num, bool flag, string str, IDictionary obj, ConstructorInfo ctor)
            {
                _num = num;
                _flag = flag;
                _str = str;
                _obj = obj;
                _ctor = ctor;
            }
            // ReSharper restore UnusedMember.Local

            public int Number { get { return _num; } }
            public bool Flag { get { return _flag; } }
            public string String { get { return _str; } }
            public IDictionary Object { get { return _obj; } }
            public ConstructorInfo Constructor { get { return _ctor; } }
        }

        [Test]
        public void ValueTypeWithDefaultConstructorConstruction()
        {
            ObjectConstructor constructor = new ObjectConstructor(typeof(ValueThing));
            ObjectConstructionResult result = constructor.CreateObject(new ImportContext(), JsonText.CreateReader("{foo:bar}"));
            Assert.That(result.Object, Is.InstanceOf<ValueThing>());
            JsonReader tail = result.TailReader;
            tail.ReadToken(JsonTokenClass.Object);
            Assert.That(tail.ReadMember(), Is.EqualTo("foo"));
            Assert.That(tail.ReadString(), Is.EqualTo("bar"));
            tail.ReadToken(JsonTokenClass.EndObject);
        }

        struct ValueThing { }
    }
}




