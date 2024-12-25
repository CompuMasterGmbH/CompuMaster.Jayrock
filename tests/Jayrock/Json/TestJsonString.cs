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
    #region Imports

    using System;
    using Jayrock.Tests;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonString : TestUtilityBase
    {
        [Test]
        public void NullInputYieldsQuotedEmpty()
        {
            Assert.That(JsonString.Enquote(null), Is.EqualTo("\"\""));
        }

        [Test]
        public void EmptyInputYieldsQuotedEmpty()
        {
            Assert.That(JsonString.Enquote(string.Empty), Is.EqualTo("\"\""));
        }

        [Test]
        public void BackslasheEscaping()
        {
            Assert.That(JsonString.Enquote(@"c:\foo\bar"), Is.EqualTo(@"""c:\\foo\\bar"""));
        }

        [Test]
        public void QuoteEscaping()
        {
            Assert.That(JsonString.Enquote("foo \"quoted\" bar"), Is.EqualTo("\"foo \\\"quoted\\\" bar\""));
        }

        [Test]
        public void GreaterThanEscaping()
        {
            Assert.That(JsonString.Enquote("foo <b>bold</b> bar"), Is.EqualTo("\"foo <b>bold<\\/b> bar\""));
        }

        [Test]
        public void NonPrintablesAsHex()
        {
            Assert.That(JsonString.Enquote(((char)00).ToString()), Is.EqualTo("\"\\u0000\""));
            Assert.That(JsonString.Enquote(((char)01).ToString()), Is.EqualTo("\"\\u0001\""));
            Assert.That(JsonString.Enquote(((char)02).ToString()), Is.EqualTo("\"\\u0002\""));
            Assert.That(JsonString.Enquote(((char)03).ToString()), Is.EqualTo("\"\\u0003\""));
            Assert.That(JsonString.Enquote(((char)04).ToString()), Is.EqualTo("\"\\u0004\""));
            Assert.That(JsonString.Enquote(((char)05).ToString()), Is.EqualTo("\"\\u0005\""));
            Assert.That(JsonString.Enquote(((char)06).ToString()), Is.EqualTo("\"\\u0006\""));
            Assert.That(JsonString.Enquote(((char)07).ToString()), Is.EqualTo("\"\\u0007\""));
            //  8...10
            Assert.That(JsonString.Enquote(((char)11).ToString()), Is.EqualTo("\"\\u000b\""));
            // 12...13
            Assert.That(JsonString.Enquote(((char)14).ToString()), Is.EqualTo("\"\\u000e\""));
            Assert.That(JsonString.Enquote(((char)15).ToString()), Is.EqualTo("\"\\u000f\""));
            Assert.That(JsonString.Enquote(((char)16).ToString()), Is.EqualTo("\"\\u0010\""));
            Assert.That(JsonString.Enquote(((char)17).ToString()), Is.EqualTo("\"\\u0011\""));
            Assert.That(JsonString.Enquote(((char)18).ToString()), Is.EqualTo("\"\\u0012\""));
            Assert.That(JsonString.Enquote(((char)19).ToString()), Is.EqualTo("\"\\u0013\""));
            Assert.That(JsonString.Enquote(((char)20).ToString()), Is.EqualTo("\"\\u0014\""));
            Assert.That(JsonString.Enquote(((char)21).ToString()), Is.EqualTo("\"\\u0015\""));
            Assert.That(JsonString.Enquote(((char)22).ToString()), Is.EqualTo("\"\\u0016\""));
            Assert.That(JsonString.Enquote(((char)23).ToString()), Is.EqualTo("\"\\u0017\""));
            Assert.That(JsonString.Enquote(((char)24).ToString()), Is.EqualTo("\"\\u0018\""));
            Assert.That(JsonString.Enquote(((char)25).ToString()), Is.EqualTo("\"\\u0019\""));
            Assert.That(JsonString.Enquote(((char)26).ToString()), Is.EqualTo("\"\\u001a\""));
            Assert.That(JsonString.Enquote(((char)27).ToString()), Is.EqualTo("\"\\u001b\""));
            Assert.That(JsonString.Enquote(((char)28).ToString()), Is.EqualTo("\"\\u001c\""));
            Assert.That(JsonString.Enquote(((char)29).ToString()), Is.EqualTo("\"\\u001d\""));
            Assert.That(JsonString.Enquote(((char)30).ToString()), Is.EqualTo("\"\\u001e\""));
            Assert.That(JsonString.Enquote(((char)31).ToString()), Is.EqualTo("\"\\u001f\""));
        }

        [Test]
        public void BackspaceEscaping()
        {
            Assert.That(JsonString.Enquote(((char)08).ToString()), Is.EqualTo("\"\\b\""));
        }

        [Test]
        public void TabEscaping()
        {
            Assert.That(JsonString.Enquote(((char)09).ToString()), Is.EqualTo("\"\\t\""));
        }

        [Test]
        public void LineFeedEscaping()
        {
            Assert.That(JsonString.Enquote(((char)10).ToString()), Is.EqualTo("\"\\n\""));
        }

        [Test]
        public void FormFeedEscaping()
        {
            Assert.That(JsonString.Enquote(((char)12).ToString()), Is.EqualTo("\"\\f\""));
        }

        [Test]
        public void CarriageReturnEscaping()
        {
            Assert.That(JsonString.Enquote(((char)13).ToString()), Is.EqualTo("\"\\r\""));
        }

        [Test]
        public void NullStringBuilderCreatesOne()
        {
            Assert.That(JsonString.Enquote(string.Empty, null), Is.Not.Null);
        }

        [Test]
        public void NullStringWithNullStringBuilderYieldsQuotedEmpty()
        {
            Assert.That(JsonString.Enquote(null, null).ToString(), Is.EqualTo("\"\""));
        }

        [Test]
        public void CannotEnquoteWithNullCharBuffer()
        {
            Assert.Throws<ArgumentNullException>(() => JsonString.Enquote(null, 0, 0));
        }

        [Test]
        public void CannotEnquoteWithNegativeOffset()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonString.Enquote(new char[0], -1, 0));
        }

        [Test]
        public void CannotEnquoteWithNegativeLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonString.Enquote(new char[0], 0, -1));
        }

        [Test]
        public void CannotEnquoteWithBadRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonString.Enquote(new char[10], 5, 10));
        }

        [Test]
        public void Chars()
        {
            char[] chars = "\"foo\" \"bar\" \"baz\"".ToCharArray();
            Assert.That(JsonString.Enquote(chars, 0, chars.Length), Is.EqualTo("\"\\\"foo\\\" \\\"bar\\\" \\\"baz\\\"\""));
            Assert.That(JsonString.Enquote(chars, 0, 5), Is.EqualTo("\"\\\"foo\\\"\""));
            Assert.That(JsonString.Enquote(chars, 6, 5), Is.EqualTo("\"\\\"bar\\\"\""));
            Assert.That(JsonString.Enquote(chars, 12, 5), Is.EqualTo("\"\\\"baz\\\"\""));
        }
    }
}
