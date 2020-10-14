using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakerApp;
using System.Collections.Generic;

namespace FakerLib.Tests
{
    [TestClass]
    public class FakerTests
    {
        private class Foo
        {
            public int a { get; private set}
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

        static Faker faker;
        private static object GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        [TestInitialize]
        public void TestInit()
        {
            FakerConfig fc = new FakerConfig();
            //fc.Add<Foo, string, StandStringGenerator>(Foo => Foo.Stand);
            Faker faker = new Faker(fc);
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
    }
}
