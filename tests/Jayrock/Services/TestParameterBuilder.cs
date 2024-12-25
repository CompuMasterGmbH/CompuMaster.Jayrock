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
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestParameterBuilder
    {
        private MethodBuilder _methodBuilder;
        private ParameterBuilder _builder;

        [SetUp]
        public void Init()
        {
            _methodBuilder = (new ServiceClassBuilder()).DefineMethod();
            _builder = _methodBuilder.DefineParameter();
        }

        [Test]
        public void Defaults()
        {
            Assert.That(_builder.Name, Is.Not.Null);
            Assert.That(_builder.Position, Is.EqualTo(0));
            Assert.That(_builder.ParameterType, Is.SameAs(typeof(object)));
            Assert.That(_builder.IsParamArray, Is.False);
            Assert.That(_builder.Method, Is.Not.Null);
        }

        [Test]
        public void GetSetName()
        {
            const string name = "MyParameter";
            _builder.Name = name;
            Assert.That(_builder.Name, Is.EqualTo(name));
        }

        [Test]
        public void GetSetReturnType()
        {
            Type type = typeof(int);
            _builder.ParameterType = type;
            Assert.That(_builder.ParameterType, Is.SameAs(type));
        }

        [Test]
        public void GetPosition()
        {
            Assert.That(_builder.Position, Is.EqualTo(0));
            Assert.That(_methodBuilder.DefineParameter().Position, Is.EqualTo(1));
        }

        [Test]
        public void ReturnTypeCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => _builder.ParameterType = null);
        }

        [Test]
        public void GetSetIsParamArray()
        {
            _builder.IsParamArray = true;
            Assert.That(_builder.IsParamArray, Is.True);
        }

        [Test]
        public void MethodReference()
        {
            Assert.That(_builder.Method, Is.SameAs(_methodBuilder));
        }
    }
}
