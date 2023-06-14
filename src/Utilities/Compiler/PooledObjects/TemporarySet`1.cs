﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Analyzer.Utilities.PooledObjects
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not used in this context")]
    internal struct TemporarySet<T>
    {
#pragma warning disable CS0649 // Field 'TemporarySet<T>.Empty' is never assigned to, and will always have its default value
        public static readonly TemporarySet<T> Empty;
#pragma warning restore CS0649 // Field 'TemporarySet<T>.Empty' is never assigned to, and will always have its default value

        /// <summary>
        /// An empty set used for creating non-null enumerators when no items have been added to the set.
        /// </summary>
        private static readonly HashSet<T> EmptyHashSet = new();

        // 🐇 PERF: use PooledHashSet<T> instead of PooledConcurrentSet<T> due to allocation overhead in
        // clearing the set for returning it to the pool.
        private PooledHashSet<T>? _storage;

        public readonly Enumerable NonConcurrentEnumerable
            => new(_storage ?? EmptyHashSet);

        public void Free(CancellationToken cancellationToken)
        {
            Interlocked.Exchange(ref _storage, null)?.Free(cancellationToken);
        }

        private PooledHashSet<T> GetOrCreateStorage(CancellationToken cancellationToken)
        {
            if (_storage is not { } storage)
            {
                var newStorage = PooledHashSet<T>.GetInstance();
                storage = Interlocked.CompareExchange(ref _storage, newStorage, null) ?? newStorage;
                if (storage != newStorage)
                {
                    // Another thread initialized the value. Make sure to release the unused object.
                    newStorage.Free(cancellationToken);
                }
            }

            return storage;
        }

        public bool Add(T item, CancellationToken cancellationToken)
        {
            var storage = GetOrCreateStorage(cancellationToken);
            lock (storage)
            {
                return storage.Add(item);
            }
        }

        public readonly bool Contains(T item)
        {
            if (_storage is not { } storage)
                return false;

            lock (storage)
            {
                return storage.Contains(item);
            }
        }

        public readonly bool Contains_NonConcurrent(T item)
        {
            if (_storage is not { } storage)
                return false;

            return storage.Contains(item);
        }

        public readonly Enumerator GetEnumerator_NonConcurrent()
        {
            return new Enumerator((_storage ?? EmptyHashSet).GetEnumerator());
        }

        public readonly struct Enumerable(HashSet<T> set)
        {
            private readonly HashSet<T> _set = set;

            public Enumerator GetEnumerator()
                => new(_set.GetEnumerator());
        }

        public struct Enumerator(HashSet<T>.Enumerator enumerator)
        {
            private HashSet<T>.Enumerator _enumerator = enumerator;

            public bool MoveNext()
                => _enumerator.MoveNext();

            public T Current
                => _enumerator.Current;
        }
    }
}
