using System;
using FakerLib.PluginsSupport;

namespace BoolGenerator
{
    public class BoolGenerator : Generator<bool>
    {
        public override bool Generate() =>  Convert.ToBoolean(new Random().Next(2));
    }
}
