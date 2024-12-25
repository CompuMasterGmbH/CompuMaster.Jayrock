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
    using Conversion;
    using NUnit.Framework;

    [TestFixture]
    public class TestJsonMember
    {
        [Test]
        public void Default()
        {
            JsonMember member = new JsonMember();
            Assert.That(member.Name, Is.EqualTo(string.Empty));
            Assert.That(member.Value, Is.Null);
            Assert.That(member.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NameWithNullValue()
        {
            JsonMember member = new JsonMember("foo", null);
            Assert.That(member.Name, Is.EqualTo("foo"));
            Assert.That(member.Value, Is.Null);
            Assert.That(member.ToString(), Is.EqualTo("foo: "));
        }

        [Test]
        public void NameWithNonNullValue()
        {
            JsonMember member = new JsonMember("foo", "bar");
            Assert.That(member.Name, Is.EqualTo("foo"));
            Assert.That(member.Value, Is.EqualTo("bar"));
            Assert.That(member.ToString(), Is.EqualTo("foo: bar"));
        }

        [Test]
        public void NameValueExported()
        {
            Assert.That(JsonConvert.ExportToString(new JsonMember("foo", "bar")),
                Is.EqualTo(@"{""name"":""foo"",""value"":""bar""}"));
        }
    }
}
