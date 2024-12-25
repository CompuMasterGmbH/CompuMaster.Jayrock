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
    using Jayrock.JsonRpc;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestServiceClass
    {
        [Test]
        public void NullType()
        {
            Assert.Throws<ArgumentNullException>(() => JsonRpcServices.GetClassFromType(null));
        }

        [Test]
        public void ServiceNameIsTypeName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.That(clazz.Name, Is.EqualTo("EmptyService"));
        }

        [Test]
        public void UntaggedMethodsNotExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.That(clazz.GetMethods().Length, Is.EqualTo(0));
        }

        [Test]
        public void TaggedMethodsExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.GetMethods().Length, Is.EqualTo(2));
        }

        [Test]
        public void CustomServiceName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.Name, Is.EqualTo("MyService"));
        }

        [Test]
        public void MethodLookupIsCaseInsensitive()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.FindMethodByName("FOO"), Is.Not.Null);
        }

        private sealed class EmptyService
        {
        }

        [JsonRpcService("MyService")]
        private sealed class TestService
        {
            [JsonRpcMethod]
            public void Foo() { }

            [JsonRpcMethod]
            public void Bar() { }
        }
    }
}



