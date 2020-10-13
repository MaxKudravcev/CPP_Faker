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
        private readonly int MaxCycle = 10;
        private List<IGenerator> generators = new List<IGenerator>();
        private Stack<Type> nestingStack = new Stack<Type>();
        private FakerConfig config = null;

        public Faker(FakerConfig config = null)
        {
            this.config = config;
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
            if (nestingStack.Where(type => type == t).Count() >= MaxCycle)
                return GetDefaultValue(t);
            nestingStack.Push(t);

            IGenerator generator = FindGenerator(t);
            if (generator != null)
            {
                nestingStack.Pop();
                return generator.Generate(new GeneratorContext(t));
            }

            (object, ParameterInfo[]) objAndParams = CreateObject(t);
            if (objAndParams.Item1 == null)
            {
                nestingStack.Pop();
                return GetDefaultValue(t);
            }

            nestingStack.Pop();
            return SetFieldsAndProps(t, objAndParams.Item1, objAndParams.Item2);
        }

        private static bool IsRequiredType(Type type, Type required)
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

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        private static bool IsDefaultValue(object obj, MemberInfo mi)
        {
            if ((mi as FieldInfo)?.GetValue(obj) == GetDefaultValue((mi as FieldInfo).FieldType))
                return true;
            else if ((mi as PropertyInfo)?.GetValue(obj) == GetDefaultValue((mi as PropertyInfo).PropertyType))
                return true;
            return false;
        }

        private (object, ParameterInfo[]) CreateObject(Type t)
        {
            ConstructorInfo[] ctors = t.GetConstructors();
            if (ctors.Length == 0 && t.IsClass)
                return (null, null);
                
            ctors.OrderByDescending(ci => ci.GetParameters().Length);

            object obj = null;
            ParameterInfo[] ctorParamInfos = null;

            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] parametersInfo = ctor.GetParameters();
                object[] parameters = new object[parametersInfo.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    FakerConfig.Rule rule = config?.Rules.Find(Rule => (Rule.MemberType == parametersInfo[i].ParameterType && Rule.MemberName == parametersInfo[i].Name));
                    if (rule != null)
                        parameters[i] = (Activator.CreateInstance(rule.GeneratorType) as IGenerator).Generate(new GeneratorContext(rule.MemberType));
                    parameters[i] = Create(parametersInfo[i].ParameterType);
                }

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

            if (obj == null && t.IsValueType)
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
            else if (obj == null)
                return (null, null);

            return (obj, ctorParamInfos);
        }

        private object SetFieldsAndProps(Type t, object obj, ParameterInfo[]  pInfos)
        {
            MemberInfo[] fieldsAndProps = t.GetFields();
            fieldsAndProps.Concat(t.GetProperties());
            foreach (MemberInfo mi in fieldsAndProps)
            {
                if (pInfos.Single(pi => pi.Name == mi.Name) == null && IsDefaultValue(obj, mi))
                {
                    FakerConfig.Rule rule = config?.Rules.Find(Rule => (Rule.MemberName == mi.Name));
                    if(rule != null)
                    {
                        if (mi is FieldInfo fi && fi.FieldType == rule.MemberType)
                            fi.SetValue(obj, (Activator.CreateInstance(rule.GeneratorType) as IGenerator).Generate(new GeneratorContext(rule.MemberType)));
                        else if ((mi as PropertyInfo).PropertyType == rule.MemberType && (mi as PropertyInfo).CanWrite)
                            (mi as PropertyInfo).SetValue(obj, (Activator.CreateInstance(rule.GeneratorType) as IGenerator).Generate(new GeneratorContext(rule.MemberType)));
                    }

                    (mi as FieldInfo)?.SetValue(obj, Create(((FieldInfo)mi).FieldType));
                    if ((mi as PropertyInfo)?.CanWrite == true)
                        ((PropertyInfo)mi).SetValue(obj, Create(((PropertyInfo)mi).PropertyType));
                }
            }
            return obj;
        }
    }
}