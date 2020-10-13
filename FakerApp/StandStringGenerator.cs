using FakerLib.PluginsSupport;
using System;

namespace FakerApp
{
    class StandStringGenerator : Generator<string>
    {
        private readonly string[] s = { "Hermit Purple", "Star Platium", "Crazy Diamond", "Gold Experience" };

        public override string Generate() => s[(new Random()).Next(4)];
    }
}