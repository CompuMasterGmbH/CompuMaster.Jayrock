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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonConvert
    {
        private bool _createImportContextCalled;
        private bool _createExportContextCalled;

        [SetUp]
        public void Init()
        {
            _createImportContextCalled = false;
            _createExportContextCalled = false;
        }

        [Test]
        public void DefaultExportContextFactory()
        {
            Assert.That(JsonConvert.DefaultExportContextFactory, Is.Not.Null);
        }

        [Test]
        public void DefaultImportContextFactory()
        {
            Assert.That(JsonConvert.DefaultImportContextFactory, Is.Not.Null);
        }

        [Test]
        public void SetCurrentExportContextFactory()
        {
            try
            {
                ExportContextFactoryHandler factory = new ExportContextFactoryHandler(CreateExportContext);
                JsonConvert.CurrentExportContextFactory = factory;
                Assert.That(JsonConvert.CurrentExportContextFactory, Is.SameAs(factory));
                JsonConvert.CreateExportContext();
                Assert.That(_createExportContextCalled, Is.True);
            }
            finally
            {
                JsonConvert.CurrentExportContextFactory = JsonConvert.DefaultExportContextFactory;
            }
        }

        [Test]
        public void CannotSetCurrentExportContextFactoryToNull()
        {
            Assert.Throws<ArgumentNullException>(() => JsonConvert.CurrentExportContextFactory = null);
        }

        [Test]
        public void SetCurrentImportContextFactory()
        {
            try
            {
                ImportContextFactoryHandler factory = new ImportContextFactoryHandler(CreateImportContext);
                JsonConvert.CurrentImportContextFactory = factory;
                Assert.That(JsonConvert.CurrentImportContextFactory, Is.SameAs(factory));
                JsonConvert.CreateImportContext();
                Assert.That(_createImportContextCalled, Is.True);
            }
            finally
            {
                JsonConvert.CurrentImportContextFactory = JsonConvert.DefaultImportContextFactory;
            }
        }

        [Test]
        public void CannotSetCurrentImportContextFactoryToNull()
        {
            Assert.Throws<ArgumentNullException>(() => JsonConvert.CurrentImportContextFactory = null);
        }

        private ExportContext CreateExportContext()
        {
            _createExportContextCalled = true;
            return null;
        }

        private ImportContext CreateImportContext()
        {
            _createImportContextCalled = true;
            return null;
        }
    }
}



