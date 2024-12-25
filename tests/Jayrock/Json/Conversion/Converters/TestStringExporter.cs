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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestStringExporter
    {
        [Test]
        public void Superclass()
        {
            Assert.That(new StringExporter(), Is.InstanceOf<ExporterBase>());
        }

        [Test]
        public void InputTypeIsString()
        {
            Assert.That((new StringExporter()).InputType, Is.SameAs(typeof(string)));
        }

        [Test]
        public void ExportEmpty()
        {
            Assert.That(Export(string.Empty).ReadString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void ExportString()
        {
            Assert.That(Export("Lorem ipsum dolor sit amet, consectetuer adipiscing elit.").ReadString(), Is.EqualTo("Lorem ipsum dolor sit amet, consectetuer adipiscing elit."));
        }

        private static JsonReader Export(string value)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonConvert.Export(value, writer);
            return writer.CreatePlayer();
        }
    }
}
