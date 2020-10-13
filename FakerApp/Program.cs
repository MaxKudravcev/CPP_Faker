using FakerLib;
using System;
using System.Collections.Generic;

namespace FakerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FakerConfig fc = new FakerConfig();
            fc.Add<Foo, string, StandStringGenerator>(Foo => Foo.Stand);
            Faker faker = new Faker(fc);

            Foo foo = faker.Create<Foo>();
            DateTime dt = faker.Create<DateTime>();
            List<float> l = faker.Create<List<float>>();

            Console.WriteLine(foo);
            Console.WriteLine(dt);
            foreach (float f in l)
                Console.WriteLine(f);

            Console.ReadLine();
        }
    }
}
