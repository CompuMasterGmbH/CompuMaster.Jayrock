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

namespace Jayrock
{
    #region Imports

    using System;
    using System.Reflection;
    using Jayrock.Tests;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestTypeResolution : TestUtilityBase
    {
        private TypeResolutionHandler _initialCurrent;

        [SetUp]
        public void Init()
        {
            _initialCurrent = TypeResolution.Current;
        }

        [TearDown]
        public void Dispose()
        {
            TypeResolution.Current = _initialCurrent;
        }

        [Test]
        public void Default()
        {
            Assert.That(TypeResolution.Default, Is.Not.Null);
        }

        [Test]
        public void Current()
        {
            Assert.That(TypeResolution.Current, Is.Not.Null);
            Assert.That(TypeResolution.Current, Is.SameAs(TypeResolution.Default));
        }

        [Test]
        public void CannotSetCurrentToNull()
        {
            Assert.Throws<ArgumentNullException>(() => TypeResolution.Current = null);
        }

        [Test]
        public void SetCurrent()
        {
            TypeResolutionHandler handler = new TypeResolutionHandler(DummyGetType);
            TypeResolution.Current = handler;
            Assert.That(TypeResolution.Current, Is.SameAs(handler));
        }

        [Test]
        public void FindTypeResolution()
        {
            TestTypeResolver resolver = new TestTypeResolver(typeof(object));
            TypeResolution.Current = new TypeResolutionHandler(resolver.Resolve);
            Assert.That(TypeResolution.FindType("obj"), Is.SameAs(typeof(object)));
            Assert.That(resolver.LastTypeName, Is.EqualTo("obj"), "typeName");
            Assert.That(resolver.LastThrowOnError, Is.False, "throwOnError");
            Assert.That(resolver.LastIgnoreCase, Is.False, "ignoreCase");
        }

        [Test]
        public void GetTypeResolution()
        {
            TestTypeResolver resolver = new TestTypeResolver(typeof(object));
            TypeResolution.Current = new TypeResolutionHandler(resolver.Resolve);
            Assert.That(TypeResolution.GetType("obj"), Is.SameAs(typeof(object)));
            Assert.That(resolver.LastTypeName, Is.EqualTo("obj"), "typeName");
            Assert.That(resolver.LastThrowOnError, Is.True, "throwOnError");
            Assert.That(resolver.LastIgnoreCase, Is.False, "ignoreCase");
        }

        private static Type DummyGetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        private sealed class TestTypeResolver
        {
            public string LastTypeName;
            public bool LastThrowOnError;
            public bool LastIgnoreCase;
            public Type NextResult;

            public TestTypeResolver(Type nextResult)
            {
                NextResult = nextResult;
            }

            public Type Resolve(string typeName, bool throwOnError, bool ignoreCase)
            {
                LastTypeName = typeName;
                LastThrowOnError = throwOnError;
                LastIgnoreCase = ignoreCase;
                return NextResult;
            }
        }
    }
}





