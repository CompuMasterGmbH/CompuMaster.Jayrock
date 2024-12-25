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

namespace Jayrock.Reflection
{
    #region Imports

    using System;
    using System.Reflection;
    using Jayrock.Tests;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestReflector : TestUtilityBase
    {
#if !NET_1_0 && !NET_1_1

        [Test]
        public void CannotIsConstructionOfNullableWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => Reflector.IsConstructionOfNullable(null));
        }

        [Test]
        public void IsConstructionOfNullable()
        {
            Assert.That(Reflector.IsConstructionOfNullable(typeof(int)), Is.False);
            Assert.That(Reflector.IsConstructionOfNullable(typeof(System.Collections.Generic.IList<int>)), Is.False);
            Assert.That(Reflector.IsConstructionOfNullable(typeof(Nullable<>)), Is.False);
            Assert.That(Reflector.IsConstructionOfNullable(typeof(int?)), Is.True);
        }

#endif // !NET_1_0 && !NET_1_1

#if !NET_1_0 && !NET_1_1 && !NET_2_0

        [Test]
        public void CannotIsTupleFamilyWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => Reflector.IsTupleFamily(null));
        }

        [Test]
        public void IsTupleFamilyOnTupleGenericTypeDefinition()
        {
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<>)), Is.False);
        }

        [Test]
        public void IsTupleFamilyOnNonGenericType()
        {
            Assert.That(Reflector.IsTupleFamily(typeof(object)), Is.False);
        }

        [Test]
        public void IsTupleFamily()
        {
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int, int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int, int, int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int, int, int, int, int>)), Is.True);
            Assert.That(Reflector.IsTupleFamily(typeof(Tuple<int, int, int, int, int, int, int, int>)), Is.True);
        }

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0
    }
}



