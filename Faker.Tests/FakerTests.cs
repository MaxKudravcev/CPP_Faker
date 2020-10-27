using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakerApp;
using System.Collections.Generic;
using System.Linq;

namespace FakerLib.Tests
{
    [TestClass]
    public class FakerTests
    {
        private class Foo
        {
            public int a { get; private set; }
            public int b;
            public float d;
            public DateTime e { get; set; }
            public string f { get; private set; }
            public double g { get; set; }

            public Foo(int b, DateTime e, string f)
            {
                this.b = b;
                this.e = e;
                this.f = f;
            }            
        }

        private class Bar
        {
            public string stand;
            public string normal;
        }

        private struct PrivateCtorStruct
        {
            public int a { get; private set; }
            public double b { get; set; }

            private PrivateCtorStruct(int a, double b)
            {
                this.a = a;
                this.b = b;
            }
        }

        private struct PluginsTestStruct
        {
            public char c { get; private set; }
            public bool b { get; private set; }

            public PluginsTestStruct(char c, bool b)
            {
                this.c = c;
                this.b = b;
            }
        }

        private class A
        {
            public B b;
        }

        private class B
        {
            public A a;
        }

        static Faker faker;
        private static object GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        [TestInitialize]
        public void TestInit()
        {
            FakerConfig fc = new FakerConfig();
            fc.Add<Bar, string, StandStringGenerator>(Bar => Bar.stand);
            faker = new Faker(fc, 2);
        }
                
        [TestMethod]
        public void SimpleClassTest()
        {
            Foo foo = faker.Create<Foo>();

            Assert.IsNotNull(foo);
            Assert.AreEqual(GetDefaultValue(typeof(int)), foo.a);
            Assert.AreNotEqual(GetDefaultValue(typeof(int)), foo.b);
            Assert.AreNotEqual(GetDefaultValue(typeof(float)), foo.d);
            Assert.AreNotEqual(GetDefaultValue(typeof(DateTime)), foo.e);
            Assert.AreNotEqual(GetDefaultValue(typeof(double)), foo.f);
            Assert.AreNotEqual(GetDefaultValue(typeof(double)), foo.g);
        }

        [TestMethod]
        public void ListTest()
        {
            List<int> list = faker.Create<List<int>>();

            Assert.IsNotNull(list);
            Assert.AreNotEqual(0, list.Count);
            foreach (int i in list)
                Assert.AreNotEqual(GetDefaultValue(typeof(int)), i);
        }

        [TestMethod]
        public void CustomGeneratorTest()
        {
            Bar bar = faker.Create<Bar>();

            Assert.IsNotNull(bar);
            Assert.IsTrue(StandStringGenerator.s.Contains(bar.stand));
            Assert.IsFalse(StandStringGenerator.s.Contains(bar.normal));
        }

        [TestMethod]
        public void StructTest()
        {
            PrivateCtorStruct s = faker.Create<PrivateCtorStruct>();

            Assert.AreEqual(GetDefaultValue(typeof(int)), s.a);
            Assert.AreNotEqual(GetDefaultValue(typeof(double)), s.b);
        }

        //[TestMethod]
        //public void PluginsTest()
        //{
        //    PluginsTestStruct s = faker.Create<PluginsTestStruct>();

        //    Assert.AreNotEqual(GetDefaultValue(typeof(char)), s.c);
        //}

        [TestMethod]
        public void NestingTest()
        {
            A a = faker.Create<A>();

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.b);
            Assert.IsNotNull(a.b.a);
            Assert.IsNotNull(a.b.a.b);
            Assert.IsNull(a.b.a.b.a);
        }
    }
}
