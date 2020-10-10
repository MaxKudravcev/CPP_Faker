﻿using System;
using System.Collections.Generic;

namespace FakerLib.PluginsSupport
{
    public abstract class Generator<T> : IGenerator
    {
        public abstract T Generate();

        object IGenerator.Generate(GeneratorContext context)
        {
            if (context.TargetType is List<T>)
                return ListGenerator.GenerateList<T>(this);
            else return Generate();
        }

        bool IGenerator.CanGenerate(Type type)
        {
            if (type == typeof(T))
                return true;
            if (type is List<T>)
                return true;
            return false;
        }
    }
}
