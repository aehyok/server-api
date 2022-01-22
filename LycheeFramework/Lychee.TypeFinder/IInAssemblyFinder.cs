using System;
using System.Collections.Generic;

namespace Lychee.TypeFinder
{
    public interface IInAssemblyFinder : IEnumerable<Type>
    {
        /// <summary>
        /// Filters types that have a parameterless constructor.
        /// </summary>
        IInAssemblyFinder WithParameterlessConstructor { get; }

        /// <summary>
        /// Excludes some hard-coded types from the result.
        /// </summary>
        /// <param name="types">The types to exclude.</param>
        IInAssemblyFinder Excluding(params Type[] types);

        /// <summary>
        /// Filters types that inherit or implement <paramref name="type"/>. This is checked using <see cref="Type.IsAssignableFrom(Type)"/>, so both class and interface types can be used.
        /// </summary>
        /// <param name="type">The type of the class or interface types must inherit.</param>
        IInAssemblyFinder ThatInherit(Type type);

        /// <summary>
        /// Filters types that inherit or implement <paramref name="type"/>. This is checked using <see cref="Type.IsAssignableFrom(Type)"/>, so both class and interface types can be used.
        /// </summary>
        /// <typeparam name="T">The type of the class or interface types must inherit.</typeparam>
        IInAssemblyFinder ThatInherit<T>();

        /// <summary>
        /// Filters types that inherit the generic type <paramref name="genericType"/>, for example <see cref="List{T}"/>.
        /// </summary>
        /// <param name="genericType">The generic type that types must inherit, for example: <code>typeof(List&lt;&gt;)</code></param>
        IInAssemblyFinder ThatInheritGenericType(Type genericType);

        /// <summary>
        /// Filters types whose <see cref="MemberInfo.Name"/> matches the specified regex pattern.
        /// </summary>
        /// <param name="regex">The regex pattern to match against.</param>
        IInAssemblyFinder WhoseNameMatches(string regex);

        /// <summary>
        /// Filters types whose <see cref="Type.FullName"/> matches the specified regex pattern.
        /// </summary>
        /// <param name="regex">The regex pattern to match against.</param>
        IInAssemblyFinder WhoseFullNameMatches(string regex);

        /// <summary>
        /// Filters types whose <see cref="Type.Namespace"/> exactly matches <paramref name="ns"/>.
        /// </summary>
        /// <param name="ns">The namespace that types must have.</param>
        IInAssemblyFinder InNamespace(string ns);

        /// <summary>
        /// Filters types that have the <paramref name="attrType"/> attribute.
        /// </summary>
        /// <param name="attrType">The attribute that types must define.</param>
        IInAssemblyFinder WithAttribute(Type attrType);

        /// <summary>
        /// Filters types that have the <typeparamref name="T"/> attribute.
        /// </summary>
        /// <typeparam name="T">The attribute that types must define.</typeparam>
        IInAssemblyFinder WithAttribute<T>() where T : Attribute;
    }
}