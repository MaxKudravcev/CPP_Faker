using System;

namespace FakerLib.PluginsSupport
{
    public class GeneratorContext
    {
        public Type TargetType { get; }
        public GeneratorContext(Type targetType)
        {
            TargetType = targetType;
        }
    }
}