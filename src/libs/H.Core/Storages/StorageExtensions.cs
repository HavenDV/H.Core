﻿using System;

namespace H.Core.Storages
{
    public static class StorageExtensions
    {
        public static T? GetOrAdd<T>(this IStorage<T?> storage, string key, T value = default)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!storage.ContainsKey(key))
            {
                storage[key] = value;
            }

            return storage[key];
        }
    }
}