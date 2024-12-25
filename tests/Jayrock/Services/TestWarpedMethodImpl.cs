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
    using System.ComponentModel;
    using System.Threading;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class TestWarpedMethodImpl
    {
        [Test]
        public void Invocation()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();

            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)),
                TypeDescriptor.GetProperties(typeof(ResultThing))[0]);

            DummyService service = new DummyService();
            object[] args = new object[] { 42, "foobar" };
            baseMethod.InvokeResult = new ResultThing(123);
            object result = warpedMethod.Invoke(service, args);

            Assert.That(baseMethod.InvokeService, Is.SameAs(service));
            Assert.That(baseMethod.InvokeArgs, Is.Not.Null);
            Assert.That(baseMethod.InvokeArgs.Length, Is.EqualTo(1));
            Assert.That(baseMethod.InvokeArgs[0], Is.Not.Null);
            Thing thing = baseMethod.InvokeArgs[0] as Thing;
            Assert.That(thing, Is.InstanceOf<Thing>());
            Assert.That(thing.Int, Is.EqualTo(args[0]));
            Assert.That(thing.Str, Is.EqualTo(args[1]));
            Assert.That(result, Is.EqualTo(baseMethod.InvokeResult.Result));
        }

        [Test]
        public void InvocationWithNoResult()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();
            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)), null);
            Assert.That(warpedMethod.Invoke(new DummyService(), new object[0]), Is.Null);
        }

        [Test]
        public void AsyncInvocation()
        {
            TestMethodImpl baseMethod = new TestMethodImpl();

            WarpedMethodImpl warpedMethod = new WarpedMethodImpl(baseMethod, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)),
                TypeDescriptor.GetProperties(typeof(ResultThing))[0]);

            DummyService service = new DummyService();
            object[] args = new object[] { 42, "foobar" };
            baseMethod.InvokeResult = new ResultThing(123);
            IAsyncResult ar = warpedMethod.BeginInvoke(service, args, null, null);
            object result = warpedMethod.EndInvoke(service, ar);

            Assert.That(baseMethod.EndInvokeAsyncResult, Is.SameAs(ar));
            Assert.That(baseMethod.InvokeService, Is.SameAs(service));
            Assert.That(baseMethod.InvokeArgs, Is.Not.Null);
            Assert.That(baseMethod.InvokeArgs.Length, Is.EqualTo(1));
            Assert.That(baseMethod.InvokeArgs[0], Is.Not.Null);
            Thing thing = baseMethod.InvokeArgs[0] as Thing;
            Assert.That(thing, Is.InstanceOf<Thing>());
            Assert.That(thing.Int, Is.EqualTo(args[0]));
            Assert.That(thing.Str, Is.EqualTo(args[1]));
            Assert.That(result, Is.EqualTo(baseMethod.InvokeResult.Result));
        }

        [Test]
        public void CannotInitializeWithNullBaseImpl()
        {
            Assert.Throws<ArgumentNullException>(() => new WarpedMethodImpl(null, typeof(Thing),
                TypeDescriptor.GetProperties(typeof(Thing)), null));
        }

        [Test]
        public void CannotInitializeWithNullArgsType()
        {
            Assert.Throws<ArgumentNullException>(() => new WarpedMethodImpl(new TestMethodImpl(), null,
                TypeDescriptor.GetProperties(typeof(Thing)), null));
        }

        [Test]
        public void CannotInitializeWithNullArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new WarpedMethodImpl(new TestMethodImpl(), typeof(Thing), null, null));
        }

        [Test]
        public void AsynchronousQueryIsDelegated()
        {
            TestMethodImpl impl = new TestMethodImpl();
            Assert.That(impl.IsAsynchronousCalled, Is.False);
            Assert.That(new WarpedMethodImpl(impl, typeof(Thing), new PropertyDescriptorCollection(null), null).IsAsynchronous, Is.False);
            Assert.That(impl.IsAsynchronousCalled, Is.True);
        }

        private sealed class Thing
        {
            private int _int;
            private string _str;

            public int Int
            {
                get { return _int; }
                set { _int = value; }
            }

            public string Str
            {
                get { return _str; }
                set { _str = value; }
            }
        }

        private sealed class ResultThing
        {
            private readonly int _result;

            public ResultThing(int result)
            {
                _result = result;
            }

            public int Result
            {
                get { return _result; }
            }
        }

        public class DummyService : IService
        {
            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestMethodImpl : IMethodImpl
        {
            public IService InvokeService;
            public object[] InvokeArgs;
            public ResultThing InvokeResult;
            public IAsyncResult EndInvokeAsyncResult;
            public bool IsAsynchronousCalled;

            public object Invoke(IService service, object[] args)
            {
                InvokeService = service;
                InvokeArgs = args;
                return InvokeResult;
            }

            public bool IsAsynchronous
            {
                get
                {
                    IsAsynchronousCalled = true;
                    return false;
                }
            }

            public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState)
            {
                Invoke(service, args);
                return new DummyAsyncResult();
            }

            public object EndInvoke(IService service, IAsyncResult asyncResult)
            {
                EndInvokeAsyncResult = asyncResult;
                return InvokeResult;
            }

            private sealed class DummyAsyncResult : IAsyncResult
            {
                public bool IsCompleted { get { throw new NotImplementedException(); } }
                public WaitHandle AsyncWaitHandle { get { throw new NotImplementedException(); } }
                public object AsyncState { get { throw new NotImplementedException(); } }
                public bool CompletedSynchronously { get { throw new NotImplementedException(); } }
            }
        }
    }
}





