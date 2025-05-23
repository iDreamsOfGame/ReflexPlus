using System.Collections.Generic;
using Newtonsoft.Json;
using ReflexPlus.Sample.Application;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class CollectionStoragePrefs : ICollectionStorage
    {
        private readonly HashSet<string> storage;

        public CollectionStoragePrefs()
        {
            var json = PlayerPrefs.GetString("collection-storage", "[]");
            storage = JsonConvert.DeserializeObject<HashSet<string>>(json);
        }

        public void Clear()
        {
            storage.Clear();
            Persist();
        }

        public void Add(string id)
        {
            storage.Add(id);
            Persist();
        }

        public int Count()
        {
            return storage.Count;
        }

        public bool IsCollected(string id)
        {
            return storage.Contains(id);
        }

        private void Persist()
        {
            var json = JsonConvert.SerializeObject(storage);
            PlayerPrefs.SetString("collection-storage", json);
        }
    }
}