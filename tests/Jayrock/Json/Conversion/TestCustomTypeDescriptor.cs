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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Runtime.CompilerServices;
    using Jayrock.Json.Conversion.Converters;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestCustomTypeDescriptor
    {
        [Test]
        public void MembersWithIgnoreAttributeExcluded()
        {
            CustomTypeDescriptor thingType = new CustomTypeDescriptor(typeof(Thing));
            PropertyDescriptorCollection properties = thingType.GetProperties();
            Assert.That(properties.Count, Is.EqualTo(4));
            Assert.That(properties.Find("Field2", true), Is.Null);
            Assert.That(properties.Find("Property2", true), Is.Null);
        }

        [Test]
        public void IndexerPropertyExcluded()
        {
            //
            // Exercises bug #11675
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=11675&group_id=4638
            //

            CustomTypeDescriptor thingType = new CustomTypeDescriptor(typeof(ThingWithIndexer));
            Assert.That(thingType.GetProperties().Count, Is.EqualTo(0));
        }

        [Test]
        public void TypeFieldDescriptorSupportsCustomization()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.That(property, Is.InstanceOf<IPropertyCustomization>());
        }

        [Test]
        public void TypePropertyDescriptorSupportsCustomization()
        {
            PropertyDescriptor property = Thing.GetProperty1Property();
            Assert.That(property, Is.InstanceOf<IPropertyCustomization>());
        }

        [Test]
        public void PropertyNameCustomization()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.That(property.Name, Is.EqualTo("Field1"));
            IPropertyCustomization customization = (IPropertyCustomization)property;
            customization.SetName("FIELD1");
            Assert.That(property.Name, Is.EqualTo("FIELD1"));
        }

        [Test]
        public void CannotCustomizePropertyWithNullName()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.Throws<ArgumentNullException>(() => ((IPropertyCustomization)property).SetName(null));
        }

        [Test]
        public void CannotCustomizePropertyWithEmptyName()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.Throws<ArgumentException>(() => ((IPropertyCustomization)property).SetName(string.Empty));
        }

        [Test]
        public void PropertyTypeCustomization()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.That(property.PropertyType, Is.EqualTo(typeof(object)));
            IPropertyCustomization customization = (IPropertyCustomization)property;
            customization.SetType(typeof(string));
            Assert.That(property.PropertyType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void CannotCustomizePropertyWithNullType()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.Throws<ArgumentNullException>(() => ((IPropertyCustomization)property).SetType(null));
        }

        [Test]
        public void PropertyGetterCustomization()
        {
            Thing thing = new Thing();
            PropertyDescriptor property = Thing.GetField1Property();
            const string testValue = "test";
            thing.Field1 = testValue;
            Assert.That(thing.Field1, Is.EqualTo(testValue));
            IPropertyCustomization customization = (IPropertyCustomization)property;
            FakePropertyImpl impl = new FakePropertyImpl();
            impl.BaseImpl = customization.OverrideImpl(impl);
            Assert.That(impl.BaseImpl, Is.Not.Null);
            Assert.That(property.GetValue(thing), Is.EqualTo("<<" + testValue));
        }

        [Test]
        public void CannotCustomizePropertyWithNullImpl()
        {
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.Throws<ArgumentNullException>(() => ((IPropertyCustomization)property).OverrideImpl(null));
        }

        [Test]
        public void PropertySetterCustomization()
        {
            Thing thing = new Thing();
            PropertyDescriptor property = Thing.GetField1Property();
            Assert.That(thing.Field1, Is.Null);
            IPropertyCustomization customization = (IPropertyCustomization)property;
            FakePropertyImpl impl = new FakePropertyImpl();
            impl.BaseImpl = customization.OverrideImpl(impl);
            property.SetValue(thing, "test");
            Assert.That(thing.Field1, Is.EqualTo(">>test"));
        }

        [Test]
        public void AddServiceToField()
        {
            AddServiceToServiceContainer((IServiceContainer)Thing.GetField1Property());
        }

        [Test]
        public void CannotAddServiceTwiceToField()
        {
            Assert.Throws<ArgumentException>(() => CannotAddServiceTwiceToServiceContainer((IServiceContainer)Thing.GetField1Property()));
        }

        [Test]
        public void AddThenRemoveServiceFromField()
        {
            AddThenRemoveService((IServiceContainer)Thing.GetField1Property());
        }

        [Test]
        public void AddServiceToProperty()
        {
            AddServiceToServiceContainer((IServiceContainer)Thing.GetProperty1Property());
        }

        [Test]
        public void CannotAddServiceTwiceToProperty()
        {
            Assert.Throws<ArgumentException>(() => CannotAddServiceTwiceToServiceContainer((IServiceContainer)Thing.GetProperty1Property()));
        }

        [Test]
        public void AddThenRemoveServiceFromProperty()
        {
            AddThenRemoveService((IServiceContainer)Thing.GetProperty1Property());
        }

        [Test]
        public void ReadOnlyFieldHasReadOnlyDescriptor()
        {
            Assert.That(Thing.GetReadOnlyFieldProperty().IsReadOnly, Is.True);
        }

        [Test]
        public void CannotSetReadOnlyFieldViaDescriptor()
        {
            Assert.Throws<NotSupportedException>(() => Thing.GetReadOnlyFieldProperty().SetValue(new Thing(), new object()));
        }

        [Test]
        public void ReadOnlyPropertyHasReadOnlyDescriptor()
        {
            Assert.That(Thing.GetReadOnlyPropertyProperty().IsReadOnly, Is.True);
        }

        [Test]
        public void CannotSetReadOnlyPropertyViaDescriptor()
        {
            Assert.Throws<NotSupportedException>(() => Thing.GetReadOnlyPropertyProperty().SetValue(new Thing(), new object()));
        }

        [Test]
        public void ReadOnlyPropertyWithIncludeAttributeIsIncluded()
        {
            CustomTypeDescriptor thingType = new CustomTypeDescriptor(typeof(ThingWithSpecialReadOnlyProperty));
            PropertyDescriptorCollection properties = thingType.GetProperties();
            Assert.That(properties.Count, Is.EqualTo(1));
        }

        [Test]
        public void AnonymousClassPropertiesExcepted()
        {
#if NET_1_0 || NET_1_1
            CustomTypeDescriptor anon = CustomTypeDescriptor.TryCreateForAnonymousClass(typeof(AnonymousThing));
#else
            CustomTypeDescriptor anon = CustomTypeDescriptor.TryCreateForAnonymousClass(typeof(AnonymousThing<string>));
#endif
            Assert.That(anon, Is.Not.Null);
            PropertyDescriptorCollection properties = anon.GetProperties();
            Assert.That(properties.Count, Is.EqualTo(1));
            PropertyDescriptor property = properties[0];
            Assert.That(property.Name, Is.EqualTo("Value"));
            Assert.That(property.IsReadOnly, Is.True);
        }

        [Test]
        public void ImmutableClassPropertiesExpected()
        {
            CustomTypeDescriptor descriptor = new CustomTypeDescriptor(typeof(ImmutableThing));
            PropertyDescriptorCollection properties = descriptor.GetProperties();
            Assert.That(properties.Count, Is.EqualTo(2));
            Assert.That(properties["field"], Is.Not.Null);
            Assert.That(properties["property"], Is.Not.Null);
        }

        private static void AddServiceToServiceContainer(IServiceContainer sc)
        {
            object service = new object();
            Type serviceType = service.GetType();
            Assert.That(sc.GetService(serviceType), Is.Null);
            sc.AddService(serviceType, service);
            Assert.That(sc.GetService(serviceType), Is.SameAs(service));
        }

        private static void CannotAddServiceTwiceToServiceContainer(IServiceContainer sc)
        {
            object service = new object();
            Type serviceType = service.GetType();
            Assert.That(sc.GetService(serviceType), Is.Null);
            sc.AddService(serviceType, service);
            sc.AddService(serviceType, service);
        }

        private static void AddThenRemoveService(IServiceContainer sc)
        {
            object service = new object();
            Type serviceType = service.GetType();
            Assert.That(sc.GetService(serviceType), Is.Null);
            sc.AddService(serviceType, service);
            Assert.That(sc.GetService(serviceType), Is.SameAs(service));
            sc.RemoveService(serviceType);
            Assert.That(sc.GetService(serviceType), Is.Null);
        }

        public sealed class ImmutableThing
        {
            public readonly object Field;

            private object _property;
            public object Property { get { return _property; } }

            public ImmutableThing(object field, object property)
            {
                Field = field;
                _property = property;
            }
        }

        public sealed class Thing
        {
            public readonly object ReadOnlyField = null;
            public object Field1;
            [JsonIgnore] public object Field2;
            public object Field3;

            public object Property1 { get { return null; } set { } }
            [JsonIgnore] public object Property2 { get { return null; } set { } }
            public object Property3 { get { return null; } set { } }
            public object ReadOnlyProperty { get { return null; } }

            public static PropertyDescriptor GetField1Property()
            {
                return CustomTypeDescriptor.CreateProperty(typeof(Thing).GetField("Field1"));
            }

            public static PropertyDescriptor GetReadOnlyFieldProperty()
            {
                return CustomTypeDescriptor.CreateProperty(typeof(Thing).GetField("ReadOnlyField"));
            }

            public static PropertyDescriptor GetProperty1Property()
            {
                return CustomTypeDescriptor.CreateProperty(typeof(Thing).GetProperty("Property1"));
            }

            public static PropertyDescriptor GetReadOnlyPropertyProperty()
            {
                return CustomTypeDescriptor.CreateProperty(typeof(Thing).GetProperty("ReadOnlyProperty"));
            }
        }

        public class ThingWithIndexer
        {
            public object this[int index]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }

        private sealed class ThingWithSpecialReadOnlyProperty
        {
            [JsonExport]
            public object SpecialReadOnlyProperty { get { return null; } }
        }

        private sealed class FakePropertyImpl : IPropertyImpl
        {
            public IPropertyImpl BaseImpl;

            public object GetValue(object obj)
            {
                return "<<" + BaseImpl.GetValue(obj);
            }

            public void SetValue(object obj, object value)
            {
                BaseImpl.SetValue(obj, ">>" + value);
            }
        }
    }

#if NET_1_0 || NET_1_1
    [CompilerGenerated]
    internal sealed class AnonymousThing
    {
        private readonly string _value;

        AnonymousThing(string value)
        {
            _value = value;
        }

        public string Value { get { return _value; } }
    }
#else
    [CompilerGenerated]
    internal sealed class AnonymousThing<T>
    {
        private readonly T _value;

        AnonymousThing(T value)
        {
            _value = value;
        }

        public T Value { get { return _value; } }
    }
#endif
}

#if NET_1_0 || NET_1_1
namespace System.Runtime.CompilerServices
{
    [SerializableAttribute] 
    [AttributeUsageAttribute(AttributeTargets.All, Inherited=true)] 
    internal sealed class CompilerGeneratedAttribute : Attribute {}
}
#endif



