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

    using System.IO;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestBufferedCharReader
    {
        [Test]
        public void InitialCounters()
        {
            BufferedCharReader reader = new BufferedCharReader(TextReader.Null);
            AssertCounters(reader, 0, 0, 0);
        }

        [Test]
        public void CountersOnReadingFirstChar()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("a"));
            reader.Next();
            AssertCounters(reader, 1, 1, 1);
        }

        [Test]
        public void CountersOnReadingSecondChar()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("ab"));
            reader.Next();
            reader.Next();
            AssertCounters(reader, 2, 1, 2);
        }

        [Test]
        public void CountersUnaffectedWhenReadingPastEOF()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("abc"));
            Assert.That(reader.Next(), Is.EqualTo('a'));
            Assert.That(reader.Next(), Is.EqualTo('b'));
            Assert.That(reader.Next(), Is.EqualTo('c'));
            Assert.That(reader.More(), Is.False);
            Assert.That(reader.Next(), Is.EqualTo('\0'));
            Assert.That(reader.CharCount, Is.EqualTo(3), "CharCount");
            Assert.That(reader.LineNumber, Is.EqualTo(1), "LineNumber");
            Assert.That(reader.LinePosition, Is.EqualTo(3), "LinePosition");
        }

        [Test]
        public void LineNumberBumpsAndPositionResetsWithEachLF()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("\n1\n23\n456\n"));
            AssertReadLineNumPos(reader, '\n', 1, 1);
            AssertReadLineNumPos(reader, '1', 2, 1);
            AssertReadLineNumPos(reader, '\n', 2, 2);
            AssertReadLineNumPos(reader, '2', 3, 1);
            AssertReadLineNumPos(reader, '3', 3, 2);
            AssertReadLineNumPos(reader, '\n', 3, 3);
            AssertReadLineNumPos(reader, '4', 4, 1);
            AssertReadLineNumPos(reader, '5', 4, 2);
            AssertReadLineNumPos(reader, '6', 4, 3);
            AssertReadLineNumPos(reader, '\n', 4, 4);
            AssertReadLineNumPos(reader, '\0', 4, 4);
        }

        [Test]
        public void BackRewindsCounters()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("a"));
            reader.Next();
            reader.Back();
            AssertCounters(reader, 0, 0, 0);
        }

        [Test]
        public void NextAfterBackRestoresCounters()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("a"));
            reader.Next();
            reader.Back();
            reader.Next();
            AssertCounters(reader, 1, 1, 1);
        }

        [Test]
        public void CountersWhenBackNextAroundLine()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader("12\n34"));
            reader.Next(); AssertCounters(reader, 1, 1, 1);
            reader.Next(); AssertCounters(reader, 2, 1, 2);
            reader.Next(); AssertCounters(reader, 3, 1, 3);
            reader.Next(); AssertCounters(reader, 4, 2, 1);
            reader.Back(); AssertCounters(reader, 3, 1, 3);
            reader.Next(); AssertCounters(reader, 4, 2, 1);
        }

        [Test]
        public void BacktrackAfterEnding()
        {
            BufferedCharReader reader = new BufferedCharReader(new StringReader(";"));
            Assert.That(reader.More(), Is.True);
            Assert.That(reader.Next(), Is.EqualTo(';'));
            Assert.That(reader.More(), Is.False);
            reader.Back();
            Assert.That(reader.More(), Is.True);
            Assert.That(reader.Next(), Is.EqualTo(';'));
            Assert.That(reader.More(), Is.False);
        }

        private static void AssertReadLineNumPos(BufferedCharReader reader, char expected, int line, int pos)
        {
            Assert.That(reader.Next(), Is.EqualTo(expected), "Read");
            AssertLineCounters(reader, line, pos);
        }

        private static void AssertCounters(BufferedCharReader reader, int chars, int line, int pos)
        {
            Assert.That(reader.CharCount, Is.EqualTo(chars), "CharCount");
            AssertLineCounters(reader, line, pos);
        }

        private static void AssertLineCounters(BufferedCharReader reader, int line, int pos)
        {
            Assert.That(reader.LineNumber, Is.EqualTo(line), "LineNumber");
            Assert.That(reader.LinePosition, Is.EqualTo(pos), "LinePosition");
        }
    }
}
