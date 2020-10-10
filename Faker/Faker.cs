using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FakerLib.PluginsSupport;
using System.IO;

namespace FakerLib
{
    public class Faker
    {
        private List<IGenerator> generators = new List<IGenerator>();

        public Faker()
        {
            List<TypeInfo> generatorTypes = Assembly.GetExecutingAssembly().DefinedTypes.Where(ti => IsRequiredType(ti, typeof(Generator<>))).ToList();
            generatorTypes.ForEach(ti => generators.Add((IGenerator)Activator.CreateInstance(ti.AsType())));

            string plugDir = Directory.GetCurrentDirectory() + "/Plugins";
            if (Directory.Exists(plugDir))
            {
                var pluginsPaths = Directory.EnumerateFiles(plugDir, "*.dll").ToList();
                pluginsPaths.ForEach(path =>
                {
                    Assembly plugin = Assembly.LoadFrom(path);
                    var types = plugin.GetTypes().Where(t => IsRequiredType(t, typeof(Generator<>))).ToList();
                    types.ForEach(t => generators.Add((IGenerator)Activator.CreateInstance(t)));
                });
                
            }
            else Directory.CreateDirectory(plugDir);
        }

        public T Create<T>() => (T)Create(typeof(T));

        private object Create(Type t)
        {
            IGenerator generator = FindGenerator(t);
            if (generator != null)
                return generator.Generate(new GeneratorContext(t));

            (object, ParameterInfo[]) objAndParams = CreateObject(t);


            MemberInfo[] fieldsAndProps = t.GetFields();
            fieldsAndProps.Concat(t.GetProperties());
            foreach (MemberInfo mi in fieldsAndProps)
            {
                if (objAndParams.Item2.Single(pi => pi.Name == mi.Name) == null)
                {                    
                    (mi as FieldInfo)?.SetValue(objAndParams.Item1, Create(((FieldInfo)mi).FieldType));
                    if((mi as PropertyInfo)?.CanWrite == true)
                        ((PropertyInfo)mi).SetValue(objAndParams.Item1, Create(((PropertyInfo)mi).PropertyType));
                }   
            }
            
        }

        private bool IsRequiredType(Type type, Type required)
        {
            while (type != null && type != typeof(object))
            {
                Type t = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

                if (t == required) return true;
                type = type.BaseType;
            }
            return false;
        }

        private IGenerator FindGenerator(Type t)
        {
            return generators.Single(g => g.CanGenerate(t));
        }

        private (object, ParameterInfo[]) CreateObject(Type t)
        {
            ConstructorInfo[] ctors = t.GetConstructors();
            if (ctors.Length == 0 && t.IsClass)
                return (null, null);
            else if (ctors.Length == 0)
            {
                try
                {
                    return (Activator.CreateInstance(t), new ParameterInfo[0]);
                }
                                    
                catch
                {
                    return (null, null);
                }       
            }
                
            ctors.OrderByDescending(ci => ci.GetParameters().Length);

            object obj = null;
            ParameterInfo[] ctorParamInfos = null;

            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] parametersInfo = ctor.GetParameters();
                object[] parameters = new object[parametersInfo.Length];
                for (int i = 0; i < parameters.Length; i++)
                    parameters[i] = Create(parametersInfo[i].ParameterType);

                try
                {
                    obj = ctor.Invoke(parameters);
                    ctorParamInfos = parametersInfo;
                    break;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            if (obj == null)
                return (null, null);
            return (obj, ctorParamInfos);
        }
    }
}