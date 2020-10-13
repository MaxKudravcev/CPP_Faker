using FakerLib.PluginsSupport;
using System;
using System.Text;

namespace FakerLib
{
    internal class IntGenerator : Generator<int>
    {
        public override int Generate() => new Random().Next();
    }

    internal class LongGenerator : Generator<long>
    {
        public override long Generate()
        {
            Random r = new Random();
            return r.Next() << 32 | r.Next();
        }
    }

    internal class DoubleGenerator : Generator<double>
    {
        public override double Generate() => new Random().NextDouble();
    }

    internal class FloatGenerator : Generator<float>
    {
        public override float Generate() => (float)new Random().NextDouble();
    }

    internal class StringGenerator : Generator<string>
    {
        public override string Generate()
        {
            Random r = new Random();
            byte[] b = new byte[r.Next(20)];
            r.NextBytes(b);
            return Encoding.UTF8.GetString(b);
        }
    }

    internal class DateTimeGenerator : Generator<DateTime>
    {
        public override DateTime Generate()
        {
            Random r = new Random();
            DateTime start = new DateTime(1990, 1, 1);
            TimeSpan range = (DateTime.Now - start);
            return start.AddDays(r.Next(range.Days));
            
            //return start.AddTicks(r.Next());
            
        }
    }
}
