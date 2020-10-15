using FakerLib.PluginsSupport;
using System;

namespace FakerApp
{
    public class StandStringGenerator : Generator<string>
    {
        public static readonly string[] s = { "Hermit Purple", "Star Platium", "Crazy Diamond", "Gold Experience" };

        public override string Generate() => s[(new Random()).Next(4)];
    }
}