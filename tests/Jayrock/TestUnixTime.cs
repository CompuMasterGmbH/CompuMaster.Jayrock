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
    public class TestUnixTime : TestUtilityBase
    {
        [Test]
        public void IntegralConversions()
        {
            DateTime t1 = new DateTime(2006, 7, 17, 10, 56, 56);
            const long u1 = 1153133816;

            DateTime t2 = new DateTime(2006, 7, 17, 11, 00, 44);
            const long u2 = 1153134044;

            Assert.That(UnixTime.ToDateTime(u1).ToUniversalTime(), Is.EqualTo(t1));
            Assert.That(UnixTime.ToDateTime(u2).ToUniversalTime(), Is.EqualTo(t2));

            Assert.That(UnixTime.ToInt64(t1.ToLocalTime()), Is.EqualTo(u1));
            Assert.That(UnixTime.ToInt64(t2.ToLocalTime()), Is.EqualTo(u2));
        }

        [Test]
        public void Milliseconds()
        {
            DateTime t1 = new DateTime(2006, 7, 17, 10, 56, 56, 456);
            const double u1 = 1153133816.456;

            DateTime t2 = new DateTime(2006, 7, 17, 11, 00, 44, 567);
            const double u2 = 1153134044.567;

            DateTime ut1 = UnixTime.ToDateTime(u1).ToUniversalTime();
            Assert.That(ut1, Is.EqualTo(t1), $"ms expected = {t1.Millisecond}, ms actual = {ut1.Millisecond}");

            DateTime ut2 = UnixTime.ToDateTime(u2).ToUniversalTime();
            Assert.That(ut2, Is.EqualTo(t2), $"ms expected = {t2.Millisecond}, ms actual = {ut2.Millisecond}");

            Assert.That(UnixTime.ToDouble(t1.ToLocalTime()), Is.EqualTo(u1).Within(0.0001));
            Assert.That(UnixTime.ToDouble(t2.ToLocalTime()), Is.EqualTo(u2).Within(0.0001));

            Assert.That(UnixTime.ToDateTime(-27501531398.766).ToUniversalTime(), Is.EqualTo(new DateTime(1098, 7, 6, 5, 43, 21, 234)));
            Assert.That(UnixTime.ToDateTime(0, +123).ToUniversalTime(), Is.EqualTo(UnixTime.EpochUtc.AddMilliseconds(+123)));
            Assert.That(UnixTime.ToDateTime(0, -123).ToUniversalTime(), Is.EqualTo(UnixTime.EpochUtc.AddMilliseconds(-123)));
        }

        [Test]
        public void CannotSpecifyPositiveTimeWithNegativeMilliseconds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => UnixTime.ToDateTime(123, -456));
        }

        [Test]
        public void CannotSpecifyNegativeTimeWithPositiveMilliseconds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => UnixTime.ToDateTime(-123, 456));
        }

        [Test]
        public void Negative()
        {
            Assert.That(UnixTime.ToDateTime(-27501531398, -766).ToUniversalTime(), Is.EqualTo(new DateTime(1098, 7, 6, 5, 43, 21, 234)));
        }

        [Test]
        public void CannotOverflowMilliseconds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => UnixTime.ToDateTime(0, 1000));
        }

        [Test]
        public void CannotOverflowNegativeMilliseconds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => UnixTime.ToDateTime(0, -1000));
        }

        [Test]
        public void Maximum()
        {
            Assert.That(UnixTime.ToDateTime(32535215999L).ToUniversalTime(), Is.EqualTo(new DateTime(3000, 12, 31, 23, 59, 59)));
        }
    }
}




