using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FakerLib.PluginsSupport;

namespace FakerLib
{
    public class FakerConfig
    {
        internal class Rule
        {
            public Type TargetType { get; }
            public Type MemberType { get; }
            public Type GeneratorType { get; }
            public string MemberName { get; }

            public Rule(Type tt, Type mt, Type gt, string mn)
            {
                TargetType = tt;
                MemberType = mt;
                GeneratorType = gt;
                MemberName = mn;
            }

        }

        private List<Rule> rules = new List<Rule>();

        public void Add<T1, T2, T3>(Expression<Func<T1, T2>> exp) where T3 : Generator<T2>
        {
            rules.Add(new Rule(typeof(T1), typeof(T2), typeof(T3), ((MemberExpression)exp.Body).Member.Name));
        }
    }
}