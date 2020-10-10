using System;

namespace FakerLib.PluginsSupport
{
    public interface IGenerator
    {
        object Generate(GeneratorContext context);
        bool CanGenerate(Type type);
    }
}
