﻿namespace Dawn
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <content>Provides preconditions for <see cref="IEnumerable" /> arguments.</content>
    public static partial class Guard
    {
        #region Methods

        /// <summary>Requires the argument to have a collection value that is empty.</summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that
        ///     will be thrown if the precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> has one or more items.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> Empty<TCollection>(
            in this ArgumentInfo<TCollection> argument, Func<TCollection, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.Count(argument.Value, 1) != 0)
            {
                var m = message?.Invoke(argument.Value) ?? Messages.CollectionEmpty(argument);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that is not empty.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that
        ///     will be thrown if the precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> is not <c>null</c> and has no items.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> NotEmpty<TCollection>(
            in this ArgumentInfo<TCollection> argument, Func<TCollection, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.Count(argument.Value, 1) == 0)
            {
                var m = message?.Invoke(argument.Value) ?? Messages.CollectionNotEmpty(argument);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that contains at least the
        ///     specified number of items.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="minCount">
        ///     The minimum number of items that the argument value is allowed to contain.
        /// </param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> contains less than the specified number of items.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> MinCount<TCollection>(
            in this ArgumentInfo<TCollection> argument, int minCount, Func<TCollection, int, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.Count(argument.Value, minCount) < minCount)
            {
                var m = message?.Invoke(argument.Value, minCount) ?? Messages.CollectionMinCount(argument, minCount);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that does not contain more than the
        ///     specified number of items.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="maxCount">
        ///     The maximum number of items that the argument value is allowed to contain.
        /// </param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> contains more than the specified number of items.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> MaxCount<TCollection>(
            in this ArgumentInfo<TCollection> argument, int maxCount, Func<TCollection, int, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.Count(argument.Value, maxCount + 1) > maxCount)
            {
                var m = message?.Invoke(argument.Value, maxCount) ?? Messages.CollectionMaxCount(argument, maxCount);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value whose number of items is between
        ///     the specified minimum and maximum values.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="minCount">
        ///     The minimum number of items that the argument value is allowed to contain.
        /// </param>
        /// <param name="maxCount">
        ///     The maximum number of items that the argument value is allowed to contain.
        /// </param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     The number of items that the <paramref name="argument" /> value contains is either
        ///     less than <paramref name="minCount" /> or greater than <paramref name="maxCount" />.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> CountInRange<TCollection>(
            in this ArgumentInfo<TCollection> argument, int minCount, int maxCount, Func<TCollection, int, int, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue())
            {
                var count = Collection<TCollection>.Count(argument.Value, maxCount + 1);
                if (count < minCount || count > maxCount)
                {
                    var m = message?.Invoke(argument.Value, minCount, maxCount)
                        ?? Messages.CollectionCountInRange(argument, minCount, maxCount);

                    throw new ArgumentException(m, argument.Name);
                }
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that contains the specified item.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the collection items.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="item">The item that the argument value is required to contain.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> does not contain <paramref name="item" />.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> Contains<TCollection, TItem>(
            in this ArgumentInfo<TCollection> argument, in TItem item, Func<TCollection, TItem, string> message = null)
            where TCollection : IEnumerable
            => ref argument.Contains(item, null, message);

        /// <summary>
        ///     Requires the argument to have a collection value that contains the specified item.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the collection items.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="item">The item that the argument value is required to contain.</param>
        /// <param name="comparer">The equality comparer to use.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> does not contain <paramref name="item" /> by the
        ///     comparison made by <paramref name="comparer" />.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> Contains<TCollection, TItem>(
            in this ArgumentInfo<TCollection> argument,
            in TItem item,
            IEqualityComparer<TItem> comparer,
            Func<TCollection, TItem, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && !Collection<TCollection>.Contains(argument.Value, item, comparer))
            {
                var m = message?.Invoke(argument.Value, item) ?? Messages.CollectionContains(argument, item);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that does not contain the
        ///     specified item.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the collection items.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="item">The item that the argument value is required not to contain.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> contains <paramref name="item" />.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> DoesNotContain<TCollection, TItem>(
            in this ArgumentInfo<TCollection> argument, in TItem item, Func<TCollection, TItem, string> message = null)
            where TCollection : IEnumerable
            => ref argument.DoesNotContain(item, null, message);

        /// <summary>
        ///     Requires the argument to have a collection value that does not contain the
        ///     specified item.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the collection items.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="item">The item that the argument value is required not to contain.</param>
        /// <param name="comparer">The equality comparer to use.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> contains <paramref name="item" /> by the comparison
        ///     made by <paramref name="comparer" />.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> DoesNotContain<TCollection, TItem>(
            in this ArgumentInfo<TCollection> argument,
            in TItem item,
            IEqualityComparer<TItem> comparer,
            Func<TCollection, TItem, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.Contains(argument.Value, item, comparer))
            {
                var m = message?.Invoke(argument.Value, item) ?? Messages.CollectionDoesNotContain(argument, item);
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that contains a <c>null</c> element.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> does not contain <c>null</c>.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> ContainsNull<TCollection>(
            in this ArgumentInfo<TCollection> argument, Func<TCollection, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && !Collection<TCollection>.ContainsNull(argument.Value))
            {
                var m = message?.Invoke(argument.Value) ?? Messages.CollectionContains(argument, "null");
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        /// <summary>
        ///     Requires the argument to have a collection value that does not contain a
        ///     <c>null</c> element.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="argument">The collection argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> contains <c>null</c>.
        /// </exception>
        public static ref readonly ArgumentInfo<TCollection> DoesNotContainNull<TCollection>(
            in this ArgumentInfo<TCollection> argument, Func<TCollection, string> message = null)
            where TCollection : IEnumerable
        {
            if (argument.HasValue() && Collection<TCollection>.ContainsNull(argument.Value))
            {
                var m = message?.Invoke(argument.Value) ?? Messages.CollectionDoesNotContain(argument, "null");
                throw new ArgumentException(m, argument.Name);
            }

            return ref argument;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        ///     Provides cached collection utilities for the type
        ///     <typeparamref name="TCollection" />.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        private static class Collection<TCollection>
            where TCollection : IEnumerable
        {
            /// <summary>
            ///     A function that returns the number of elements in the specified collection.
            ///     It enumerates the collection and counts the elements if the collection does not
            ///     provide a Count/Length property. The integer parameter specifies the maximum
            ///     number of iterations.
            /// </summary>
            public static readonly Func<TCollection, int, int> Count = InitCount();

            /// <summary>
            ///     A function that returns a value that indicates whether the specified collection
            ///     contains a <c>null</c> element. It enumerates the collection and checks the
            ///     elements one by one if the collection does not provide a Contains method that
            ///     accepts a single, nullable argument.
            /// </summary>
            public static readonly Func<TCollection, bool> ContainsNull = InitContainsNull();

            /// <summary>
            ///     Determines whether a generic collection contains the specified element.
            /// </summary>
            /// <typeparam name="TItem">The type of the item to find.</typeparam>
            /// <param name="collection">The collection to search.</param>
            /// <param name="item">The item to find.</param>
            /// <param name="comparer">The equality comparer to use.</param>
            /// <returns>
            ///     <c>true</c>, if <paramref name="collection" /> contains
            ///     <paramref name="item" />; otherwise, <c>false</c>.
            /// </returns>
            public static bool Contains<TItem>(
                TCollection collection, TItem item, IEqualityComparer<TItem> comparer)
                => Typed<TItem>.Contains(collection, item, comparer);

            /// <summary>Initializes <see cref="Count" />.</summary>
            /// <returns>
            ///     A function that returns the number of elements in the specified collection.
            /// </returns>
            private static Func<TCollection, int, int> InitCount()
            {
                var type = typeof(TCollection);
                var integer = typeof(int);

                var getter = type.GetPropertyGetter("Count");
                if (getter?.IsStatic == false && getter.ReturnType == integer)
                    return Compile();

                getter = type.GetPropertyGetter("Length");
                if (getter?.IsStatic == false && getter.ReturnType == integer)
                    return Compile();

                return (collection, max) =>
                {
                    var i = 0;
                    var enumerator = collection.GetEnumerator();
                    try
                    {
                        for (; i < max && enumerator.MoveNext(); i++)
                        {
                        }
                    }
                    finally
                    {
                        (enumerator as IDisposable)?.Dispose();
                    }

                    return i;
                };

                Func<TCollection, int, int> Compile()
                {
                    var t = Expression.Parameter(type, "collection");
                    var c = Expression.Call(t, getter);
                    var l = Expression.Lambda<Func<TCollection, int>>(c, t);
                    var count = l.Compile();
                    return (collection, max) => count(collection);
                }
            }

            /// <summary>Initializes <see cref="ContainsNull" />.</summary>
            /// <returns>
            ///     A function that returns a value that indicates whether the specified collection
            ///     contains a <c>null</c> element.
            /// </returns>
            private static Func<TCollection, bool> InitContainsNull()
            {
                const string name = "Contains";
                var type = typeof(TCollection);

                IEnumerable<MethodInfo> search;
#if NETSTANDARD1_0
                search = type.GetTypeInfo().GetDeclaredMethods(name).Where(m => m.IsPublic && !m.IsStatic);
#else
                search = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.Name == name);
#endif

                var methods = search.Where(m => m.ReturnType == typeof(bool)).ToList();
                if (methods.Count > 0)
                {
                    var nullableContains = null as MethodInfo;
                    var nullableContainsParamType = null as Type;
                    var foundVal = false;
                    foreach (var method in methods)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length != 1)
                            continue;

                        var paramType = parameters[0].ParameterType;
                        if (!IsValueType(paramType) || paramType.IsGenericType(typeof(Nullable<>)))
                        {
                            nullableContains = method;
                            nullableContainsParamType = paramType;
                            break;
                        }

                        foundVal = true;
                    }

                    if (nullableContains != null)
                    {
                        var t = Expression.Parameter(type, "collection");
                        var v = IsValueType(nullableContainsParamType)
                            ? Activator.CreateInstance(nullableContainsParamType)
                            : null;

                        var i = Expression.Constant(v, nullableContainsParamType);
                        var c = Expression.Call(t, nullableContains, i);
                        var l = Expression.Lambda<Func<TCollection, bool>>(c, t);
                        return l.Compile();
                    }

                    if (foundVal)
                        return collection => false;
                }

                return collection =>
                {
                    foreach (var item in collection)
                        if (item == null)
                            return true;

                    return false;
                };
            }

            /// <summary>
            ///     Provides cached collection utilities for collections that contain instances of
            ///     <typeparamref name="TItem" />.
            /// </summary>
            /// <typeparam name="TItem">The type of the collection items.</typeparam>
            private static class Typed<TItem>
            {
                /// <summary>
                ///     A function that determines whether a generic collection contains the
                ///     specified element.
                /// </summary>
                public static readonly Func<TCollection, TItem, IEqualityComparer<TItem>, bool> Contains
                    = InitContains();

                /// <summary>Initializes <see cref="Contains" />.</summary>
                /// <returns>
                ///     A function that determines whether a generic collection contains the
                ///     specified element.
                /// </returns>
                private static Func<TCollection, TItem, IEqualityComparer<TItem>, bool> InitContains()
                {
                    var collectionType = typeof(TCollection);
                    var itemType = typeof(TItem);
                    do
                    {
                        var method = collectionType.GetMethod("Contains", new[] { itemType });
                        if (method?.IsStatic == false && method.ReturnType == typeof(bool))
                        {
                            var t = Expression.Parameter(collectionType, "collection");
                            var i = Expression.Parameter(itemType, "item");
                            var c = Expression.Call(t, method, i);
                            var l = Expression.Lambda<Func<TCollection, TItem, bool>>(c, t, i);
                            var compiled = l.Compile();
                            return (collection, item, comparer) => comparer != null
                                ? EnumeratingContains(collection, item, comparer)
                                : compiled(collection, item);
                        }

                        itemType = itemType.GetBaseType();
                    }
                    while (itemType != null);
                    return EnumeratingContains;
                }

                /// <summary>
                ///     Determines whether a generic collection contains the specified element by
                ///     enumerating the collection and checking the items one by one.
                /// </summary>
                /// <param name="collection">The collection to search.</param>
                /// <param name="item">The item to find.</param>
                /// <param name="comparer">The equality comparer to use.</param>
                /// <returns>
                ///     <c>true</c>, if <paramref name="collection" /> contains
                ///     <paramref name="item" />; otherwise, <c>false</c>.
                /// </returns>
                private static bool EnumeratingContains(TCollection collection, TItem item, IEqualityComparer<TItem> comparer)
                {
                    if (comparer is null)
                        comparer = EqualityComparer<TItem>.Default;

                    if (collection is IEnumerable<TItem> typed)
                    {
                        foreach (var current in typed)
                            if (comparer.Equals(current, item))
                                return true;
                    }
                    else
                    {
                        foreach (var current in collection)
                            if (current is TItem c && comparer.Equals(c, item))
                                return true;
                    }

                    return false;
                }
            }
        }

        #endregion Classes
    }
}
