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

namespace Jayrock.Services
{
    #region Imports

    using System;
    using System.Security;
    using System.Security.Permissions;
    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestMethod
    {
        [Test]
        public void FailedMethodYieldsInvocationException()
        {
            try
            {
                TestService service = new TestService();
                service.GetClass().GetMethodByName("BadMethod").Invoke(service, null, null);
                Assert.Fail("Expecting an exception.");
            }
            catch (TargetMethodException e)
            {
                Assert.That(e.InnerException.GetType(), Is.EqualTo(typeof(ApplicationException)), $"Unexpected inner exception ({e.InnerException.GetType().FullName}).");
            }
        }

        [Test]
        public void VariableArguments()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("VarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            object[] invokeArgs = method.TransposeVariableArguments(args);
            Assert.That(invokeArgs.Length, Is.EqualTo(1));
            Assert.That(invokeArgs[0], Is.EqualTo(args));
        }

        [Test]
        public void FixedAndVariableArguments()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            args = method.TransposeVariableArguments(args);
            Assert.That(args.Length, Is.EqualTo(3));
            Assert.That(args[0], Is.EqualTo(1));
            Assert.That(args[1], Is.EqualTo(2));
            Assert.That(args[2], Is.EqualTo(new object[] { 3, 4 }));
        }

        [Test]
        public void RetransposingYieldsTheSame()
        {
            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2, 3, 4 };
            args = method.TransposeVariableArguments(args);
            args = method.TransposeVariableArguments(args);
            Assert.That(args.Length, Is.EqualTo(3));
            Assert.That(args[0], Is.EqualTo(1));
            Assert.That(args[1], Is.EqualTo(2));
            Assert.That(args[2], Is.EqualTo(new object[] { 3, 4 }));
        }

        [Test]
        public void Bug8131()
        {
            //
            // Bug #8131: Varargs transposition when last arg is collection
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=8131&group_id=4638
            //

            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("VarMethod");
            object[] args = new object[] { 1, 2, new int[] { 3, 4 } };
            args = method.TransposeVariableArguments(args);
            Assert.That(args.Length, Is.EqualTo(1));
            object[] varargs = (object[])args[0];
            Assert.That(varargs.Length, Is.EqualTo(3));
            Assert.That(varargs[0], Is.EqualTo(1));
            Assert.That(varargs[1], Is.EqualTo(2));
            Assert.That(varargs[2], Is.EqualTo(new int[] { 3, 4 }));
        }

        [Test]
        public void Bug8148()
        {
            //
            // Bug #8148: Varargs transposition drops args
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=8148&group_id=4638
            //

            TestService service = new TestService();
            Method method = service.GetClass().FindMethodByName("FixedVarMethod");
            object[] args = new object[] { 1, 2,
                new int[] { 3, 4 },
                new JsonObject(new string[] { "five", "six" }, new object[] { 5, 6 }) };
            args = method.TransposeVariableArguments(args);
            Assert.That(args.Length, Is.EqualTo(3));
            Assert.That(args[0], Is.EqualTo(1));
            Assert.That(args[1], Is.EqualTo(2));
            object[] varargs = (object[])args[2];
            Assert.That(varargs.Length, Is.EqualTo(2));
            Assert.That(varargs[0], Is.EqualTo(new int[] { 3, 4 }));
            JsonObject o = (JsonObject)varargs[1];
            Assert.That(o["five"], Is.EqualTo(5));
            Assert.That(o["six"], Is.EqualTo(6));
        }

        [Test]
#if MONO
        [Ignore("See http://code.google.com/p/jayrock/issues/detail?id=23")]
#endif
        public void Issue12()
        {
            // Issue #12: PrincipalPermissionAttribute causes InvalidCastException when applied to service methods 
            // http://code.google.com/p/jayrock/issues/detail?id=12

            SecuredService service = new SecuredService();
            Method method = service.GetClass().GetMethodByName("SecuredMethod");
            try
            {
                method.Invoke(service, null, null);
            }
            catch (TargetMethodException e)
            {
                Assert.That(e.InnerException, Is.Not.Null);
                throw e.InnerException;
            }
        }

        private sealed class TestService : JsonRpcService
        {
            [JsonRpcMethod]
            public void BadMethod()
            {
                throw new ApplicationException();
            }

            [JsonRpcMethod]
            public void VarMethod(params object[] args)
            {
            }

            [JsonRpcMethod]
            public void FixedVarMethod(int a, int b, params object[] args)
            {
            }
        }

        private sealed class SecuredService : JsonRpcService
        {
            [JsonRpcMethod]
#if NETFRAMEWORK
            [PrincipalPermission(SecurityAction.Demand)]
#endif
            public void SecuredMethod() { }
        }
    }
}




