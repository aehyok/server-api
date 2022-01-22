using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Lychee.TypeFinder
{
    internal class InAssemblyFinder : IInAssemblyFinder
    {
        private IRule MainRule;
        private bool IsFrozen;

        private readonly Assembly[] Assemblies;

        public InAssemblyFinder(Assembly[] assemblies)
        {
            this.Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        private void CheckFrozen()
        {
            if (IsFrozen)
                throw new InvalidOperationException("Cannot modify after enumerating");
        }

        public IInAssemblyFinder WithRule(IRule rule)
        {
            CheckFrozen();
            MainRule = new CombinedRule(MainRule, rule);

            return this;
        }

        public IInAssemblyFinder Excluding(params Type[] types) => WithRule(new ExcludeTypesRule(types));

        public IInAssemblyFinder ThatInherit(Type type) => WithRule(new InheritanceRule(type));

        public IInAssemblyFinder ThatInherit<T>() => ThatInherit(typeof(T));

        public IInAssemblyFinder ThatInheritGenericType(Type genericType) => WithRule(new GenericSubclassRule(genericType));

        public IInAssemblyFinder WhoseNameMatches(string regex) => WithRule(new NameRegexRule(regex, false));

        public IInAssemblyFinder WhoseFullNameMatches(string regex) => WithRule(new NameRegexRule(regex, true));

        public IInAssemblyFinder InNamespace(string ns) => WithRule(new InNamespaceRule(ns));

        public IInAssemblyFinder WithParameterlessConstructor => WithRule(new ParameterlessCtorRule());

        public IInAssemblyFinder WithAttribute(Type attrType) => WithRule(new HasAttributeRule(attrType));

        public IInAssemblyFinder WithAttribute<T>() where T : Attribute => WithAttribute(typeof(T));

        IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
        {
            IsFrozen = true;

            foreach (var ass in Assemblies)
            {
                foreach (var type in ass.GetTypes())
                {
                    if (MainRule?.Complies(type) ?? true)
                    {
                        yield return type;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Type>)this).GetEnumerator();
    }
}