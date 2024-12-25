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

#if !NET_1_0 && !NET_1_1 && !NET_2_0

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Text;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestExpandoObjectImporter
    {
        [Test]
        public void Superclass()
        {
            Assert.That(new ExpandoObjectImporter(), Is.InstanceOf<ImporterBase>());
        }

        [Test]
        public void InputTypeIsExpandoObject()
        {
            Assert.That((new ExpandoObjectImporter()).OutputType, Is.SameAs(typeof(ExpandoObject)));
        }

        [Test]
        public void Export()
        {
            var context = JsonConvert.CreateImportContext();
            var importer = new ExpandoObjectImporter();
            var expando = importer.Import(context, JsonText.CreateReader(@"
            {
                num: 123, flag: true, str: foobar, nil: null,
                obj: { x: 456, y: 789 },
                arr: [ 123, { x: 456, y: 789 } ],
            }"));
            Assert.That(expando, Is.Not.Null);
            Assert.That(expando, Is.InstanceOf<ExpandoObject>());
            dynamic obj = expando;
            Assert.That(((JsonNumber)obj.num).ToInt32(), Is.EqualTo(123));
            Assert.That((bool)obj.flag, Is.True);
            Assert.That((string)obj.str, Is.EqualTo("foobar"));
            Assert.That((object)obj.obj, Is.InstanceOf<ExpandoObject>());
            Assert.That(((JsonNumber)obj.obj.x).ToInt32(), Is.EqualTo(456));
            Assert.That(((JsonNumber)obj.obj.y).ToInt32(), Is.EqualTo(789));
            Assert.That(typeof(IList<object>).IsAssignableFrom(((object)obj.arr).GetType()), Is.True);
            var arr = (IList<object>)obj.arr;
            Assert.That(((JsonNumber)arr[0]).ToInt32(), Is.EqualTo(123));
            Assert.That(arr[1], Is.InstanceOf<ExpandoObject>());
            Assert.That(((JsonNumber)(((dynamic)(arr[1])).x)).ToInt32(), Is.EqualTo(456));
            Assert.That(((JsonNumber)(((dynamic)(arr[1])).y)).ToInt32(), Is.EqualTo(789));
        }
    }
}

#endif // !NET_1_0 && !NET_1_1 && !NET_2_0
