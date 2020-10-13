using System;
using System.Collections.Generic;

namespace FakerLib.PluginsSupport
{
    public static class ListGenerator
    {
        public static List<T> GenerateList<T>(IGenerator generator)
        {
            if (generator.CanGenerate(typeof(T)))
            {
                List<T> list = new List<T>();
                for (int i = 0; i < new Random().Next(10); i++)
                    list.Add((T)generator.Generate(new GeneratorContext(typeof(T))));
                return list;
            }
            else throw new Exception("Invalid generator sent as a parameter");
        }
    }
}
