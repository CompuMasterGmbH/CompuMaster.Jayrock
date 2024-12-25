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

namespace Jayrock.Json
{
    #region Imports

    using System;
    using Conversion;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestObjectSurrogateConstructor
    {
        [Test]
        public void CannotInitializeWithNullSurrogateType()
        {
            Assert.Throws<ArgumentNullException>(() => new ObjectSurrogateConstructor(null));
        }

        [Test]
        public void CannotInitializeWithNonSurrogateTypeNotImpementingIObjectSurrogateConstructor()
        {
            Assert.Throws<ArgumentException>(() => new ObjectSurrogateConstructor(typeof(object)));
        }

        [Test]
        public void SurrogateTypeInitialization()
        {
            ObjectSurrogateConstructor ctor = new ObjectSurrogateConstructor(typeof(Surrogate));
            Assert.That(ctor.SurrogateType, Is.EqualTo(typeof(Surrogate)));
        }

        [Test]
        public void CannotCreateObjectWithNullImportContext()
        {
            ObjectSurrogateConstructor ctor = new ObjectSurrogateConstructor(typeof(Surrogate));
            Assert.Throws<ArgumentNullException>(() => ctor.CreateObject(null, StockJsonBuffers.EmptyObject.CreateReader()));
        }

        [Test]
        public void CannotCreateObjectWithNullJsonReader()
        {
            ObjectSurrogateConstructor ctor = new ObjectSurrogateConstructor(typeof(Surrogate));
            Assert.Throws<ArgumentNullException>(() => ctor.CreateObject(new ImportContext(), null));
        }

        [Test]
        public void CreateObject()
        {
            ObjectSurrogateConstructor ctor = new ObjectSurrogateConstructor(typeof(Surrogate));
            ImportContext context = new ImportContext();
            ObjectConstructionResult result = ctor.CreateObject(context, JsonText.CreateReader("{y:2000,m:12,d:4}"));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Object, Is.EqualTo(new DateTime(2000, 12, 4)));
        }

        public class Surrogate : IObjectSurrogateConstructor
        {
            [JsonMemberName("y")] public int Year;
            [JsonMemberName("m")] public int Month;
            [JsonMemberName("d")] public int Day;

            public ObjectConstructionResult CreateObject(ImportContext context)
            {
                return new ObjectConstructionResult(new DateTime(Year, Month, Day), StockJsonBuffers.EmptyObject.CreateReader());
            }
        }
    }
}



