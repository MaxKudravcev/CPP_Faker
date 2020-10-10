using System;
using FakerLib.PluginsSupport;

namespace CharGenerator
{
    public class CharGenerator : Generator<char>
    {
        public override char Generate() => (char)new Random().Next(256);
    }
}
