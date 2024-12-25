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

namespace Jayrock.Services
{
    #region Imports

    using System;
    using System.Reflection;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestMethodBuilder
    {
        private MethodBuilder _builder;
        private ServiceClassBuilder _classBuilder;

        [SetUp]
        public void Init()
        {
            _classBuilder = new ServiceClassBuilder();
            _builder = _classBuilder.DefineMethod();
        }

        [Test]
        public void Defaults()
        {
            Assert.That(_builder.Name, Is.Not.Null);
            Assert.That(_builder.InternalName, Is.Not.Null);
            Assert.That(_builder.ResultType, Is.SameAs(typeof(void)));
            Assert.That(_builder.Handler, Is.Null);
            Assert.That(_builder.Description, Is.Not.Null);
            Assert.That(_builder.Description.Length, Is.EqualTo(0));
            Assert.That(_builder.ServiceClass, Is.Not.Null);
            Assert.That(_builder.Idempotent, Is.False);
            Assert.That(_builder.HasParameters, Is.False);
            Assert.That(_builder.CustomAttributes, Is.Not.Null);
        }

        [Test]
        public void GetSetName()
        {
            const string name = "MyMethod";
            _builder.Name = name;
            Assert.That(_builder.Name, Is.EqualTo(name));
        }

        [Test]
        public void GetSetInternalName()
        {
            const string name = "MyMethod";
            _builder.InternalName = name;
            Assert.That(_builder.InternalName, Is.EqualTo(name));
        }

        [Test]
        public void GetSetReturnType()
        {
            Type type = typeof(int);
            _builder.ResultType = type;
            Assert.That(_builder.ResultType, Is.SameAs(type));
        }

        [Test]
        public void ReturnTypeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _builder.ResultType = null);
        }

        [Test]
        public void HasParameters()
        {
            Assert.That(_builder.HasParameters, Is.False);
            _builder.DefineParameter();
            Assert.That(_builder.HasParameters, Is.True);
        }

        [Test]
        public void DefineTwoParameters()
        {
            Assert.That(_builder.DefineParameter(), Is.Not.Null);
            Assert.That(_builder.DefineParameter(), Is.Not.Null);
            ParameterBuilder[] parameters = (ParameterBuilder[])CollectionHelper.ToArray(_builder.Parameters, typeof(ParameterBuilder));
            Assert.That(parameters, Is.Not.Null);
            Assert.That(parameters.Length, Is.EqualTo(2));
        }

        [Test]
        public void GetSetHandler()
        {
            IMethodImpl handler = new DummyMethod();
            _builder.Handler = handler;
            Assert.That(_builder.Handler, Is.SameAs(handler));
        }

        [Test]
        public void ParametersAreAutoPositioned()
        {
            Assert.That(_builder.DefineParameter().Position, Is.EqualTo(0));
            Assert.That(_builder.DefineParameter().Position, Is.EqualTo(1));
            Assert.That(_builder.DefineParameter().Position, Is.EqualTo(2));
        }

        [Test]
        public void CannotSetCustomAttributesToNull()
        {
            Assert.Throws<ArgumentNullException>(() => _builder.CustomAttributes = null);
        }

        [Test]
        public void CustomAttributes()
        {
            TestCustomAttributeProvider attributes = new TestCustomAttributeProvider();
            _builder.CustomAttributes = attributes;
            Assert.That(_builder.CustomAttributes, Is.SameAs(attributes));
        }

        [Test]
        public void GetSetDescription()
        {
            const string name = "Test description of a method.";
            _builder.Description = name;
            Assert.That(_builder.Description, Is.EqualTo(name));
        }

        [Test]
        public void ServiceClassReference()
        {
            Assert.That(_builder.ServiceClass, Is.SameAs(_classBuilder));
        }

        [Test]
        public void GetSetIdempotent()
        {
            _builder.Idempotent = true;
            Assert.That(_builder.Idempotent, Is.True);
        }

        [Test]
        public void ParametersCollectionIsReadOnly()
        {
            Assert.That(_builder.Parameters.IsReadOnly, Is.True);
        }

        private sealed class DummyMethod : IMethodImpl
        {
            public object Invoke(IService service, object[] args) { throw new NotImplementedException(); }
            public bool IsAsynchronous { get { throw new NotImplementedException(); } }
            public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState) { throw new NotImplementedException(); }
            public object EndInvoke(IService service, IAsyncResult asyncResult) { throw new NotImplementedException(); }
        }

        private sealed class TestCustomAttributeProvider : ICustomAttributeProvider
        {
            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }
        }
    }
}




