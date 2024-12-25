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

namespace Jayrock.JsonML
{
    #region Imports

    using System;
    using System.IO;
    using System.Xml;
    using Json;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonMLCodec
    {
        [Test]
        public void EncodeDoesNotAcceptNullXmlReader()
        {
            Assert.Throws<ArgumentNullException>(() => JsonMLCodec.Encode(null, new JsonRecorder()));
        }

        [Test]
        public void EncodeDoesNotAcceptNullJsonWriter()
        {
            Assert.Throws<ArgumentNullException>(() => JsonMLCodec.Encode(new XmlTextReader("http://www.example.com/"), null));
        }

        [Test]
        public void EncodeArrayOnSingleEmptyElement()
        {
            JsonReader reader = EncodeArray("<root />");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayOnSingleEmptyElementWithAttributes()
        {
            JsonReader reader = EncodeArray("<root a1='v1' a2='v2' />");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("a1"));
            Assert.That(reader.ReadString(), Is.EqualTo("v1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a2"));
            Assert.That(reader.ReadString(), Is.EqualTo("v2"));
            reader.ReadToken(JsonTokenClass.EndObject);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayOnSingleElementWithAttributesAndContent()
        {
            JsonReader reader = EncodeArray("<root a1='v1' a2='v2'>content</root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("a1"));
            Assert.That(reader.ReadString(), Is.EqualTo("v1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a2"));
            Assert.That(reader.ReadString(), Is.EqualTo("v2"));
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.That(reader.ReadString(), Is.EqualTo("content"));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayOnNestedEmptyElements()
        {
            JsonReader reader = EncodeArray(@"
                <root>
                    <child1 />
                    <child2 />
                    <child3 />
                </root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("child1"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("child2"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("child3"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayOnInterspersedElementsAndText()
        {
            JsonReader reader = EncodeArray(@"<root>text1<e1 />text2<e2 />text3</root>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            Assert.That(reader.ReadString(), Is.EqualTo("text1"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("e1"));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.ReadString(), Is.EqualTo("text2"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("e2"));
            reader.ReadToken(JsonTokenClass.EndArray);
            Assert.That(reader.ReadString(), Is.EqualTo("text3"));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayCDataHandling()
        {
            JsonReader reader = EncodeArray("<e><![CDATA[<content>]]></e>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            Assert.That(reader.ReadString(), Is.EqualTo("<content>"));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayOnComplexXml()
        {
            JsonReader reader = EncodeArray(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e></e>" +
                "<d>re</d><f/><d>mi</d></a>");

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("a"));
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("ichi"));
            Assert.That(reader.ReadString(), Is.EqualTo("1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("ni"));
            Assert.That(reader.ReadString(), Is.EqualTo("2"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("b"));
            Assert.That(reader.ReadString(), Is.EqualTo("The content of b"));
            reader.ReadToken(JsonTokenClass.EndArray);

            Assert.That(reader.ReadString(), Is.EqualTo(" and "));

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("c"));
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("san"));
            Assert.That(reader.ReadString(), Is.EqualTo("3"));
            reader.ReadToken(JsonTokenClass.EndObject);
            Assert.That(reader.ReadString(), Is.EqualTo("The content of c"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadString(), Is.EqualTo("do"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadString(), Is.EqualTo("re"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("f"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadString(), Is.EqualTo("mi"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.EndArray); // a
        }

        [Test]
        public void EncodeArraySkipsComments()
        {
            JsonReader reader = EncodeArray("<e><!--comment--></e>");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [Test]
        public void EncodeArrayExpectsXmlReaderOnElement()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader("<e>text</e>"));
            reader.Read();
            reader.Read();
            Assert.Throws<ArgumentException>(() => JsonMLCodec.EncodeArrayForm(reader, new JsonRecorder()));
        }

        private static JsonReader EncodeArray(string xml)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonMLCodec.EncodeArrayForm(new XmlTextReader(new StringReader(xml)), writer);
            return writer.CreatePlayer();
        }

        [Test]
        public void EncodeObjectOnSingleEmptyElement()
        {
            JsonReader reader = EncodeObject("<root />");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectOnSingleEmptyElementWithAttributes()
        {
            JsonReader reader = EncodeObject("<root a1='v1' a2='v2' />");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a1"));
            Assert.That(reader.ReadString(), Is.EqualTo("v1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a2"));
            Assert.That(reader.ReadString(), Is.EqualTo("v2"));
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectOnSingleElementWithAttributesAndContent()
        {
            JsonReader reader = EncodeObject("<root a1='v1' a2='v2'>content</root>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a1"));
            Assert.That(reader.ReadString(), Is.EqualTo("v1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("a2"));
            Assert.That(reader.ReadString(), Is.EqualTo("v2"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("content"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectOnNestedEmptyElements()
        {
            JsonReader reader = EncodeObject(@"
                <root>
                    <child1 />
                    <child2 />
                    <child3 />
                </root>");

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("child1"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("child2"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("child3"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectOnInterspersedElementsAndText()
        {
            JsonReader reader = EncodeObject(@"<root>text1<e1 />text2<e2 />text3</root>");

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("root"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);

            Assert.That(reader.ReadString(), Is.EqualTo("text1"));

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("e1"));
            reader.ReadToken(JsonTokenClass.EndObject);

            Assert.That(reader.ReadString(), Is.EqualTo("text2"));

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("e2"));
            reader.ReadToken(JsonTokenClass.EndObject);

            Assert.That(reader.ReadString(), Is.EqualTo("text3"));

            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectCDataHandling()
        {
            JsonReader reader = EncodeObject("<e><![CDATA[<content>]]></e>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("<content>"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectOnComplexXml()
        {
            JsonReader reader = EncodeObject(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e></e>" +
                "<d>re</d><f/><d>mi</d></a>");

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("a"));
            Assert.That(reader.ReadMember(), Is.EqualTo("ichi"));
            Assert.That(reader.ReadString(), Is.EqualTo("1"));
            Assert.That(reader.ReadMember(), Is.EqualTo("ni"));
            Assert.That(reader.ReadString(), Is.EqualTo("2"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("b"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("The content of b"));
            reader.ReadToken(JsonTokenClass.EndArray);

            Assert.That(reader.ReadString(), Is.EqualTo(" and "));

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("c"));
            Assert.That(reader.ReadMember(), Is.EqualTo("san"));
            Assert.That(reader.ReadString(), Is.EqualTo("3"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("The content of c"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("do"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("re"));
            reader.ReadToken(JsonTokenClass.EndArray);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("f"));
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("d"));
            Assert.That(reader.ReadMember(), Is.EqualTo("childNodes"));
            reader.ReadToken(JsonTokenClass.Array);
            Assert.That(reader.ReadString(), Is.EqualTo("mi"));
            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject);

            reader.ReadToken(JsonTokenClass.EndArray);
            reader.ReadToken(JsonTokenClass.EndObject); // a
        }

        [Test]
        public void EncodeObjectSkipsComments()
        {
            JsonReader reader = EncodeObject("<e><!--comment--></e>");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.That(reader.ReadMember(), Is.EqualTo("tagName"));
            Assert.That(reader.ReadString(), Is.EqualTo("e"));
            reader.ReadToken(JsonTokenClass.EndObject);
        }

        [Test]
        public void EncodeObjectExpectsXmlReaderOnElement()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader("<e>text</e>"));
            reader.Read();
            reader.Read();
            Assert.Throws<ArgumentException>(() => JsonMLCodec.EncodeObjectForm(reader, new JsonRecorder()));
        }

        private static JsonReader EncodeObject(string xml)
        {
            JsonRecorder writer = new JsonRecorder();
            JsonMLCodec.EncodeObjectForm(new XmlTextReader(new StringReader(xml)), writer);
            return writer.CreatePlayer();
        }

        [Test]
        public void DecodeDoesNotAcceptNullJsonReader()
        {
            Assert.Throws<ArgumentNullException>(() => JsonMLCodec.Decode(null, new XmlTextWriter(new StringWriter())));
        }

        [Test]
        public void DecodeDoesNotAcceptNullXmlWriter()
        {
            Assert.Throws<ArgumentNullException>(() => JsonMLCodec.Decode(new JsonRecorder().CreatePlayer(), null));
        }

        [Test]
        public void DecodeArrayOnSingleElement()
        {
            Assert.That(DecodeArray("[root]"), Is.EqualTo("<root />"));
        }

        [Test]
        public void DecodeArrayExpectsJsonArray()
        {
            Assert.Throws<JsonException>(() => DecodeArray("{}"));
        }

        [Test]
        public void DecodeArrayOnSingleElementWithAttributes()
        {
            Assert.That(DecodeArray("[root,{a:1,b:2}]"), Is.EqualTo("<root a='1' b='2' />"));
        }

        [Test]
        public void DecodeArrayThrowsWhenAttributeIsObject()
        {
            Assert.Throws<JsonMLException>(() => DecodeArray("[root,{bad:{}}]"));
        }

        [Test]
        public void DecodeArrayThrowsWhenAttributeIsArray()
        {
            Assert.Throws<JsonMLException>(() => DecodeArray("[root,{bad:[]}]"));
        }

        [Test]
        public void DecodeArrayOnAttributeWithNullYieldsEmptyXmlAttributeValue()
        {
            Assert.That(DecodeArray("[root,{nullattr:null}]"), Is.EqualTo("<root nullattr='' />"));
        }

        [Test]
        public void DecodeArrayOnSingleElementWithAttributesAndContent()
        {
            Assert.That(DecodeArray("[root,{a:1,b:2},content]"), Is.EqualTo("<root a='1' b='2'>content</root>"));
        }

        [Test]
        public void DecodeArrayOnNestedEmptyElements()
        {
            Assert.That(DecodeArray("[root,[child1],[child2],[child3]]"), Is.EqualTo("<root><child1 /><child2 /><child3 /></root>"));
        }

        [Test]
        public void DecodeArrayOnInterspersedElementsAndText()
        {
            Assert.That(DecodeArray("[root,text1,[e1],text2,[e2],text3]"), Is.EqualTo("<root>text1<e1 />text2<e2 />text3</root>"));
        }

        [Test]
        public void DecodeArrayOnComplexJson()
        {
            Assert.That(DecodeArray(@"
            [a, {ichi: 1, ni: 2},
                [b, The content of b],
                ' and ',
                [c, {san: 3}, The content of c],
                [d, do],
                [e],
                [d, re],
                [f],
                [d, mi]
            ]"), Is.EqualTo(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e />" +
                "<d>re</d><f /><d>mi</d></a>"));
        }

        [Test]
        public void DecodeArrayOnElementWithEmptyAttributesObject()
        {
            Assert.That(DecodeArray("[e,{},text]"), Is.EqualTo("<e>text</e>"));
        }

        [Test]
        public void DecodeArrayOnElementPermitsChildNodes()
        {
            Assert.That(DecodeArray("[e,abc,[],def]"), Is.EqualTo("<e>abcdef</e>"));
            Assert.That(DecodeArray("[e,[]]"), Is.EqualTo("<e />"));
            Assert.That(DecodeArray("[e,{a:1},[]]"), Is.EqualTo("<e a='1' />"));
        }

        [Test]
        public void DecodeArrayDoesNotLikeEmptyRootElement()
        {
            Assert.Throws<JsonMLException>(() => DecodeArray("[]"));
        }

        [Test]
        public void DecodeArrayAllowsEmptyRootElementAsLongAsXmlWriterIsStarted()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.WriteStartElement("root");
            JsonMLCodec.Decode(new JsonTextReader(new StringReader("[]")), writer);
        }

        [Test]
        public void DecodeArrayDoesNotLikeObjectsAfterAttributes()
        {
            Assert.Throws<JsonMLException>(() => DecodeArray("[e,{a:1},text,{/*bad*/}]"));
        }

        [Test]
        public void DecodeArrayIgnoresNullElements()
        {
            Assert.That(DecodeArray("[e,null,text,null]"), Is.EqualTo("<e>text</e>"));
        }

        private static string DecodeArray(string json)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.QuoteChar = '\'';
            JsonMLCodec.DecodeArrayForm(new JsonTextReader(new StringReader(json)), writer);
            return sw.ToString();
        }

        [Test]
        public void DecodeObjectOnSingleElement()
        {
            Assert.That(DecodeObject("{tagName:root}"), Is.EqualTo("<root />"));
        }

        [Test]
        public void DecodeObjectExpectsJsonArray()
        {
            Assert.Throws<JsonException>(() => DecodeObject("[]"));
        }

        [Test]
        public void DecodeObjectOnSingleElementWithAttributes()
        {
            Assert.That(DecodeObject("{tagName:root,a:1,b:2}"), Is.EqualTo("<root a='1' b='2' />"));
        }

        [Test]
        public void DecodeObjectAcceptsTagNameAsNonFirstMember()
        {
            Assert.That(DecodeObject("{a:1,b:2,tagName:root}"), Is.EqualTo("<root a='1' b='2' />"));
        }

        [Test]
        public void DecodeObjectThrowsWhenAttributeIsObject()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,bad:{}}"));
        }

        [Test]
        public void DecodeObjectThrowsWhenAttributeIsArray()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,bad:[]}"));
        }

        [Test]
        public void DecodeObjectOnAttributeWithNullSkipsXmlAttribute()
        {
            Assert.That(DecodeObject("{tagName:root,nullattr:null}"), Is.EqualTo("<root />"));
        }

        [Test]
        public void DecodeObjectToleratesDuplicateTagNames()
        {
            Assert.That(DecodeObject("{tagName:root,tagName:root}"), Is.EqualTo("<root />"));
        }

        [Test]
        public void DecodeObjectThrowsWhenTagNameIsRedefined()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,tagName:ROOT}"));
        }

        [Test]
        public void DecodeObjectOnSingleElementWithAttributesAndContent()
        {
            Assert.That(DecodeObject("{tagName:root,a:1,b:2,childNodes:[content]}"), Is.EqualTo("<root a='1' b='2'>content</root>"));
        }

        [Test]
        public void DecodeObjectThrowsWhenChildNodesIsObject()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,childNodes:{}}"));
        }

        [Test]
        public void DecodeObjectThrowsWhenChildNodesIsString()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,childNodes:foo}"));
        }

        [Test]
        public void DecodeObjectThrowsWhenChildNodesIsBoolean()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,childNodes:true}"));
        }

        [Test]
        public void DecodeObjectThrowsWhenChildNodesIsNumber()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:root,childNodes:123}"));
        }

        [Test]
        public void DecodeObjectToleratesNullChildNodes()
        {
            Assert.That(DecodeObject("{tagName:root,childNodes:null}"), Is.EqualTo("<root />"));
        }

        [Test]
        public void DecodeObjectOnNestedEmptyElements()
        {
            Assert.That(DecodeObject(@"{
            tagName: root,
            childNodes: [
                {tagName:child1},
                {tagName:child2},
                {tagName:child3}
            ]
        }"), Is.EqualTo("<root><child1 /><child2 /><child3 /></root>"));
        }

        [Test]
        public void DecodeObjectOnInterspersedElementsAndText()
        {
            Assert.That(DecodeObject(@"{
            tagName: root,
            childNodes: [
                text1,
                {tagName:e1},
                text2,
                {tagName:e2},
                text3
            ]
        }"), Is.EqualTo("<root>text1<e1 />text2<e2 />text3</root>"));
        }

        [Test]
        public void DecodeObjectSkipsNullsInsideChildNodes()
        {
            Assert.That(DecodeObject(@"{
            tagName: root,
            childNodes: [
                text1,
                {tagName:e1},
                null,
                {tagName:e2},
                null
            ]
        }"), Is.EqualTo("<root>text1<e1 /><e2 /></root>"));
        }

        [Test]
        public void DecodeObjectHandlesNonStringChildNodes()
        {
            Assert.That(DecodeObject(@"{
            tagName:root,
            childNodes:[
                string,
                true,
                false,
                123.456
            ]
        }"), Is.EqualTo("<root>stringtruefalse123.456</root>"));
        }

        [Test]
        public void DecodeObjectAccumulatesDuplicateChildNodes()
        {
            Assert.That(DecodeObject(@"{
        tagName:root,
        childNodes:[123.456,foo,null,{tagName:child1},true,false],
        a1: 1,
        childNodes:[false,true,{tagName:child2},null,bar,456.789],
        a2: 2
    }"), Is.EqualTo("<root a1='1' a2='2'>123.456foo<child1 />truefalsefalsetrue<child2 />bar456.789</root>"));
        }

        [Test]
        public void DecodeObjectOnComplexJson()
        {
            Assert.That(DecodeObject(@"{
            tagName: a,
            ichi: 1,
            ni: 2,
            childNodes: [ {
                    tagName: b,
                    childNodes: [ 'The content of b' ] 
                },
                ' and ', {
                    tagName: c,
                    san: 3,
                    childNodes: [ 'The content of c' ] 
                },
                { tagName: d, childNodes: [ do ] },
                { tagName: e },
                { tagName: d, childNodes: [ re ] },
                { tagName: f },
                { tagName: d, childNodes: [ mi ] }
            ]
        }"), Is.EqualTo(
                "<a ichi='1' ni='2'><b>The content of b</b> and " +
                "<c san='3'>The content of c</c><d>do</d><e />" +
                "<d>re</d><f /><d>mi</d></a>"));
        }

        [Test]
        public void DecodeObjectThrowsWhenMissingTagName()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{}"));
        }

        [Test]
        public void DecodeObjectThrowsWhenTagNameIsEmpty()
        {
            Assert.Throws<JsonMLException>(() => DecodeObject("{tagName:''}"));
        }

        [Test]
        public void DecodeObjectAllowsEmptyRootElementAsLongAsXmlWriterIsStarted()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.WriteStartElement("root");
            JsonMLCodec.Decode(new JsonTextReader(new StringReader("{}")), writer);
        }

        private static string DecodeObject(string json)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.QuoteChar = '\'';
            JsonMLCodec.DecodeObjectForm(new JsonTextReader(new StringReader(json)), writer);
            return sw.ToString();
        }
    }
}