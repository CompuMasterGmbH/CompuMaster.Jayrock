namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.IO;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestUriImporter
    {
        [Test]
        public void ImportString()
        {
            AssertImport(new Uri("http://jayrock.berlios.de/"), "\"http://jayrock.berlios.de/\"");
        }

        [Test]
        public void ImportNull()
        {
            AssertImport(null, "null");
        }

        [Test]
        public void CannotImportNumber()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "123"));
        }

        [Test]
        public void CannotImportTrue()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "true"));
        }

        [Test]
        public void CannotImportFalse()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "false"));
        }

        [Test]
        public void CannotImportArray()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "[]"));
        }

        [Test]
        public void CannotImportObject()
        {
            Assert.Throws<JsonException>(() => AssertImport(null, "{}"));
        }

        private static void AssertImport(Uri expected, string input)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(input));
            UriImporter importer = new UriImporter();
            object o = importer.Import(new ImportContext(), reader);
            Assert.That(reader.EOF, Is.True, "Reader must be at EOF.");
            if (expected != null)
                Assert.That(o, Is.InstanceOf<Uri>());
            Assert.That(o, Is.EqualTo(expected));
        }
    }
}


