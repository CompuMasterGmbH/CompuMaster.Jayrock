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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using System;
    using System.ComponentModel;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonMemberNamingConventionAttribute
    {
        [Test]
        public void IsSerializable()
        {
            Assert.That(typeof(JsonMemberNamingConventionAttribute).IsSerializable, Is.True);
        }

        [Test]
        public void DefaultInitializationYieldsNoneConvention()
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute();
            Assert.That(attribute.Convention, Is.EqualTo(NamingConvention.None));
        }

        [Test]
        public void InitializeConvention()
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute(NamingConvention.Pascal);
            Assert.That(attribute.Convention, Is.EqualTo(NamingConvention.Pascal));
        }

        [Test]
        public void InitializeUnderscores()
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute(NamingConvention.Pascal, UnderscoreConvention.Separate);
            Assert.That(attribute.Underscores, Is.EqualTo(UnderscoreConvention.Separate));
        }

        [Test]
        public void SetConvention()
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute();
            Assert.That(attribute.Convention, Is.EqualTo(NamingConvention.None));
            attribute.Convention = NamingConvention.Pascal;
            Assert.That(attribute.Convention, Is.EqualTo(NamingConvention.Pascal));
        }

        [Test]
        public void SetUnderscores()
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute();
            Assert.That(attribute.Underscores, Is.EqualTo(UnderscoreConvention.None));
            attribute.Underscores = UnderscoreConvention.Separate;
            Assert.That(attribute.Underscores, Is.EqualTo(UnderscoreConvention.Separate));
        }

        [Test]
        public void CustomizationSkippedWhenNoneConvention()
        {
            TestPropertyDescriptor property = CreateTestProperty("foo");
            IPropertyDescriptorCustomization customization = new JsonMemberNamingConventionAttribute();
            customization.Apply(property);
            Assert.That(property.CustomizedName, Is.Null);
        }

        [Test]
        public void CannotApplyToNullPropertyDescriptor()
        {
            IPropertyDescriptorCustomization customization = new JsonMemberNamingConventionAttribute();
            Assert.Throws<ArgumentNullException>(() => customization.Apply(null));
        }

        [Test]
        public void PascalCaseApplication()
        {
            TestNamingCase("pascalCase", NamingConvention.Pascal, UnderscoreConvention.None, "PascalCase");
        }

        [Test]
        public void SingleLetterPascalCaseApplication()
        {
            TestNamingCase("p", NamingConvention.Pascal, UnderscoreConvention.None, "P");
        }

        [Test]
        public void CamelCaseApplication()
        {
            TestNamingCase("CamelCase", NamingConvention.Camel, UnderscoreConvention.None, "camelCase");
            TestNamingCase("C", NamingConvention.Camel, UnderscoreConvention.None, "c");
        }

        [Test]
        public void SingleLetterCamelCaseApplication()
        {
            TestNamingCase("C", NamingConvention.Camel, UnderscoreConvention.None, "c");
        }

        [Test]
        public void UpperCaseApplication()
        {
            TestNamingCase("upper", NamingConvention.Upper, UnderscoreConvention.None, "UPPER");
        }

        [Test]
        public void SingleLetterUpperCaseApplication()
        {
            TestNamingCase("u", NamingConvention.Upper, UnderscoreConvention.None, "U");
        }

        [Test]
        public void LowerCaseApplication()
        {
            TestNamingCase("LOWER", NamingConvention.Lower, UnderscoreConvention.None, "lower");
        }

        [Test]
        public void SingleLetterLowerCaseApplication()
        {
            TestNamingCase("LOWER", NamingConvention.Lower, UnderscoreConvention.None, "lower");
            TestNamingCase("L", NamingConvention.Lower, UnderscoreConvention.None, "l");
        }

        [Test]
        public void UnderscorePrefixApplication()
        {
            TestNamingCase("FooBarBaz", NamingConvention.Camel, UnderscoreConvention.Prefix, "_FooBarBaz");
            TestNamingCase("FooBarBaz", NamingConvention.Pascal, UnderscoreConvention.Prefix, "_FooBarBaz");
            TestNamingCase("FooBarBaz", NamingConvention.Upper, UnderscoreConvention.Prefix, "_FOOBARBAZ");
            TestNamingCase("FooBarBaz", NamingConvention.Lower, UnderscoreConvention.Prefix, "_foobarbaz");
        }

        [Test]
        public void UnderscoreSeparatorApplication()
        {
            TestNamingCase("FooBarBaz", NamingConvention.Camel, UnderscoreConvention.Separate, "foo_Bar_Baz");
            TestNamingCase("FooBarBaz", NamingConvention.Pascal, UnderscoreConvention.Separate, "Foo_Bar_Baz");
            TestNamingCase("FooBarBaz", NamingConvention.Upper, UnderscoreConvention.Separate, "FOO_BAR_BAZ");
            TestNamingCase("FooBarBaz", NamingConvention.Lower, UnderscoreConvention.Separate, "foo_bar_baz");
            TestNamingCase("foobarbaz", NamingConvention.None, UnderscoreConvention.Separate, "foobarbaz");
        }

        private static void TestNamingCase(string baseName, NamingConvention testCase, UnderscoreConvention testUnder, string expected)
        {
            JsonMemberNamingConventionAttribute attribute = new JsonMemberNamingConventionAttribute(testCase, testUnder);
            TestPropertyDescriptor property = CreateTestProperty(baseName);
            IPropertyDescriptorCustomization customization = attribute;

            customization.Apply(property);

            Assert.That(property.CustomizedName, Is.EqualTo(expected));
        }

        private static TestPropertyDescriptor CreateTestProperty(string baseName)
        {
            TestPropertyDescriptor property = new TestPropertyDescriptor(baseName);
            Assert.That(property.Name, Is.EqualTo(baseName));
            Assert.That(property.CustomizedName, Is.Null);
            return property;
        }

        private sealed class TestPropertyDescriptor : PropertyDescriptor, IPropertyCustomization
        {
            public string CustomizedName;

            public TestPropertyDescriptor(string name)
                : base(name, null) { }

            public void SetName(string name)
            {
                CustomizedName = name;
            }

            public void SetType(Type type) { throw new NotImplementedException(); }
            public IPropertyImpl OverrideImpl(IPropertyImpl impl) { throw new NotImplementedException(); }

            #region Unimplemented members of PropertyDescriptor

            public override bool CanResetValue(object component) { throw new NotImplementedException(); }
            public override object GetValue(object component) { throw new NotImplementedException(); }
            public override void ResetValue(object component) { throw new NotImplementedException(); }
            public override void SetValue(object component, object value) { throw new NotImplementedException(); }
            public override bool ShouldSerializeValue(object component) { throw new NotImplementedException(); }
            public override Type ComponentType { get { throw new NotImplementedException(); } }
            public override bool IsReadOnly { get { throw new NotImplementedException(); } }
            public override Type PropertyType { get { throw new NotImplementedException(); } }

            #endregion
        }
    }
}



