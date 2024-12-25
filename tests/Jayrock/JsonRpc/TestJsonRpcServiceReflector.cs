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

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Collections;
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestJsonRpcServiceReflector
    {
        [Test]
        public void NullType()
        {
            Assert.Throws<ArgumentNullException>(() => JsonRpcServices.GetClassFromType(null));
        }

        [Test]
        public void ServiceNameIsTypeName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.That(clazz.Name, Is.EqualTo("EmptyService"));
        }

        [Test]
        public void UntaggedMethodsNotExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.That(clazz.GetMethods().Length, Is.EqualTo(0));
        }

        [Test]
        public void TaggedMethodsExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.GetMethods().Length, Is.EqualTo(4));
        }

        [Test]
        public void CustomServiceName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.Name, Is.EqualTo("MyService"));
        }

        [Test]
        public void NullService()
        {
            Assert.Throws<ArgumentNullException>(() => JsonRpcServices.GetClassFromType(null));
        }

        [Test]
        public void DefaultNameIsMethodName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.FindMethodByName("Foo").Name, Is.EqualTo("Foo"));
        }

        [Test]
        public void AffliatedWithService()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            foreach (Method method in clazz.GetMethods())
                Assert.That(method.ServiceClass, Is.SameAs(clazz));
        }

        [Test]
        public void CustomNameViaAttribute()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Foo");
            Assert.That(method.Name, Is.EqualTo("Foo"));
            Assert.That(method.InternalName, Is.EqualTo("Foo"));
        }

        [Test]
        public void AttributeFromMethod()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Baz");
            Assert.That(method.Name, Is.EqualTo("Baz"));
            Assert.That(method.InternalName, Is.EqualTo("Bar"));
        }

        [Test]
        public void ResultTypeIsMethodReturnType()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.GetMethodByName("Foo").ResultType, Is.EqualTo(typeof(void)));
            Assert.That(clazz.GetMethodByName("Baz").ResultType, Is.EqualTo(typeof(void)));
            Assert.That(clazz.GetMethodByName("Sum").ResultType, Is.EqualTo(typeof(int)));
            Assert.That(clazz.GetMethodByName("Format").ResultType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Parameters()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Parameter[] parameters = clazz.GetMethodByName("Sum").GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2));

            Parameter parameter;

            parameter = parameters[0];
            Assert.That(parameter.Name, Is.EqualTo("a"));
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(int)));
            Assert.That(parameter.Position, Is.EqualTo(0));

            parameter = parameters[1];
            Assert.That(parameters[1].Name, Is.EqualTo("b"));
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(int)));
            Assert.That(parameter.Position, Is.EqualTo(1));
        }

        [Test]
        public void ParamArray()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));

            Assert.That(clazz.GetMethodByName("Foo").HasParamArray, Is.False);
            Assert.That(clazz.GetMethodByName("Baz").HasParamArray, Is.False);

            Method method;
            Parameter[] parameters;

            method = clazz.GetMethodByName("Sum");
            parameters = method.GetParameters();
            Assert.That(method.HasParamArray, Is.False);
            Assert.That(parameters[0].IsParamArray, Is.False);
            Assert.That(parameters[1].IsParamArray, Is.False);

            method = clazz.GetMethodByName("Format");
            parameters = method.GetParameters();
            Assert.That(method.HasParamArray, Is.True);
            Assert.That(parameters[0].IsParamArray, Is.False);
            Assert.That(parameters[1].IsParamArray, Is.True);
        }

        [Test]
        public void Invocation()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            TestService service = new TestService();
            object result = clazz.GetMethodByName("Sum").Invoke(service, null, new object[] { 2, 3 });
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void MethodDescriptions()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.That(clazz.GetMethodByName("Foo").Description.Length, Is.EqualTo(0));
            Assert.That(clazz.GetMethodByName("Baz").Description.Length, Is.EqualTo(0));
            Assert.That(clazz.GetMethodByName("Sum").Description.Length, Is.EqualTo(0));
            Assert.That(clazz.GetMethodByName("Format").Description, Is.EqualTo("Formats a string."));
        }

        [Test]
        public void ServiceDescription()
        {
            Assert.That(JsonRpcServices.GetClassFromType(typeof(TestService)).Description, Is.EqualTo("A test service."));
        }

        [Test]
        public void CustomAttributes()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            MyAttribute[] attributes = (MyAttribute[])JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo").GetCustomAttributes(typeof(MyAttribute));
            Assert.That(attributes.Length, Is.EqualTo(3));
            foreach (MyAttribute attribute in attributes)
                expectedValues.Remove(attribute.TestValue);
            Assert.That(expectedValues.Count, Is.EqualTo(0));
        }

        [Test]
        public void CustomAttributesAreCopied()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.That(method.GetCustomAttributes()[0], Is.Not.SameAs(method.GetCustomAttributes()[0]));
        }

        [Test]
        public void FindFirstCustomAttribute()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            MyAttribute attribute = (MyAttribute)method.FindFirstCustomAttribute(typeof(MyAttribute));
            expectedValues.Remove(attribute.TestValue);
            Assert.That(expectedValues.Count, Is.EqualTo(2));
        }

        [Test]
        public void FindFirstCustomAttributeYieldsCopy()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.That(method.FindFirstCustomAttribute(typeof(MyAttribute)), Is.Not.SameAs(method.FindFirstCustomAttribute(typeof(MyAttribute))));
        }

        [Test]
        public void MethodIdempotency()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(IdempotencyTestService));
            Assert.That(clazz.GetMethodByName("NonIdempotentMethod").Idempotent, Is.False);
            Assert.That(clazz.GetMethodByName("IdempotentMethod").Idempotent, Is.True);
        }

        [Test]
        public void MethodLookupIsCaseFree()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method foo = clazz.FindMethodByName("Foo");
            Assert.That(foo.Name, Is.EqualTo("Foo"));
            Assert.That(clazz.FindMethodByName("foo"), Is.SameAs(foo));
            Assert.That(clazz.FindMethodByName("FOO"), Is.SameAs(foo));
            Assert.That(clazz.FindMethodByName("Foo"), Is.SameAs(foo));
        }

        [Test]
        public void MethodWithWarpedParametersAndResult()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(ServiceWithMethodsUsingWarpedParameters));
            Method foo = clazz.FindMethodByName("Foo");

            Assert.That(foo.ResultType, Is.EqualTo(typeof(DateTime)));

            Parameter[] parameters = foo.GetParameters();
            Assert.That(parameters.Length, Is.EqualTo(2));

            Parameter parameter;

            parameter = parameters[0];
            Assert.That(parameter.Name, Is.EqualTo("stringArg"));
            Assert.That(parameter.Position, Is.EqualTo(0));
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(string)));

            parameter = parameters[1];
            Assert.That(parameter.Name, Is.EqualTo("intArg"));
            Assert.That(parameter.Position, Is.EqualTo(1));
            Assert.That(parameter.ParameterType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void MethodWithWarpedParametersButVoidResult()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(ServiceWithMethodsUsingWarpedParameters));
            Method foo = clazz.FindMethodByName("FooNoResult");

            Assert.That(foo.ResultType, Is.EqualTo(typeof(void)));
        }

        private sealed class EmptyService
        {
        }

        [JsonRpcService("MyService")]
        [JsonRpcHelp("A test service.")]
        private sealed class TestService : IService
        {
            [JsonRpcMethod]
            [MyAttribute(12), MyAttribute(56), MyAttribute(34)]
            public void Foo() { throw new NotImplementedException(); }

            [JsonRpcMethod("Baz")]
            public void Bar() { throw new NotImplementedException(); }

            [JsonRpcMethod]
            public int Sum(int a, int b)
            {
                return a + b;
            }

            [JsonRpcMethod, JsonRpcHelp("Formats a string.")]
            public string Format(string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class IdempotencyTestService
        {
            [JsonRpcMethod]
            public void NonIdempotentMethod() { }

            [JsonRpcMethod(Idempotent = true)]
            public void IdempotentMethod() { }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        private class MyAttribute : Attribute
        {
            private readonly int _testValue;

            public MyAttribute(int testValue)
            {
                _testValue = testValue;
            }

            public int TestValue
            {
                get { return _testValue; }
            }
        }

        private sealed class ServiceWithMethodsUsingWarpedParameters : IService
        {
            public FooArgs Args;
            public FooResult Result = null;

            [JsonRpcMethod(WarpedParameters = true)]
            public FooResult Foo(FooArgs args)
            {
                this.Args = args;
                return Result;
            }

            [JsonRpcMethod(WarpedParameters = true)]
            public void FooNoResult(FooArgs args)
            {
                this.Args = args;
            }

            public sealed class FooArgs
            {
                //
                // NOTE: The default assignments on the following fields is
                // to maily shut off the following warning from the compiler:
                //
                // warning CS0649: Field '...' is never assigned to, and will always have its default value
                //
                // This warning is harmless. Since the C# 1.x compiler does
                // not support #pragma warning disable, we have to resort
                // to a more brute force method.
                //

                public string stringArg = null;
                public int intArg = 0;
            }

            public sealed class FooResult
            {
                public DateTime Result;

                public FooResult(DateTime result)
                {
                    this.Result = result;
                }
            }

            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }
    }
}


